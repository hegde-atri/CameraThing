using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPFMediaKit.DirectShow.Controls;

namespace CameraThing;

public partial class ToolbarWindow : Window
{
    private readonly MainWindow _mainWindow;

    public ToolbarWindow(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow; // Handle window closing to shut down the entire application
        Closing += ToolbarWindow_Closing; // Populate camera sources
        if (MultimediaUtil.VideoInputDevices.Length > 0)
        {
            // Create camera display items with tooltips for full names
            var cameraItems = MultimediaUtil.VideoInputNames.Select(name => new
            {
                DisplayName = TrimCameraName(name),
                FullName = name,
                ToolTip = name.Length > 30 ? name : null
            }).ToArray();

            cobVideoSource.ItemsSource = cameraItems;
            cobVideoSource.DisplayMemberPath = "DisplayName";
            cobVideoSource.SelectedIndex = 0;
            _mainWindow.SetCameraDevice(MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex]);
        }
    }

    private void cobVideoSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cobVideoSource.SelectedIndex < 0)
        {
            // Stop any current camera capture
            _mainWindow.SetCameraDevice(null);
            return;
        }

        // Set the selected camera device
        _mainWindow.SetCameraDevice(MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex]);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // Close the application by closing the main window
        _mainWindow.Close();
    }

    private void ToolbarWindow_Closing(object sender, CancelEventArgs e)
    {
        // When toolbar window is closing (by any means), shut down the entire application
        Application.Current.Shutdown();
    }

    private static string TrimCameraName(string cameraName)
    {
        if (string.IsNullOrEmpty(cameraName))
            return cameraName;

        const int maxLength = 30;
        if (cameraName.Length <= maxLength)
            return cameraName;

        return cameraName.Substring(0, maxLength - 3) + "...";
    }
}