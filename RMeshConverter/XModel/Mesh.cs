using System.Numerics;

namespace RMeshConverter.XModel;

public class Mesh : Node
{
   public Mesh(string name, List<Vector3> vertices, List<MeshFace> faces, List<Vector2> textureCoords, MeshNormals meshNormals, MeshMaterialList materialList)
   {
      Name = name;
      Vertices = vertices;
      Faces = faces;
      TextureCoords = textureCoords;
      MeshNormals = meshNormals;
      MaterialList = materialList;
   }

   public string Name;
   public List<Vector3> Vertices;
   public List<MeshFace> Faces;
   public List<Vector2> TextureCoords;
   public MeshNormals MeshNormals;
   public MeshMaterialList MaterialList;
}