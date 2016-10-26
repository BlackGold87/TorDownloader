﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ragnar;

namespace Libtor.Service
{
    public class Session
    {
        private readonly Ragnar.Session _session = null;
        private readonly Worker.BackgroundWorker workerAlert = null;

        public event EventHandler<TorrentAddedAlert> OnTorrentAddedAlert;
        public event EventHandler<TorrentResumedAlert> OnTorrentResumedAlert;
        public event EventHandler<TorrentCheckedAlert> OnTorrentCheckedAlert;
        public event EventHandler<StateUpdateAlert> OnStateUpdateAlert;
        public event EventHandler<StateChangedAlert> OnStateChangedAlert;
        public event EventHandler<StatsAlert> OnStatsAlert;
        public event EventHandler<Alert> OnAlert;

        public Session()
        {
            this._session = new Ragnar.Session(new Fingerprint("TOR", 0, 0, 1, 0));
            this.workerAlert = new Worker.BackgroundWorker(_alertEventHandler, true);
        }

        public void Start()
        {
            _session.ListenOn(6881, 6889);
            this._session.SetAlertMask(SessionAlertCategory.All);
            this.workerAlert.Run();
        }

        public void Stop()
        {
            this._session.Pause();
            this.workerAlert.Cancel();
        }
        private void _alertEventHandler(object sender, DoWorkEventArgs e)
        {
            var timeout = TimeSpan.FromSeconds(0.5);
            var lastPost = DateTime.Now;
            var worker = sender as Worker.BackgroundWorker;
            while (!worker.CancellationPending)
            {
                if ((DateTime.Now - lastPost).TotalSeconds > 1)
                {
                    _session.PostTorrentUpdates();

                    lastPost = DateTime.Now;
                }

                var foundAlerts = _session.Alerts.PeekWait(timeout);
                if (!foundAlerts) continue;

                var alerts = _session.Alerts.PopAll();

                foreach (var alert in alerts)
                {
                    Console.WriteLine(alert.GetType().ToString() + " :" + alert.Message);
                    if (alert is TorrentAddedAlert) OnTorrentAddedAlert?.Invoke(this, alert as TorrentAddedAlert);
                    else if (alert is StateUpdateAlert) OnStateUpdateAlert?.Invoke(this, alert as StateUpdateAlert);
                    else if (alert is StateChangedAlert) OnStateChangedAlert?.Invoke(this, alert as StateChangedAlert);
                    else if (alert is TorrentResumedAlert) OnTorrentResumedAlert?.Invoke(this, alert as TorrentResumedAlert);
                    else if (alert is StatsAlert) OnStatsAlert?.Invoke(this, alert as StatsAlert);
                    else if (alert is TorrentCheckedAlert) OnTorrentCheckedAlert?.Invoke(this, alert as TorrentCheckedAlert);
                    else OnAlert?.Invoke(this, alert);
                }
            }
            e.Cancel = true;
        }

        public void AddTorrent(AddTorrentParams @params)
        {
            this._session.AsyncAddTorrent(@params);
        }

        public bool TorrentExist(AddTorrentParams @params)
        {
            return this._session.FindTorrent(@params.TorrentInfo.InfoHash) != null;
        }
    }
}
