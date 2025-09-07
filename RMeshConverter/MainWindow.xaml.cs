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
                Config._files = Directory.GetFiles(fileDialog.FolderName, "*.rmesh", SearchOption.AllDirectories);
                var counter = (Label)FindName("FileCounter");
                counter.Content = $".rmesh Files: {Config._files.Length}";
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
        Parallel.ForEach(Config._files, file =>
        {
            try
            {
                var relativePath = file.Split("\\").Last().Replace(".rmesh", "");
                // var relativePath = file.Replace(".rmesh", ".obj").Replace(Config.InputFolder, "");
                using var reader = new RoomMeshReader(file);
                reader.Read();
                using var writer = GetExporter(_selectedFormat, relativePath, file, reader);
                writer.Convert();
            }
            catch (Exception e)
            {
            
            }
        });
        GC.Collect();
        _logger.LogInformation("Finished Converting.");
    }

    private Exporter.Exporter GetExporter(string exporter, string relativePath, string file, RoomMeshReader reader)
    {
        Exporter.Exporter exp;
        string outputFolder = Config.OutputFolder;
        if (_folderPer)
        {
            outputFolder += $"\\{relativePath}";
        }
        
        switch (exporter)
        {
            case "WaveFront Obj":
                exp = new ObjExporter(relativePath, outputFolder, file, reader);
                break;
            case "Valve Vmdl":
                exp = new VmdlExporter(reader, file, relativePath, outputFolder);
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
                break;
            case "Valve Vmdl":
                break;
            default:
                _isFormatSelected = false;
                break;
        }
        TryEnableConvert();
    }
}