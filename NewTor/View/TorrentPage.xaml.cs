using System;
using System.Collections.Generic;
using System.Globalization;
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
using MahApps.Metro.Controls.Dialogs;
using NewTor.Datasource;
using NewTor.ViewModel;

namespace NewTor.View
{
    /// <summary>
    /// Logica di interazione per TorrentPage.xaml
    /// </summary>
    public partial class TorrentPage : Page
    {
        private MainWindow ParentWindow = null;
        public TorrentPage(MainWindow _Parentwindow)
        {
            InitializeComponent();
            this.ParentWindow = _Parentwindow;
            DataContext = this.ParentWindow.TorrentModel;
        }

        private async void FilesPanel_OnDropPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    try
                    {
                        TorrentInfo info = new TorrentInfo(file);
                        if (info.IsValid)
                        {
                            var piecelengthConv = (string)new Converter.FileSizeConverter().Convert(info.PieceLength, info.PieceLength.GetType(), null, CultureInfo.CurrentCulture);
                            var totsizzeConv = (string)new Converter.FileSizeConverter().Convert(info.TotalSize, info.TotalSize.GetType(), null, CultureInfo.CurrentCulture);

                            string torrInfo = string.Format("Commento: {0}\n" +
                                                            "Data Creazione: {1}\n" +
                                                            "Creatore: {2}\n" +
                                                            "Hash: {3}\n" +
                                                            "Nome: {4}\n" +
                                                            "Files: {5}\n" +
                                                            "Parti: {6} x {7}\n" +
                                                            "Server Privato: {8}\n" +
                                                            "Certificato SSL: {9}\n" +
                                                            "Dimensione Totale: {10}", info.Comment, info.CreationDate, info.Creator, info.InfoHash, info.Name, info.NumFiles, info.NumPieces, piecelengthConv, new Converter.BoolConverter().Convert(info.Private, info.Private.GetType(), null, CultureInfo.CurrentCulture), info.SslCert, totsizzeConv);

                            var data = await this.ParentWindow.ShowMessageAsync("Aggiungi Nuovo torrent", torrInfo, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AnimateHide = true, AnimateShow = true });
                            if (data.Equals(MessageDialogResult.Affirmative)) HandleAddTorrent(info, file);
                        }
                    }
                    catch { }
                }

            }
        }

        private void HandleAddTorrent(TorrentInfo info, string filePath)
        {
            var par = new AddTorrentParams { SavePath = "C:/Downloads", TorrentInfo = info };

            if (!this.ParentWindow.TorrentModel.Session.TorrentExist(par))
            {
                this.ParentWindow.TorrentModel.Session.AddTorrent(par);
                using (var db = new TorLiteDatabase())
                {
                    var fadded = db.FileStorage.Upload(Guid.NewGuid().ToString(), filePath);
                    var cools = db.GetCollection<Torrent>("Torrents");
                    cools.Insert(new Torrent { Name = par.TorrentInfo.Name, UTC_CreationDate = DateTime.UtcNow, File = fadded.Id, Hash = par.TorrentInfo.InfoHash, Path = par.SavePath });
                }
            }
        }

        private void FilesPanel_OnPreviewDragOver(object sender, DragEventArgs e) => e.Handled = true;


        private void HandleBtnOpen()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Torrents (*.torrent)|*.torrent",
                Title = "Select a .torrent file to open",
                Multiselect = false,
            };
            var d = dialog.ShowDialog(this.ParentWindow);
            if (d.HasValue && d == true)
            {
                try
                {
                    var info = new TorrentInfo(dialog.FileName);
                    if (info.IsValid)
                        HandleAddTorrent(info, dialog.FileName);
                }
                catch { }
            }

        }
    }
}
