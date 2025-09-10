using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using RMeshConverter.Exporter;
using RMeshConverter.Exporter.Obj;
using RMeshConverter.Exporter.Valve;
using RMeshConverter.RMesh;
using RMeshConverter.RMesh.Entity;
using RMeshConverter.XModel;

namespace RMeshConverter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ILogger _logger;

    private bool _isOutputSelected;
    private bool _isInputSelected;
    private bool _isFormatSelected;

    private string _selectedFormat;

    private bool _folderPer;
    
    public MainWindow()
    {
        InitializeComponent();
        
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<MainWindow>();
    }

    
    public void GetFolder(object sender, RoutedEventArgs routedEventArgs)
    {
        var fileDialog = new OpenFolderDialog();
        var result = fileDialog.ShowDialog();

        var txt = (TextBox?) FindName("FilePickerTextBox");

        if (txt == null)
        {
            throw new Exception();
        }
        
        switch (result)
        {
            case true:
                txt.Text = fileDialog.FolderName;
                Config.InputFolder = fileDialog.FolderName;
                Config.Files = Directory.GetFiles(fileDialog.FolderName, "*.rmesh", SearchOption.AllDirectories);
                Config.ModelFiles = Directory.GetFiles(fileDialog.FolderName, "*.x", SearchOption.AllDirectories);
                var counter = (Label)FindName("FileCounter");
                counter.Content = $".rmesh Files: {Config.Files.Length}";
                _isInputSelected = true;
                break;    
            default:
                _isInputSelected = false;
                break; 
        }
        TryEnableConvert();
    }

    public void SetOutputFolder(object sender, RoutedEventArgs routedEventArgs)
    {   
        var fileDialog = new OpenFolderDialog();
        var result = fileDialog.ShowDialog();
        
        var txt = (TextBox?) FindName("OutputFilePickerTextBox");
        
        if (txt == null)
        {
            throw new Exception();
        }
        
        switch (result)
        {
            case true:
                txt.Text = fileDialog.FolderName;
                Config.OutputFolder = fileDialog.FolderName;
                _isOutputSelected = true;
                break;    
            default:
                _isOutputSelected = false;
                break; 
        }
        TryEnableConvert();
    }

    private void TryEnableConvert()
    {
        var button = (Button?)FindName("ConvertButton");
        if (button == null) return;
        button.IsEnabled = _isInputSelected && _isFormatSelected && _isOutputSelected;
    }
    
    public void Convert(object sender, RoutedEventArgs routedEventArgs)
    {
        Parallel.ForEach(Config.Files, file =>
        {
            try
            {
                var name = file.Split("\\").Last().Replace(".rmesh", "");
                using var reader = new RoomMeshReader(file);
                reader.Read();
                
                using var writer = GetExporter(_selectedFormat, name, file, reader);
                writer.Convert();
            }
            catch (Exception e)
            {
                _logger.LogCritical("{}", e); 
            }
        });

        Parallel.ForEach(Config.ModelFiles, file =>
        {
            try
            {
                var name = file.Split("\\").Last().Replace(".x", "");
                using var conv = new XAsciiReader(file);
                conv.Convert();
                using var xpr = new XExporter(conv, file, name, $"{Config.OutputFolder}\\Models");
                xpr.Convert();
            }
            catch (Exception e)
            {
                _logger.LogCritical("{}", e); 
            }
        });
        
        GC.Collect();
        _logger.LogInformation("Finished Converting.");
    }

    private Exporter.MeshExporter GetExporter(string exporter, string name, string file, RoomMeshReader reader)
    {
        Exporter.MeshExporter exp;
        string outputFolder = Config.OutputFolder;
        if (_folderPer)
        {
            outputFolder += $"\\{name}";
        }
        
        switch (exporter)
        {
            case "WaveFront Obj":
                exp = new ObjRoomMeshExporter(name, outputFolder, file, reader);
                break;
            case "FBX (Binary)":
                exp = new FbxRoomMeshExporter(reader, file, name, outputFolder);
                break;
            case "S&Box Vmdl (Obj)":
                exp = new VmdlRoomMeshExporter(reader, file, name, outputFolder);
                break;
            case "S&Box Vmdl (FBX Binary)":
                // exp = new VmdlExporter(reader, file, relativePath, outputFolder);
                throw new NotImplementedException("S&Box FBX has not yet been implemented");
                break;
            default:
                throw new ArgumentException($"exporter by name '{exporter}' does not exist");
        }

        return exp;
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _logger.LogInformation("{}", e.AddedItems);
        _isFormatSelected = true;
        _selectedFormat = (string) e.AddedItems[0];
        switch (e.AddedItems[0])
        {
            case "WaveFront Obj":
            case "FBX (Binary)":
            case "S&Box Vmdl (Obj)":
            case "S&Box Vmdl (FBX Binary)":
                break;
            default:
                _isFormatSelected = false;
                break;
        }
        TryEnableConvert();
    }
}