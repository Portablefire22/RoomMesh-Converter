namespace RMeshConverter.Exporter.Valve.Json;

public class Root
{
    public GameObject RootObject{get;set;}
    public int ResourceVersion { get; set; } = 2;
    public bool ShowInMenu{get;set;}
    public string? MenuPath { get; set; } = null;
    public string? MenuIcon { get; set; } = null;
    public bool DontBreakAsTemplate{get;set;}
    public string[] __references {get;set;} = new string[] { };
    public int __version { get; set; } = 2;

    public Root(GameObject rootObject)
    {
        RootObject = rootObject;
    }
}