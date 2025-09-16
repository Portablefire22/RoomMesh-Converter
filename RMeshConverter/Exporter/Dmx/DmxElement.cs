namespace RMeshConverter.Exporter;

public class DmxElement
{
    public string Type;
    public string Name;
    public string Uuid = Guid.NewGuid().ToString();

    public DmxAttribute[] Attributes;

    public DmxElement(string type, string name, DmxAttribute[] attributes)
    {
        Type = type;
        Name = name;
        Attributes = attributes;
    }
}