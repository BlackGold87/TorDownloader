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
using Microsoft.Win32;
using Ragnar;

namespace NewTor
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Libtor.Service.Session Session = new Libtor.Service.Session();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnOpen.TouchDown += btnOpen_TouchDown;
            this.btnOpen.Click += BtnOpen_Click;
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

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            this.HandleBtnOpen();
        }

        private void btnOpen_TouchDown(object sender, TouchEventArgs e)
        {
            this.HandleBtnOpen();
        }

        private void HandleBtnOpen()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Torrents (*.torrent)|*.torrent",
                Title = "Select a .torrent file to open",
                Multiselect = false,
            };
            var d = dialog.ShowDialog(this);
            if (d.HasValue && d == true)
            {
                var par = new AddTorrentParams { SavePath = "C:/Downloads", TorrentInfo = new TorrentInfo(dialog.FileName) };

                if (!this.Session.TorrentExist(par))
                {
                    this.Session.AddTorrent(par);
                    using (var db = new LiteDatabase("Test.db"))
                    {
                        var fadded = db.FileStorage.Upload(Guid.NewGuid().ToString(), dialog.FileName);
                        var cools = db.GetCollection<Torrent>("Torrents");
                        cools.Insert(new Torrent { Name = par.TorrentInfo.Name, UTC_CreationDate = DateTime.UtcNow, File = fadded.Id });
                    }
                }


            }

        }
    }
}
