using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Device;
using ServerSuperIO.Protocol;

namespace TestDeviceDriver
{
    public class DeviceSingletonDriver:RunDevice
    {
        private DeviceDyn _deviceDyn;
        private DevicePara _devicePara;
        private DeviceProtocol _protocol;
        public DeviceSingletonDriver()
            : base()
        {
            _devicePara = new DevicePara();
            _deviceDyn = new DeviceDyn();
            _protocol=new DeviceProtocol();
        }

        public override void Initialize(int devid)
        {
            this.Protocol.InitDriver(this);

            //初始化设备参数信息
            
            _devicePara.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的参数文件。
            if (System.IO.File.Exists(_devicePara.SavePath))
            {
                //如果参数文件存在，则获得参数实例
                _devicePara = _devicePara.Load<DevicePara>();
            }
            else
            {
                //如果参数文件不存在，则序列化一个文件
                _devicePara.Save<DevicePara>(_devicePara);
            }

            //初始化设备实时数据信息
            _deviceDyn.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的实时数据文件。
            if (System.IO.File.Exists(_deviceDyn.SavePath))
            {
                //如果参数文件存在，则获得参数实例
                _deviceDyn = _deviceDyn.Load<DeviceDyn>();
            }
            else
            {
                //如果参数文件不存在，则序列化一个文件
                _deviceDyn.Save<DeviceDyn>(_deviceDyn);
            }
        }

        public override byte[] GetConstantCommand()
        {
            return this.Protocol.DriverPackage(0, "61", null);
        }

        public override void Communicate(ServerSuperIO.Communicate.IRequestInfo info)
        {
            object obj = this.Protocol.DriverAnalysis("61", info.Data, null);
            if (obj != null)
            {
                Dyn dyn = (Dyn) obj;
                _deviceDyn.Dyn = dyn;
            }

            ((ISocketSession)info.Channel).TrySend(new byte[] {0x00, 0x01, 0x03, 0x04, 0x05},true);

            OnDeviceRuningLog("通讯正常");
        }

        public override void CommunicateInterrupt(ServerSuperIO.Communicate.IRequestInfo info)
        {
            OnDeviceRuningLog("通讯中断");
        }

        public override void CommunicateError(ServerSuperIO.Communicate.IRequestInfo info)
        {
            OnDeviceRuningLog("通讯干扰");
        }

        public override void CommunicateNone()
        {
            OnDeviceRuningLog("通讯未知");
        }

        public override void Alert()
        {
            return;
        }

        public override void Save()
        {
            try
            {
                _deviceDyn.Save<DeviceDyn>(_deviceDyn);
            }
            catch (Exception ex)
            {
                OnDeviceRuningLog(ex.Message);
            }
        }

        public override void Show()
        {
            List<string> list=new List<string>();
            list.Add(_devicePara.DeviceName);
            list.Add(_deviceDyn.Dyn.Flow.ToString());
            list.Add(_deviceDyn.Dyn.Signal.ToString());
            OnDeviceObjectChanged(list.ToArray());
        }

        public override void UnknownIO()
        {
            OnDeviceRuningLog("未知通讯接口");
        }

        public override void CommunicateStateChanged(ServerSuperIO.Communicate.CommunicateState comState)
        {
            OnDeviceRuningLog("通讯状态改变");
        }

        public override void ChannelStateChanged(ServerSuperIO.Communicate.ChannelState channelState)
        {
            OnDeviceRuningLog("通道状态改变");
        }

        public override void Exit()
        {
            OnDeviceRuningLog("退出设备");
        }

        public override void Delete()
        {
            OnDeviceRuningLog("删除设备");
        }

        public override object GetObject()
        {
            throw new NotImplementedException();
        }

        public override void ShowContextMenu()
        {
            throw new NotImplementedException();
        }

        public override IDeviceDynamic DeviceDynamic
        {
            get { return _deviceDyn; }
        }

        public override IDeviceParameter DeviceParameter
        {
            get { return _devicePara; }
        }

        public override IProtocolDriver Protocol { get; }

        public override DeviceType DeviceType
        {
            get { return DeviceType.Common; }
        }

        public override string ModelNumber
        {
            get { return "serversuperio"; }
        }

        public override System.Windows.Forms.Control DeviceGraphics
        {
            get { throw new NotImplementedException(); }
        }
    }
}
