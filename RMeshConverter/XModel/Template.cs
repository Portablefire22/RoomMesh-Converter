namespace RMeshConverter.XModel;

public class Template
{
    public string Name;
    public Dictionary<string, dynamic> Properties;

    public Template(string name, Dictionary<string, dynamic> properties)
    {
        Name = name;
        Properties = properties;
    }
}