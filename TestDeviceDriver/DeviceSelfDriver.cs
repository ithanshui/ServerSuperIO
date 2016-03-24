using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDeviceDriver
{
    public class DeviceSelfDriver:DeviceDriver
    {
        public DeviceSelfDriver() : base()
        {
            
        }

        public override void Initialize(int devid)
        {
            this.RunTimerInterval = 2000;
            this.IsRunTimer = true;
            base.Initialize(devid);
        }

        public override void OnRunTimer()
        {
            byte[] data = this.GetConstantCommand();
            OnSendData(data);
            base.OnRunTimer();
        }
    }
}
