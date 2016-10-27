using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Libtor.Model;
using LiteDB;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using NewTor.View;
using Ragnar;

namespace NewTor
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private static Frame _masterFrame = null;
        private bool _isNavigatingBack = false;
        public MainWindow()
        {
            InitializeComponent();

            _masterFrame = MasterFrame;
            _masterFrame.Navigated += _masterFrame_Navigated;
            _masterFrame.Navigating += _masterFrame_Navigating;
            this.Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) => this.TorrentModel.Dispose();

        private void _masterFrame_Navigating(object sender, NavigatingCancelEventArgs e) => this._isNavigatingBack = (e.NavigationMode == NavigationMode.Back);

        private void _masterFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (_isNavigatingBack)
            {

            }
        }

        public static void Navigate(object page, object extraData = null) => _masterFrame?.Navigate(page, extraData);

        internal static void GoBack() => _masterFrame?.GoBack();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) => this.MasterFrame.Navigate(new TorrentPage(this));

    }
}
