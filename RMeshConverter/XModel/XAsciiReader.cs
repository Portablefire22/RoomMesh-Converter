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
   public Dictionary<string, Template> Templates;
   public List<Frame> Frames;

   public List<Mesh> Meshes;
   
   public int unknown;
   
   public string Path;
   public XAsciiReader(string path)
   {
      using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
      Logger = factory.CreateLogger<XAsciiReader>();
      InputFileStream = File.Open(path, FileMode.Open);
      Path = path;

      Materials = new List<Material>();
      Templates = new Dictionary<string, Template>();
      Meshes = new List<Mesh>();
      Templates.Add("AnimationSet", new Template("", new Dictionary<string, dynamic>()));
      Frames = new List<Frame>();
   }

   public string ReadLine()
   {
      var bf = new byte[256];
      var i = 0;
      var isComment = false;
      while (true)
      {
         var x = InputFileStream.ReadByte();
         if (x == -1) throw new EndOfStreamException();
         bf[i] = (byte) x;
         if (bf[i++] == '\n') {break;}
      }
      // Get the next line if it was a comment
      var str = Encoding.UTF8.GetString(bf[new Range(0, i)]).Trim().TrimEnd('\r', '\n');
      if (str.Length == 0) return ReadLine();
      if (str[0] == '/' || str[0] == '#' || (str[0] == '{' && str.Length == 1)) isComment = true;
      if (isComment) return ReadLine();
      return str;
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
      var faceColor = ReadRgba();
      var power = float.Parse(ReadValue());
      var specularColor = ReadRgb();
      var emissiveColor = ReadRgb();
      string? textureFileName = null;

      var str = ReadLine();
      str = str.Replace("{", "").Trim();
      if (str == "TextureFilename")
      {
         var x = ReadValue();
         textureFileName = x.Replace("\"", "");
         ReadClosing();
         ReadClosing();
      } 
      return new Material(name, faceColor, power, specularColor, emissiveColor, textureFileName);
   }

   public string[] ReadUntil(char chr)
   {
      var strings = new List<string>();
      while (true)
      {
         var str = ReadLine();
         strings.Add(str);
         if (str.EndsWith(chr)) break;
      }

      return strings.ToArray();
   }
   
   public Matrix4x4 ReadTransformMatrix()
   {
      // Why is there no matrix4x4 parse :)
      var mat = new Matrix4x4();
      var lines = ReadUntil(';');
      var y = 0;
      for (int i = 0; i < lines.Length; i++)
      {
         var line = lines[i].Split(",");

         for (int x = 0; x < line.Length; x++)
         {
            if (line[x] == "") break;
            mat[x % 4, y] = float.Parse(line[x].Replace(";", ""));
            if (x % 4 == 0 && x != 0) y++;
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
      var end = x.Split(",");
      // The way vec2s are represented changes between files?
      if (end.Length == 3 || (end.Length > 1 && end[1].Contains(";;")))
      {
         end = x.Replace(";", "").Split(",");
      }
      else
      {
         end = x.Replace(",", "").Split(";");
      } 
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
         if (str[0] == '{')
         {
            materials.Add(str.Remove(str.Length-1).Remove(0, 1));
            continue;
         }

         var tmo = str.Split(" ");
         Materials.Add(ReadMaterial(tmo[1]));
         materials.Add(tmo[1]);
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
      var str = ReadLine().Replace("{", "").Trim();
      List<Vector2>? tex = new List<Vector2>();
      MeshNormals? norms = null;
      MeshMaterialList? mats = null;
      while (str != "}" && (norms == null || mats == null || tex != null))
      {
         switch (str)
         {
            case "MeshNormals":
               norms = ReadMeshNormals();
               break;
            case "MeshTextureCoords":
               tex = ReadMeshTextureCoords();
               break;
            case "MeshMaterialList":
               mats = ReadMeshMaterialList();
               break;
            default:
               if (Templates.ContainsKey(str))
               {
                  while (ReadLine() != "}") ;
               }
               break;
         }
         
         if (norms != null && mats != null && tex != null)
         {
            ReadLine();break;}

         str = ReadLine();
         str = str.Replace("{", "").Trim(); 
      }
      return new Mesh(name, verts, faces, tex, norms, mats);
   }

   public Frame ReadFrame(string name)
   {
      Logger.LogInformation("Reading Frame: {}", name);
      Templates.TryAdd("AnimTicksPerSecond", new Template("", new Dictionary<string, dynamic>()));
      Templates.TryAdd("VertexDuplicationIndices", new Template("", new Dictionary<string, dynamic>()));
      var frame = new Frame(name);
      var read = true;
      try
      {
         while (read)
         {
            var x = ReadLine();

            var templateName = x.Split(" ");
            switch (templateName[0])
            {
               case "FrameTransformMatrix":
                  frame.TransformMatrix = ReadTransformMatrix();
                  break;
               case "}":
                  read = false;
                  break;
               case "Header": // No fucking clue what this does
                  while (ReadLine() != "}") ;
                  break;
               case "Material":
                  Materials.Add(ReadMaterial(templateName[1]));
                  break;
               case "Frame":
                  frame.Children.Add(ReadFrame(templateName[1]));
                  break;
               case "Mesh":
                  frame.Meshes.Add(ReadMesh(templateName[1]));
                  break;
               case "template":
                  Templates.TryAdd(templateName[1], ReadTemplate(templateName[1]));
                  while (ReadLine().Trim() != "}") ;
                  break;
               default:
                  if (Templates.ContainsKey(templateName[0]))
                  {
                     Logger.LogInformation(templateName[0]);
                     while (true)
                     {
                        var xdas = ReadLine().Trim();
                        Logger.LogInformation(xdas);
                        if (xdas == "}") break;
                     };
                     Logger.LogInformation("broke");
                  }
                  else
                  {
                     Logger.LogCritical("Unknown Template '{}'", templateName[0]);
                     unknown++;
                     if (unknown > 10)
                     {
                        Logger.LogCritical("Uknown limit reached");
                        InputFileStream.Close();
                        return frame;
                     }
                  }

                  break;
            }
         }
      }
      catch (Exception e)
      {
         Logger.LogCritical(e.ToString());
      }

      return frame;
   }

   public Template ReadTemplate(string name)
   {
      Logger.LogInformation("Reading Template: {}", name);
      while (ReadLine() != "}") ;
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
         Frames.Add(ReadFrame("root"));
      }

      catch (Exception e)
      {
         Logger.LogCritical("{}", e);
         Logger.LogCritical("{}", Path);
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