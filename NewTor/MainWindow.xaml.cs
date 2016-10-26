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
using Ragnar;

namespace NewTor
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Libtor.Service.Session Session = null;

        public Libtor.Service.Session TorrentSession => Session;

        private static Frame _masterFrame = null;
        private bool _isNavigatingBack = false;
        public MainWindow()
        {
            InitializeComponent();

            _masterFrame = MasterFrame;
            _masterFrame.Navigated += _masterFrame_Navigated;
            _masterFrame.Navigating += _masterFrame_Navigating;
            this.Loaded += MainWindow_Loaded;
        }

        private void _masterFrame_Navigating(object sender, NavigatingCancelEventArgs e) => this._isNavigatingBack = (e.NavigationMode == NavigationMode.Back);

        private void _masterFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (_isNavigatingBack)
            {

            }
        }

        public static void Navigate(object page, object extraData = null) => _masterFrame?.Navigate(page, extraData);

        internal static void GoBack() => _masterFrame?.GoBack();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Session = new Libtor.Service.Session();
            this.Session.Start();
            this.Session.OnAlert += Session_OnAlert;
            this.Session.OnTorrentAddedAlert += Session_OnTorrentAddedAlert;
            this.Session.OnTorrentCheckedAlert += Session_OnTorrentCheckedAlert;
            this.Session.OnTorrentResumedAlert += Session_OnTorrentResumedAlert;
            this.Session.OnStateChangedAlert += Session_OnStateChangedAlert;
            this.Session.OnStateUpdateAlert += Session_OnStateUpdateAlert;
            this.Session.OnStatsAlert += Session_OnStatsAlert;
            using (var db = new LiteDB.LiteDatabase("Test.db"))
            {

                var cools = db.GetCollection<Torrent>("Torrents");
                foreach (Torrent torrent in cools.FindAll())
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Position = 0;
                    db.FileStorage.Download(torrent.File, ms);
                    ms.Position = 0;
                    var par = new AddTorrentParams { SavePath = "C:/Downloads", TorrentInfo = new TorrentInfo(ms.GetBuffer()) };
                    this.Session.AddTorrent(par);
                }
            }
            this.MasterFrame.Navigate(new TorrentPage(this));
        }

        private void Session_OnStatsAlert(object sender, Ragnar.StatsAlert e)
        {
        }

        private void Session_OnStateUpdateAlert(object sender, Ragnar.StateUpdateAlert e)
        {
        }

        private void Session_OnStateChangedAlert(object sender, Ragnar.StateChangedAlert e)
        {
        }

        private void Session_OnTorrentResumedAlert(object sender, Ragnar.TorrentResumedAlert e)
        {
        }

        private void Session_OnTorrentCheckedAlert(object sender, Ragnar.TorrentCheckedAlert e)
        {
        }

        private void Session_OnTorrentAddedAlert(object sender, Ragnar.TorrentAddedAlert e)
        {
        }

        private void Session_OnAlert(object sender, Ragnar.Alert e)
        {

        }
    }
}
