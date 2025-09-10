using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace RMeshConverter.Exporter;

public abstract class MaterialWriter
{
   protected FileStream OutputFileStream;

   protected readonly List<string> TextureLocations;
   
   protected readonly string OriginalDirectory;
   protected readonly string Path;

   protected readonly string Name; 

   protected ILogger Logger;

   public MaterialWriter(List<string> textureLocations, string originalDirectory, string path, string name)
   {
      TextureLocations = textureLocations;
      Name = name;
      OriginalDirectory = originalDirectory;
      Path = path;
   }

   protected string GetBumpName(string nameAndExtension)
   {
      var extension = nameAndExtension[new Range(nameAndExtension.LastIndexOf('.'), ^0)];
      var name = RemoveExtension(nameAndExtension);
      return $"{name}bump{extension}";
   }
   
   protected void CopyTexture(string name)
   {
      File.Copy($"{OriginalDirectory}{name}", $"{Path}\\{name}", true);
      // Bump files dont seem to be mentioned but do exist
      try
      {
         var bumpName = GetBumpName(name);
         File.Copy($"{OriginalDirectory}{bumpName}", $"{Path}\\{bumpName}", true);
      }
      catch (Exception e)
      {
         Logger.LogCritical("{}", e);
      }
}

   protected static string RemoveExtension(string file)
   {
      return file.Remove(file.LastIndexOf('.'));
   }

   protected void WriteString(string str)
   {
      OutputFileStream.Write(Encoding.UTF8.GetBytes(str));
   }
   public abstract void Convert();
}