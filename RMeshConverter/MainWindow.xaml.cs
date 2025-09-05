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
using Microsoft.Win32;
using RMeshConverter.RMesh;

namespace RMeshConverter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
                Config._files = Directory.GetFiles(fileDialog.FolderName, "*.rmesh", SearchOption.AllDirectories);
                var counter = (Label)FindName("FileCounter");
                counter.Content = $".rmesh Files: {Config._files.Length}";
                break;    
            default:
                break; 
        }
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
                break;    
            default:
                break; 
        }
    }

    public void Convert(object sender, RoutedEventArgs routedEventArgs)
    {
        var conv = new CbConverter(Config._files[6]);
 
        Console.WriteLine(conv.ReadB3DString());
    }
}