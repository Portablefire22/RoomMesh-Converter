using Microsoft.Extensions.Logging;
using RMeshConverter.Exporter;

namespace RMeshConverter.XModel;

public class XMtlWriter : MtlWriter, IDisposable
{
    private List<Material> Materials;
    public XMtlWriter(List<Material> materials, string path, string name, string originalDirectory) : base(ConvertToTextureLocations(materials), path, name, originalDirectory)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<XMtlWriter>();
        Materials = materials;
    }

    public override void Convert()
    {
        WriteHeader();
        foreach (var material in Materials)
        {
            WriteTexture(material);
            if (material.TextureFile != null) try{CopyTexture(material.TextureFile);} catch {}
        }
    }

    private static List<String> ConvertToTextureLocations(List<Material> materials)
    {
        return materials.Select(material => material.TextureFile).OfType<string>().ToList();
    }
    public void Dispose()
    {
        OutputFileStream.Dispose();
        GC.SuppressFinalize(this);
    }
}