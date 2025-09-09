using System.IO;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace RMeshConverter.XModel;

public class XAsciiReader : MeshReader
{

   public List<Material> Materials;
   
   public XAsciiReader(string path)
   {
      using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
      Logger = factory.CreateLogger<XAsciiReader>();
      InputFileStream = File.Open(path, FileMode.Open);

      Materials = new List<Material>();
   }

   public string ReadLine()
   {
      var bf = new byte[128];
      var i = 0;
      var isComment = false;
      while (true)
      {
         bf[i] = (byte) InputFileStream.ReadByte();
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
   
   public Material ReadMaterial(string name)
   {
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
      ReadLine();
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

      return mat;
   }

   public Vector3 ReadVector3()
   {
      var x = ReadLine();
      var end = x.Split(";");
      return new Vector3(float.Parse(end[0]), float.Parse(end[1]), float.Parse(end[2]));
   }

   public MeshFace ReadMeshFace()
   {
      var line = ReadLine();
      var end = line.Split(";");
      var indexCount = int.Parse(end[0]);
      var indicesRaw = end[1].Split(",");
      var indices = new List<int>();
      for (int i = 0; i < indexCount; i++)
      {
         indices.Add(int.Parse(indicesRaw[i]));
      }

      return new MeshFace(indices);
   }
   
   public Mesh ReadMesh(string name)
   {
      var vertexCount = int.Parse(ReadValue());
      var verts = new List<Vector3>();
      for (int i = 0; i < vertexCount; i++)
      {
         verts.Add(ReadVector3());
      }
      var faceCount = int.Parse(ReadValue());
      var faces = new List<MeshFace>();
      for (int i = 0; i < faceCount; i++)
      {
          faces.Add(ReadMeshFace());
      }

      return new Mesh(name, verts, faces);
   }
   
   public Frame ReadFrame(string name)
   {
      var frame = new Frame();
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

   public void Convert()
   {
      if (ReadLine() != "xof 0302txt 0064")
      {
         throw new XException("input file is not a .x model");
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
                  break;
               case "Material":
                  Materials.Add(ReadMaterial(templateName[1]));
                  break;
               case "Frame":
                  ReadFrame(templateName[1]);
                  break;
               default:
                  Logger.LogCritical("Unknown Template '{}'", templateName[0]);
                  break;
            }
         }
      }
      catch (Exception e)
      {
         Logger.LogCritical("{}", e);
      } // We know when we done when we EOF :)
      InputFileStream.Close();
   }

   public override void Dispose()
   {
      throw new NotImplementedException();
   }
}