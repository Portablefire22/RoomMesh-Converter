using System.IO;
using System.Text;

namespace RMeshConverter.Exporter;

public abstract class DmxExporter : MeshExporter
{
    protected readonly int Version = 9;
    protected readonly string Format = "model";
    protected readonly int FormatVersion = 22;

    protected readonly string Root;

    protected string[] StringTable;
    
    protected DmxExporter(string inputFilePath, string name, string outputDirectory, string root) : base(inputFilePath, name, outputDirectory)
    {
        Root = root;
        OutputFileStream = File.Create($"{OutputDirectory}\\{Name}.dmx");
    }

    protected void WriteString(string str)
    {
        OutputFileStream.Write(Encoding.ASCII.GetBytes(str));
    }

    protected void WriteNullTerminatedString(string str)
    {
        WriteString(str + '\0');
    }

    protected void WriteInt(int num)
    {
        OutputFileStream.Write(BitConverter.GetBytes(num));
    }

    protected void WriteStringTable(string[] strings)
    {
        WriteInt(strings.Length);
        foreach (var str in strings)
        {
            WriteNullTerminatedString(str);
        }
    }

    protected void WriteTime(float time)
    {
        WriteInt((int)time * 10000);
    }

    protected void WriteColor(int[] rgba)
    {
        if (rgba.Length != 4) throw new DmxException("Given RGBA does not contain four members");
        var chr = rgba.Select(x => (char)x).ToArray();
        OutputFileStream.Write(Encoding.ASCII.GetBytes(chr));
    }

    protected void WriteCharArray(char[] arr)
    {
        WriteInt(arr.Length);
        foreach (var chr in arr)
        {
            OutputFileStream.Write(BitConverter.GetBytes(chr));
        }
    }

    protected void WriteChar(char chr)
    {
        OutputFileStream.Write(BitConverter.GetBytes(chr));
    }

    protected void WriteAttribute(DmxAttribute element)
    {
        WriteStringIndex(element.Name);
        WriteChar(element.Type);
        OutputFileStream.Write(BitConverter.GetBytes(element.Value));
    } 
    
    protected void WriteElement(DmxElement element)
    {
        WriteStringIndex(element.Type);
        WriteStringIndex(element.Name);
        WriteInt(element.Attributes.Length);
        foreach (var attribute in element.Attributes)
        {
            WriteAttribute(attribute);
        }
    }

    protected void WriteElements(DmxElement[] elements)
    {
        WriteInt(elements.Length);
        foreach (var element in elements)
        {
            WriteElement(element);
        }
    }

    /// <summary>
    /// Gets the StringTable index of the given string and writes its index as an int32. Required since DMX files
    /// do not write strings after the StringTable initialisation, and instead write the index to prevent duplication.
    /// </summary>
    /// <param name="str"></param>
    /// <exception cref="DmxException"></exception>
    protected void WriteStringIndex(string str)
    {
        var index = Array.IndexOf(StringTable, str);
        if (index == -1) throw new DmxException($"string '{str}' could not be found in StringTable");
        WriteInt(index);
    }
    
    protected void WriteHeader()
    {
        WriteString($"<!-- dmx encoding binary {Version} format {Format} {FormatVersion} -->\n");
        WriteInt(0); // Files im looking at seem to have a 0 at the start
    }
}