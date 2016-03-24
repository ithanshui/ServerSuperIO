using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Service;

namespace TestService
{
    public class Service:AppService
    {
        private bool _isDispose = false;
        public override string ThisKey
        {
            get { return "应用服务"; }
        }

        public override string ThisName
        {
            get { return "应用服务"; }
        }

        public override void UpdateDevice(int devid, object obj)
        {
            OnAppServiceLog("更新数据");
        }

        public override void RemoveDevice(int devid)
        {
            OnAppServiceLog("删除数据");
        }

        public override void StartService()
        {
            OnAppServiceLog("启动服务");
        }

        public override void StopService()
        {
            
            OnAppServiceLog("停止服务");
        }

        public override void Dispose()
        {
            _isDispose = true;
            OnAppServiceLog("释放资源");
        }

        public override bool IsDisposed
        {
            get { return _isDispose; }
        }
    }
}
