using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using ServerSuperIO.Server;
using System.Windows.Forms;

namespace ServerSuperIO.Common
{
    public class MonitorException:ServerProvider
    {
        public MonitorException():base()
        {
            
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public void Monitor()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(MainThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        public void UnMonitor()
        {
            Application.ThreadException -= new ThreadExceptionEventHandler(MainThreadException);
            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void MainThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                this.Server.Logger.Error(true, "", e.Exception);
            }
            catch 
            {
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                this.Server.Logger.Error(true, "", (Exception)e.ExceptionObject);
            }
            catch 
            {
            }
        }
    }
}
