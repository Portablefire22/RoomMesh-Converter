using System.Numerics;

namespace RMeshConverter.XModel;

public class Mesh : Node
{
   public Mesh(string name, List<Vector3> vertices, List<MeshFace> faces)
   {
      Name = name;
      Vertices = vertices;
      Faces = faces;
   }

   public string Name;
   public List<Vector3> Vertices;
   public List<MeshFace> Faces;
}