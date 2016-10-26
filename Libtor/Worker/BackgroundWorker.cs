using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libtor.Worker
{
    public class BackgroundWorker : System.ComponentModel.BackgroundWorker
    {
        private System.ComponentModel.DoWorkEventHandler _DoWorkEventHandler = null;
        public System.Threading.Thread _backgroundWorkerThread = null;
        private object _Argument = null;
        public object Argument { set { this._Argument = value; } get { return this._Argument; } }

        public BackgroundWorker(System.ComponentModel.DoWorkEventHandler __DoWorkEventHandler, bool _WorkerSupportsCancellation = true, bool _AutoRun = false, object __Argument = null)
        {
            this.WorkerSupportsCancellation = _WorkerSupportsCancellation;
            this._DoWorkEventHandler = __DoWorkEventHandler;
            this.DoWork += _DoWorkEventHandler;
            this.Argument = __Argument;
            if (_AutoRun) this.Run();
        }

        public void Run()
        {
            if (!this.IsBusy)
                this.RunWorkerAsync(this.Argument);
        }

        public void Cancel()
        {
            if (this.WorkerSupportsCancellation)
                this.CancelAsync();
        }

        public void Abort()
        {
            if (_backgroundWorkerThread != null) _backgroundWorkerThread.Abort();
        }

        public new void Dispose()
        {
            this.Cancel();
            this.DoWork -= _DoWorkEventHandler;
            this._DoWorkEventHandler = null;
            this.Argument = null;
            base.Dispose();
        }
    }

}
