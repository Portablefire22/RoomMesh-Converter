using System.IO;
using System.Numerics;
using System.Text;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;

namespace RMeshConverter.XModel;

/* Credit https://paulbourke.net/dataformats/directx/#xfilefrm_Template_MeshMaterialList */

public class XAsciiReader : MeshReader
{

   public List<Material> Materials;
   public List<Template> Templates;
   public List<Frame> Frames;

   public string Path;
   public XAsciiReader(string path)
   {
      using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
      Logger = factory.CreateLogger<XAsciiReader>();
      InputFileStream = File.Open(path, FileMode.Open);
      Path = path;

      Materials = new List<Material>();
      Templates = new List<Template>();
      Frames = new List<Frame>();
   }

   public string ReadLine()
   {
      var bf = new byte[128];
      var i = 0;
      var isComment = false;
      while (true)
      {
         var x = InputFileStream.ReadByte();
         if (x == -1) throw new EndOfStreamException();
         bf[i] = (byte) x;
         if ((i == 0 || i == 1) && bf[i] == '/' || bf[0] == '#') isComment = true;
         if (bf[i++] == '\n') {break;}
      }
      // Get the next line if it was a comment
      if (isComment) return ReadLine();
      return Encoding.UTF8.GetString(bf[new Range(0, i)]).Trim();
   }

   public string ReadValue()
   {
      return ReadLine().Split(";")[0];
   }

   public Vector4 ReadRgba()
   {
      var x = ReadLine();
      var end = x.Split(";");
      return new Vector4(float.Parse(end[0]), float.Parse(end[1]), float.Parse(end[2]), float.Parse(end[3]));
   }

   public Vector3 ReadRgb()
   {
      var x = ReadLine();
      var end = x.Split(";");
      return new Vector3(float.Parse(end[0]), float.Parse(end[1]), float.Parse(end[2]));
   }

   public int ReadInt()
   {
      return int.Parse(ReadValue());
   }

   
   public void ReadClosing()
   {
      var str = ReadLine();
      if (str != "}") throw new XException($"Expected to read '}}' instead got '{str}'");
   }

   /// <summary>
   /// Reads an array value, only reading correctly if each value is on a new line
   /// </summary>
   /// <returns></returns>
   public string ReadArrayValue()
   {
      // Values typically end with ',' whilst the last values end with ';' or ';;'
      return ReadLine().Split(",")[0].Split(";")[0];
   }

   public int ReadArrayInt()
   {
      return int.Parse(ReadArrayValue());
   }
   
   public Material ReadMaterial(string name)
   {
      Logger.LogInformation("Reading Material: {}", name);
      ReadLine(); // Open Bracket
      var faceColor = ReadRgba();
      var power = float.Parse(ReadValue());
      var specularColor = ReadRgb();
      var emissiveColor = ReadRgb();
      string? textureFileName = null;
      if (ReadLine() == "TextureFilename")
      {
         ReadLine();
         var x = ReadValue();
         textureFileName = x.Replace("\"", "");
         ReadLine();
      } 
      ReadClosing();
      return new Material(name, faceColor, power, specularColor, emissiveColor, textureFileName);
   }

   public Matrix4x4 ReadTransformMatrix()
   {
      // Why is there no matrix4x4 parse :)
      var mat = new Matrix4x4();
      for (int y = 0; y < 4; y++)
      {
         var line = ReadLine().Split(",");
         for (int x = 0; x < 4; x++)
         {
            mat[x, y] = float.Parse(line[x].Replace(";", ""));
         }
      }
      ReadClosing();
      return mat;
   }

   public Vector3 ReadVector3()
   {
      var x = ReadLine();
      var end = x.Split(";");
      return new Vector3(float.Parse(end[0]), float.Parse(end[1]), float.Parse(end[2]));
   }
   
   public Vector2 ReadVector2()
   {
      var x = ReadLine();
      var end = x.Replace(";", "").Split(",");
      return new Vector2(float.Parse(end[0]), float.Parse(end[1]));
   }

   public MeshFace ReadMeshFace()
   {
      Logger.LogInformation("Reading Mesh Face");
      var line = ReadLine();
      var end = line.Split(";");
      var indexCount = int.Parse(end[0]);
      var indicesRaw = end[1].Split(",");
      var indices = new int[indexCount];
      for (int i = 0; i < indexCount; i++)
      {
         indices[i] = int.Parse(indicesRaw[i]);
      }
      Logger.LogInformation($"Read Mesh Indices: {indexCount}");
      return new MeshFace(indices);
   }

   public List<Vector2> ReadMeshTextureCoords()
   {
      Logger.LogInformation("Reading Mesh TextureCoords");
      var vertexCount = ReadInt();
      var verts = new List<Vector2>();
      for (int i = 0; i < vertexCount; i++)
      {
         verts.Add(ReadVector2());
      }

      ReadLine(); // "}"
      return verts;
   }

   public MeshNormals ReadMeshNormals()
   {
      Logger.LogInformation("Reading Mesh Normals");
      var normalCount = ReadInt();
      var normals = new List<Vector3>();
      for (int i = 0; i < normalCount; i++)
      {
         normals.Add(ReadVector3());
      }

      var faceCount = ReadInt();
      var faceNormals = new List<MeshFace>();
      for (int i = 0; i < faceCount; i++)
      {
         faceNormals.Add(ReadMeshFace());
      }
      ReadClosing();
      return new MeshNormals(normals, faceNormals);
   }

   public MeshMaterialList ReadMeshMaterialList()
   {
      Logger.LogInformation("Reading Mesh Material List");
      var materialCount = ReadInt();
      var indicesCount = ReadInt();
      var indices = new List<int>();
      for (int i = 0; i < indicesCount; i++)
      {
         indices.Add(ReadArrayInt());
      }

      var materials = new List<string>();
      for (int i = 0; i < materialCount; i++)
      {
         var str = ReadLine();
         materials.Add(str.Remove(str.Length-1).Remove(0, 1));
      }
      var materialsSame = materials.All(x => x == materials[0]);
      ReadClosing();
      return new MeshMaterialList(indices, materials, materialsSame);
   }
   
   public Mesh ReadMesh(string name)
   {
      Logger.LogInformation("Reading Mesh: {}", name);
      var vertexCount = ReadInt();
      var verts = new List<Vector3>();
      for (int i = 0; i < vertexCount; i++)
      {
         verts.Add(ReadVector3());
      }
      var faceCount = ReadInt();
      var faces = new List<MeshFace>();
      for (int i = 0; i < faceCount; i++)
      {
          faces.Add(ReadMeshFace());
      }
      ReadLine(); // "MeshTextureCoords"
      ReadLine(); // "{"
      
      var tex = ReadMeshTextureCoords();
      ReadLine(); // "MeshNormals"
      ReadLine(); // "{"
      var norms = ReadMeshNormals();
      ReadLine();
      ReadLine();
      var mats = ReadMeshMaterialList();
      ReadClosing();
      return new Mesh(name, verts, faces, tex, norms, mats);
   }

   public Frame ReadFrame(string name)
   {
      Logger.LogInformation("Reading Frame: {}", name);
      var frame = new Frame(name);
      ReadLine();
      var read = true;
      while (read)
      {
         var x = ReadLine();

         var templateName = x.Split(" ");
         switch (templateName[0])
         {
            case "FrameTransformMatrix":
               ReadLine();
               frame.TransformMatrix = ReadTransformMatrix();
               break;
            case "Mesh":
               ReadLine();
               frame.Meshes.Add(ReadMesh(templateName[1]));
               break;
            
            case "}":
               read = false;
               break;
         }
      }
      return frame;
   }

   public Template ReadTemplate(string name)
   {
      Logger.LogInformation("Reading Template: {}", name);
      ReadClosing();
      return new Template("", new Dictionary<string, dynamic>());
   }
   
   public void Convert()
   {
      var magic = ReadLine();
      if (magic != "xof 0302txt 0064" && magic != "xof 0303txt 0032")
      {
         throw new XException($"input file '{Path}' is not a .x model");
      }

      try
      {
         while (true)
         {
            var x = ReadLine();

            // Templates are {TYPE} {NAME}
            var templateName = x.Split(" ");
            switch (templateName[0])
            {
               case "Header": // No fucking clue what this does
                  while (ReadLine() != "}") ;
                  break;
               case "Material":
                  Materials.Add(ReadMaterial(templateName[1]));
                  break;
               case "Frame":
                  Frames.Add(ReadFrame(templateName[1]));
                  break;
               case "template":
                  Templates.Add(ReadTemplate(templateName[1]));
                  break;
               default:
                  Logger.LogCritical("Unknown Template '{}'", templateName[0]);
                  break;
            }
         }
      }
      catch (EndOfStreamException e)
      {
      }
      catch (Exception e)
      {
         Logger.LogCritical("{}", e);
      } // We know when we done when we EOF :)

      Logger.LogInformation("Finished Reading X Model: {}", Path);
      InputFileStream.Close();
   }

   public override void Dispose()
   {
      Frames.Clear();
      Materials.Clear();
      Templates.Clear();
      InputFileStream.Dispose();
   }
}