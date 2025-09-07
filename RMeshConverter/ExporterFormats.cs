using System.Collections.ObjectModel;
using System.Windows;

namespace RMeshConverter;

public class ExporterFormats : ObservableCollection<string>
{
    public ExporterFormats()
    {
        Add("WaveFront Obj");
        Add("Valve Vmdl");
    }
}