using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Service
{
    public abstract class AppService:IAppService
    {
        protected AppService()
        {
            
        }

        public abstract string ThisKey { get; }

        public abstract string ThisName { get; }

        public abstract void UpdateDevice(int devid, object obj);

        public abstract void RemoveDevice(int devid);

        public abstract void StartService();

        public abstract void StopService();

        public event AppServiceLogHandler AppServiceLog;

        protected void OnAppServiceLog(string log)
        {
            if (AppServiceLog != null)
            {
                AppServiceLog(log);
            }
        }

        public abstract bool IsDisposed { get;}

        public abstract void Dispose();
    }
}
