using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MusicSync.Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SettingsModel settings;

        public MainWindow()
        {
            settings = new SettingsModel();
            settings.Load();
            this.DataContext = settings;
            
            InitializeComponent();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            settings.Save();
            Application.Current.Shutdown(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Choose an iTunes Library";
            dlg.FileName = "iTunes Music Library.xml"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "iTunes XML Library (.xml)|*.xml"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                settings.iTunesLibraryPath = dlg.FileName;
            }

        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.Current.Shutdown(0);
        }
    }
}
