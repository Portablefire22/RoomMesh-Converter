using System.Numerics;

namespace RMeshConverter.RMesh.Entity;

public class SoundEmitter : Entity
{
    public int SoundIndex { get; set; }
    public float Range { get; set; }

    public SoundEmitter(Vector3 position,int soundIndex, float range) : base(position)
    {
        SoundIndex = soundIndex;
        Range = range;
    }
}