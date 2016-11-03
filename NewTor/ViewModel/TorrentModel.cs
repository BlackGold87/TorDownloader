using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libtor.Model;
using NewTor.CustomComponent;
using NewTor.Datasource;
using Ragnar;
using Session = Libtor.Service.Session;

namespace NewTor.ViewModel
{
    public class TorrentModel : ObservableObject, IDisposable
    {
        private Session _Session;

        public Session Session
        {
            get { return this._Session; }
            set
            {
                if (value != _Session)
                {
                    _Session = value;
                    OnPropertyChanged("Session");
                }
            }
        }

        private IList<TorrentStatus> _Status;

        public IList<TorrentStatus> Status
        {
            get { return this._Status; }
            set
            {
                if (value != this._Status)
                {
                    this._Status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public TorrentModel()
        {
            Session = new Session();
            using (var db = new TorLiteDatabase())
            {
                var colls = db.GetCollection<SessionState>("SessionState");
                var state = colls.FindAll().FirstOrDefault();
                if (state != null)
                {
                    var ms = new MemoryStream();
                    db.FileStorage.Download(state.State, ms);
                    ms.Position = 0;
                    Session.Start(ms.GetBuffer());
                }
                else Session.Start();
            }

            this.Session.OnAlert += Session_OnAlert;
            this.Session.OnTorrentAddedAlert += Session_OnTorrentAddedAlert;
            this.Session.OnTorrentCheckedAlert += Session_OnTorrentCheckedAlert;
            this.Session.OnTorrentResumedAlert += Session_OnTorrentResumedAlert;
            this.Session.OnStateChangedAlert += Session_OnStateChangedAlert;
            this.Session.OnStateUpdateAlert += Session_OnStateUpdateAlert;
            this.Session.OnSaveResumeDataAlert += Session_OnSaveResumeDataAlert; ;
            this.Session.OnStatsAlert += Session_OnStatsAlert;

            using (var db = new TorLiteDatabase())
            {

                var cools = db.GetCollection<Torrent>("Torrents");
                foreach (Torrent torrent in cools.FindAll())
                {
                    MemoryStream ms = new MemoryStream();
                    MemoryStream msInfo = new MemoryStream();

                    db.FileStorage.Download(torrent.File, msInfo);
                    msInfo.Position = 0;

                    db.FileStorage.Download(torrent.ResumeData, ms);
                    ms.Position = 0;

                    var par = new AddTorrentParams { ResumeData = ms.GetBuffer(), TorrentInfo = new TorrentInfo(msInfo.GetBuffer()) };
                    this.Session.AddTorrent(par);
                }
            }
        }

        private void Session_OnSaveResumeDataAlert(object sender, SaveResumeDataAlert e)
        {
            using (var db = new TorLiteDatabase())
            {
                var cools = db.GetCollection<Torrent>("Torrents");
                var tor = cools.FindOne(x => x.Hash.Equals(e.Handle.TorrentFile.InfoHash));
                if (tor != null)
                {
                    MemoryStream ms = new MemoryStream(e.ResumeData);
                    ms.Position = 0;
                    var fadded = db.FileStorage.Upload(Guid.NewGuid().ToString(), ms);
                    if (!string.IsNullOrWhiteSpace(tor.ResumeData)) db.FileStorage.Delete(tor.ResumeData);
                    tor.ResumeData = fadded.Id;
                    cools.Update(tor);
                }
            }
        }

        private void Session_OnStatsAlert(object sender, StatsAlert e)
        {
            if (e.Handle.NeedSaveResumeData()) e.Handle.SaveResumeData();
        }

        private void Session_OnStateUpdateAlert(object sender, StateUpdateAlert e)
        {
            this.Status = e.Statuses;
        }

        private void Session_OnStateChangedAlert(object sender, StateChangedAlert e)
        {
        }

        private void Session_OnTorrentResumedAlert(object sender, TorrentResumedAlert e)
        {
        }

        private void Session_OnTorrentCheckedAlert(object sender, TorrentCheckedAlert e)
        {
        }

        private void Session_OnTorrentAddedAlert(object sender, TorrentAddedAlert e)
        {
        }

        private void Session_OnAlert(object sender, Alert e)
        {

        }

        public void Dispose()
        {
            var state = Session.SaveState();
            if (state != null && state.Length > 0)
            {
                using (var db = new TorLiteDatabase())
                {
                    var cools = db.GetCollection<SessionState>("SessionState");
                    var Sstate = cools.FindAll().FirstOrDefault();
                    var ms = new MemoryStream(state);
                    ms.Position = 0;
                    var fadded = db.FileStorage.Upload(Guid.NewGuid().ToString(), ms);
                    if (Sstate != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Sstate.State)) db.FileStorage.Delete(Sstate.State);
                        Sstate.State = fadded.Id;
                        cools.Update(Sstate);
                    }
                    else cools.Insert(new SessionState { UTC_CreationDate = DateTime.UtcNow, State = fadded.Id });
                }
            }

            Session.Dispose();
        }
    }
}
