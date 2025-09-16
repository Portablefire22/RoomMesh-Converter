namespace RMeshConverter.Exporter.Valve.Json;

public class RenderOptions
{
    public bool GameLayer { get; set; } = true;
    public bool OverlayLayer{get;set;}
    public bool BloomLayer{get;set;}
    public bool AfterUILayer{get;set;}

    public RenderOptions()
    {
        GameLayer = true;
        OverlayLayer = false;
        BloomLayer = false;
        AfterUILayer = false;
    }
    
    public RenderOptions(bool gameLayer, bool overlayLayer, bool bloomLayer, bool afterUiLayer)
    {
        GameLayer = gameLayer;
        OverlayLayer = overlayLayer;
        BloomLayer = bloomLayer;
        AfterUILayer = afterUiLayer;
    }
}