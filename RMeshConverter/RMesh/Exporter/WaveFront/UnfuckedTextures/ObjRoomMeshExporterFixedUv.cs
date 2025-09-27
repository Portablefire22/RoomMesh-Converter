using System.Drawing;
using System.IO;
using System.Net;
using System.Numerics;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter.Obj.UnfuckedTextures;

public class ObjRoomMeshExporterFixedUv : ObjRoomMeshExporter
{

    private string _currentTexture;
    private List<Face> _placedFaces;
    private Bitmap _image;
    private Graphics _graphics;
    private string _imagePath;
    private Vector2 _placeOffset;
    
    public ObjRoomMeshExporterFixedUv(string name, string outputDirectory, string filePath, RoomMeshReader reader) : base(name, outputDirectory, filePath, reader)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<ObjRoomMeshExporterFixedUv>();

        _imagePath = $"{filePath}\\{name}.jpg";
        _image = new Bitmap(9000, 9000);
    }

    private void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion,ref Bitmap destBitmap, Rectangle destRegion)
    {
        using (Graphics grD = Graphics.FromImage(destBitmap))            
        {
            grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);                
        }
    }

    private Rectangle BoundingBoxToNormalisedRectangle(BoundingBox box, SizeF dimension)
    {
        throw new NotImplementedException();
    }
    
    private Vector2 PlaceFace(Face face, string textureName)
    {
        var box = face.GetBoundingBox();
        var srcImage = Image.FromFile($"{InputDirectory}{textureName}");
        var rect = BoundingBoxToNormalisedRectangle(box, srcImage.PhysicalDimension);
        CopyRegionIntoImage((Bitmap) srcImage, rect, ref _image, new Rectangle(new Point(500, 500),rect.Size)); 
        _placedFaces.Add(face);
        return box.y1;
    }
    
    private void PlaceFaces()
    {
        var xOffset = 0f;
        var yOffset = 0f;
        foreach (var (textureName, faces) in Reader.TextureFaces)
        {
            foreach (var face in faces)
            {
                PlaceFace(face, textureName);
                break;
            }
            break;
        }
    }
    
    private void FixTextures()
    {
        PlaceFaces();
    }
    
    public override void Convert()
    {
        WriteHeader();
        WriteMtlLib();
        Logger.LogInformation("Wrote Geometric Vertices: {}", WriteGeometricVertices());
        // if (writeMaterial) 
        Logger.LogInformation("Wrote UV positions: {}", WriteUvs());
        Logger.LogInformation("Wrote Indices: {}", WriteVertexIndices());

        // var mtl = new RoomMeshMtlWriter($"{OutputDirectory}", Name,  InputDirectory, Reader.TexturePaths);
        // mtl.Convert();
        
        FixTextures();
        _image.Save(_imagePath);
        OutputFileStream.Close();
    }

    public override void Dispose()
    {
        _graphics.Dispose();
        _image.Dispose();
        base.Dispose();
    }
}