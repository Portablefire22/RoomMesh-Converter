namespace RMeshConverter.XModel;

public class MeshMaterialList
{
    public List<int> MaterialIndices;
    public List<string> MaterialName;
    public bool AllMaterialsSame;

    public MeshMaterialList(List<int> materialIndices, List<string> materialName, bool allMaterialsSame)
    {
        MaterialIndices = materialIndices;
        MaterialName = materialName;
        AllMaterialsSame = allMaterialsSame;
    }
}