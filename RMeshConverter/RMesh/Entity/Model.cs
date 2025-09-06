using System.Numerics;

namespace RMeshConverter.RMesh.Entity;

public class Model : Entity
{
   public string Name { get; set; }
   public Vector3 Rotation { get; set; } 
   public Vector3 Scale { get; set; }

   public Model(string name, Vector3 position, Vector3 rotation, Vector3 scale) : base(position)
   {
      Name = name;
      Rotation = rotation;
      Scale = scale;
   }
}