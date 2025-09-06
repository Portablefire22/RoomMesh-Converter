using System.IO;
using System.Text;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter;

/* https://code.blender.org/2013/08/fbx-binary-file-format-specification/ */

public class RoomMeshToFbxConverter
{
   private RoomMeshReader _reader;
   
   public RoomMeshToFbxConverter(RoomMeshReader reader)
   {
      _reader = reader;
   }
   
   public bool Convert(RoomMeshReader mesh)
   {
      return false;
   }
}