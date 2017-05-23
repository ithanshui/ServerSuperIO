using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using ServerSuperIO.CommandCache;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Log;
using ServerSuperIO.Protocol;
using ServerSuperIO.Server;

namespace ServerSuperIO.Device
{
    public abstract class RunDevice : ServerProvider,IRunDevice
    {
        private string _SaveBytesPath = AppDomain.CurrentDomain.BaseDirectory+"原始数据";
        private bool _IsRunTimer = false;
        private System.Timers.Timer _Timer = null;
        private ICommandCache _CommandCache = null;
        private readonly object _SyncLock=new object();
        private bool _IsDisposed = false; //是否释放资源

        private MonitorChannelForm _monitorChannelForm = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        protected RunDevice()
        {
            this.Tag = null;
            this.IsRunDevice = true;
            this.DevicePriority=DevicePriority.Normal;
            this._CommandCache=new CommandCache.CommandCache();

            this._Timer = new Timer(1000) {AutoReset = true};
            this._Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            this.IsRunTimer = false;
            this.RunTimerInterval = 1000;
        }

        /// <summary>
        /// 终结器
        /// </summary>
        ~RunDevice()
        {
            Dispose(false);
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="devid"></param>
        public abstract void Initialize(int devid);

        /// <summary>
        /// 保存源始数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="desc"></param>
        public void SaveBytes(byte[] data, string desc)
        {
            if (this.DeviceParameter.IsSaveOriginBytes)
            {
                if (!System.IO.Directory.Exists(_SaveBytesPath))
                {
                    System.IO.Directory.CreateDirectory(_SaveBytesPath);
                }

                string path = String.Format("{0}/{1}_{2}.txt", _SaveBytesPath, DateTime.Now.ToString("yyyy年MM月dd日"), this.DeviceParameter.DeviceID.ToString());

                string hexString = String.Format("[{0}]{1}", desc, BinaryUtil.ByteToHex(data));

                FileUtil.WriteAppend(path, hexString);
            }
        }

        /// <summary>
        /// 获得要发送的数据信息
        /// </summary>
        /// <returns></returns>
        public byte[] GetSendBytes()
        {
            byte[] data = new byte[] { };
            //如果没有命令就增加实时数据的命令
            if (this.CommandCache.Count <= 0)
            {
                data = this.GetConstantCommand();
                this.DevicePriority = DevicePriority.Normal;
            }
            else
            {
                data = this.CommandCache.Get();
                this.DevicePriority = DevicePriority.Priority;
            }
            return data;
        }

        /// <summary>
        /// 获得实时数据
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetConstantCommand();

        /// <summary>
        /// 发送数据接口
        /// </summary>
        /// <param name="io"></param>
        /// <param name="senddata"></param>
        public virtual void Send(IChannel io, byte[] senddata)
        {
            io.Write(senddata);
        }

        /// <summary>
        /// 接收数据接口
        /// </summary>
        /// <param name="io"></param>
        /// <returns></returns>
        public virtual byte[] Receive(IChannel io)
        {
            return io.Read();
        }

        /// <summary>
        /// 运行设备
        /// </summary>
        /// <param name="io"></param>
        public void Run(IChannel io)
        {
            //不运行设备
            if (!this.IsRunDevice)
            {
                OnDeviceRuningLog("设备已经停止运行");
                return;
            }

            if (io == null)
            {
                this.UnknownIO();

                if (this.DeviceDynamic.CommunicateState != CommunicateState.None)
                {
                    this.DeviceDynamic.CommunicateState = CommunicateState.None;
                    this.CommunicateStateChanged(CommunicateState.None);
                }
            }
            else
            {
                //-------------------获得发送数据命令--------------------//
                byte[] data = this.GetSendBytes();

                if (data != null && data.Length > 0)
                {
                    //-------------------发送数据----------------------------//
                    this.Send(io, data);

                    this.ShowMonitorData(data, "发送");

                    this.SaveBytes(data, "发送");
                }

                //---------------------读取数据--------------------------//
                byte[] revdata = this.Receive(io);

                this.ShowMonitorData(revdata, "接收");

                this.SaveBytes(revdata, "接收");

                //---------------------检测通讯状态----------------------//
                CommunicateState state = this.CheckCommunicateState(revdata);

                if (this.DeviceDynamic.CommunicateState != state)
                {
                    this.DeviceDynamic.CommunicateState = state;
                    this.CommunicateStateChanged(state);
                }

                IRequestInfo info = new RequestInfo()
                {
                    Key = io.Key,
                    Data = revdata,
                    Channel = null
                };

                if (state == CommunicateState.Communicate)
                {
                    this.Communicate(info);
                }
                else if (state == CommunicateState.Interrupt)
                {
                    this.CommunicateInterrupt(info);
                }
                else if (state == CommunicateState.Error)
                {
                    this.CommunicateError(info);
                }
                else
                {
                    this.CommunicateNone();
                }

                this.Alert();

                this.Save();

                this.Show();
            }
        }

        /// <summary>
        /// 运行设备
        /// </summary>
        /// <param name="key"></param>
        /// <param name="channel"></param>
        /// <param name="revData"></param>
        public void Run(string key,IChannel channel, byte[] revData)
        {
            //不运行设备
            if (!this.IsRunDevice)
            {
                OnDeviceRuningLog("设备已经停止运行");
                return;
            }

            if (revData == null)
            {
                this.UnknownIO();

                if (this.DeviceDynamic.CommunicateState != CommunicateState.None)
                {
                    this.DeviceDynamic.CommunicateState = CommunicateState.None;
                    this.CommunicateStateChanged(CommunicateState.None);
                }
            }
            else
            {
                IRequestInfo info = new RequestInfo()
                {
                    Key = key,
                    Data = revData,
                    Channel = channel
                };
                //---------------------检测通讯状态----------------------//
                CommunicateState state = this.CheckCommunicateState(revData);

                if (this.DeviceDynamic.CommunicateState != state)
                {
                    this.DeviceDynamic.CommunicateState = state;
                    this.CommunicateStateChanged(state);
                }

                if (state == CommunicateState.Communicate)
                {
                    this.Communicate(info);
                }
                else if (state == CommunicateState.Interrupt)
                {
                    this.CommunicateInterrupt(info);
                }
                else if (state == CommunicateState.Error)
                {
                    this.CommunicateError(info);
                }
                else
                {
                    this.CommunicateNone();
                }

                this.Alert();

                this.Save();

                this.Show();
            }
        }

        /// <summary>
        /// 通讯正常时调用此函数
        /// </summary>
        /// <param name="info"></param>
        public abstract void Communicate(IRequestInfo info);

        /// <summary>
        /// 通讯中断时调用此函数
        /// </summary>
        /// <param name="info"></param>
        public abstract void CommunicateInterrupt(IRequestInfo info);

        /// <summary>
        /// 通讯干扰时调用此函数
        /// </summary>
        /// <param name="info"></param>
        public abstract void CommunicateError(IRequestInfo info);

        /// <summary>
        /// 通讯未知状态，默认
        /// </summary>
        public abstract void CommunicateNone();

        /// <summary>
        /// 检测通讯状态
        /// </summary>
        /// <param name="revdata"></param>
        /// <returns></returns>
        public CommunicateState CheckCommunicateState(byte[] revdata)
        {
            CommunicateState state = CommunicateState.None;
            if (revdata.Length <= 0)
            {
                state = CommunicateState.Interrupt;
            }
            else
            {
                state = this.Protocol.CheckData(revdata) ? CommunicateState.Communicate : CommunicateState.Error;
            }
            return state;
        }

        /// <summary>
        /// 报警函数，每次调度都会调用
        /// </summary>
        public abstract void Alert();

        /// <summary>
        /// 保存数据，每次调度都会调用
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// 展示
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// 当IO为空的时候，调用此函数接口
        /// </summary>
        public abstract void UnknownIO();

        /// <summary>
        /// 当通讯状态改变的时候调用此函数
        /// </summary>
        /// <param name="comState">状态改变后的通讯状态</param>
        public abstract void CommunicateStateChanged(CommunicateState comState);

        /// <summary>
        /// 通道状态改变
        /// </summary>
        /// <param name="channelState"></param>
        public abstract void ChannelStateChanged(ChannelState channelState);

        ///// <summary>
        ///// 当有网络连接的时候调用此函数
        ///// </summary>
        ///// <param name="ip"></param>
        ///// <param name="port"></param>
        //public abstract void SocketConnect(string ip, int port);

        ///// <summary>
        ///// 当有网络连接断开的时候调用此函数
        ///// </summary>
        ///// <param name="ip"></param>
        ///// <param name="port"></param>
        //public abstract void SocketDisconnect(string ip, int port);

        /// <summary>
        /// 退出软件或平台的宿主程序的时候调用此函数
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 删除设备
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// 预留接口，当其他服务调用特定对象。
        /// </summary>
        /// <returns></returns>
        public abstract object GetObject();

        /// <summary>
        /// 是否启动设备时钟，如果为真，则调用定时执行OnRunTimer函数
        /// </summary>
        public bool IsRunTimer
        {
            set
            {
                this._IsRunTimer = value;
                if (this._IsRunTimer)  //
                {
                    this._Timer.Start();
                    this._Timer.Enabled = true;
                }
                else
                {
                    this._Timer.Stop();
                    this._Timer.Enabled = false;
                }
            }
            get
            {
                return this._IsRunTimer;
            }
        }

        /// <summary>
        /// 时钟的定时周期，决定多长时间调用一次OnRunTimer函数
        /// </summary>
        public int RunTimerInterval
        {
            set { this._Timer.Interval = value; }
            get { return (int) this._Timer.Interval; }
        }

        /// <summary>
        /// 时钟定时回调函数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            OnRunTimer();
        }

        /// <summary>
        /// 时钟定时调用的函数，可以重写此函数接口
        /// </summary>
        public virtual void OnRunTimer()
        {

        }

        /// <summary>
        /// 显示上下文菜单
        /// </summary>
        public abstract void ShowContextMenu();

        /// <summary>
        /// 显示IO监视器的窗体
        /// </summary>
        public void ShowMonitorDialog()
        {
            if (_monitorChannelForm != null)
            {
                if (_monitorChannelForm.IsDisposed)
                {
                    _monitorChannelForm = new MonitorChannelForm(this.DeviceParameter.DeviceName);
                    _monitorChannelForm.Show();
                    _monitorChannelForm.Focus();
                }
                else
                {
                    _monitorChannelForm.Show();
                    _monitorChannelForm.Focus();
                }
            }
            else
            {
                _monitorChannelForm = new MonitorChannelForm(this.DeviceParameter.DeviceName);
                _monitorChannelForm.Show();
                _monitorChannelForm.Focus();
            }
        }

        /// <summary>
        /// 在IO监视器上显示byte[]数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="desc"></param>
        public void ShowMonitorData(byte[] data, string desc)
        {
            if (_monitorChannelForm != null && !_monitorChannelForm.IsDisposed)
            {
                string hexString = String.Format("[{0}]:{1}", desc, BinaryUtil.ByteToHex(data));
                _monitorChannelForm.Update(hexString);
            }
        }

        /// <summary>
        /// 设备驱动的临时标签
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 同步对象锁
        /// </summary>
        public object SyncLock
        {
            get { return this._SyncLock; }
        }

        /// <summary>
        /// 实时数据接口
        /// </summary>
        public abstract IDeviceDynamic DeviceDynamic { get; }

        /// <summary>
        /// 参数数据接口
        /// </summary>
        public abstract IDeviceParameter DeviceParameter { get; }

        /// <summary>
        /// 协议驱动接口
        /// </summary>
        public abstract IProtocolDriver Protocol { get; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public abstract DeviceType DeviceType { get;}

        /// <summary>
        /// 设备编号
        /// </summary>
        public abstract string ModelNumber { get; }

        /// <summary>
        /// 设备运行的优先级别，决定是否优先调用设备运行
        /// </summary>
        public DevicePriority DevicePriority { get; set; }

        /// <summary>
        /// 通讯类型
        /// </summary>
        public CommunicateType CommunicateType { get; set; }

        /// <summary>
        /// 命令缓冲区，如果没有可用的命令，则调用GetRealTimeCommand接口
        /// </summary>
        public ICommandCache CommandCache
        {
            get { return this._CommandCache; }
        }

        /// <summary>
        /// 是否运行设备
        /// </summary>
        public bool IsRunDevice { set; get; }


        public abstract System.Windows.Forms.Control DeviceGraphics { get; }

        ///// <summary>
        ///// 接口数据事件
        ///// </summary>
        //public event ReceiveDataHandler ReceiveData;

        //public void OnReceiveData(byte[] revdata)
        //{
        //    if (this.ReceiveData == null) return;
        //    ReceiveDataArgs args = null;
        //    if (this.CommunicateType == CommunicateType.COM)
        //    {
        //        args = new ReceiveDataArgs(
        //            this.DeviceParameter.DeviceID,
        //            this.DeviceParameter.DeviceAddr,
        //            this.DeviceParameter.DeviceName,
        //            this.DeviceParameter.COM.Port,
        //            this.DeviceParameter.COM.Baud,
        //            revdata);
        //    }
        //    else if (this.CommunicateType == CommunicateType.NET)
        //    {
        //        args = new ReceiveDataArgs(
        //            this.DeviceParameter.DeviceID,
        //            this.DeviceParameter.DeviceAddr,
        //            this.DeviceParameter.DeviceName,
        //            this.DeviceParameter.NET.RemoteIP,
        //            this.DeviceParameter.NET.RemotePort,
        //            revdata);
        //    }
        //    this.ReceiveData(this, args);
        //}

        /// <summary>
        /// 发送数据事件
        /// </summary>
        public event SendDataHandler SendData;

        public void OnSendData(byte[] senddata)
        {
            if (this.SendData == null) return;
            SendDataArgs args = null;
            if (this.CommunicateType == CommunicateType.COM)
            {
                args = new SendDataArgs(
                    this.DeviceParameter.DeviceID,
                    this.DeviceParameter.DeviceAddr,
                    this.DeviceParameter.DeviceName,
                    this.DeviceParameter.COM.Port,
                    this.DeviceParameter.COM.Baud,
                    senddata);
            }
            else if (this.CommunicateType == CommunicateType.NET)
            {
                args = new SendDataArgs(this.DeviceParameter.DeviceID,
                    this.DeviceParameter.DeviceAddr,
                    this.DeviceParameter.DeviceName,
                    this.DeviceParameter.NET.RemoteIP,
                    this.DeviceParameter.NET.RemotePort,
                    senddata);
            }

            this.SendData(this, args);
        }

        /// <summary>
        /// 显示的日志事件
        /// </summary>
        public event DeviceRuningLogHandler DeviceRuningLog;

        public void OnDeviceRuningLog(string statetext)
        {
            if (this.DeviceRuningLog == null) return;

            DeviceRuningLogArgs args = new DeviceRuningLogArgs(
                this.DeviceParameter.DeviceID,
                this.DeviceParameter.DeviceAddr,
                this.DeviceParameter.DeviceName,
                statetext);

            this.DeviceRuningLog(this, args);
        }

        /// <summary>
        /// 更改串口事件
        /// </summary>
        public event ComParameterExchangeHandler ComParameterExchange;

        public void OnComParameterExchange(int oldcom, int oldbaud, int newcom, int newbaud)
        {
            if (this.ComParameterExchange == null) return;

            ComParameterExchangeArgs args = new ComParameterExchangeArgs(
                this.DeviceParameter.DeviceID,
                this.DeviceParameter.DeviceAddr,
                this.DeviceParameter.DeviceName,
                this.DeviceParameter.COM.Port,
                this.DeviceParameter.COM.Baud,
                oldcom,
                oldbaud,
                newcom,
                newbaud);

            this.ComParameterExchange(this, args);
        }

        /// <summary>
        /// 对象数据改变事件
        /// </summary>
        public event DeviceObjectChangedHandler DeviceObjectChanged;

        public void OnDeviceObjectChanged(object obj)
        {
            if (this.DeviceObjectChanged == null) return;

            DeviceObjectChangedArgs args = new DeviceObjectChangedArgs(
                this.DeviceParameter.DeviceID,
                this.DeviceParameter.DeviceAddr,
                this.DeviceParameter.DeviceName,
                obj,
                this.DeviceType);

            this.DeviceObjectChanged.BeginInvoke(this, args,null,null);
        }

        //public event DeleteDeviceCompletedHandler DeleteDeviceCompleted;
        ///// <summary>
        ///// 删除设备事件
        ///// </summary>
        //public void OnDeleteDeviceCompleted()
        //{
        //    if (this.DeleteDeviceCompleted == null) return;

        //    DeleteDeviceCompletedArgs args = new DeleteDeviceCompletedArgs(
        //        this.DeviceParameter.DeviceID,
        //        this.DeviceParameter.DeviceAddr,
        //        this.DeviceParameter.DeviceName);

        //    this.DeleteDeviceCompleted(this, args);
        //}

        /// <summary>
        /// 虚拟设备运行接口
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="obj"></param>
        public virtual void RunVirtualDevice(int devid, object obj)
        {
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_IsDisposed)
            {
                if (disposing)
                {
                    this._Timer.Close();
                    this._Timer.Dispose();
                    this._Timer = null;
                }

                _IsDisposed = true;
            }
        }

        /// <summary>
        /// 是否释放资源
        /// </summary>
        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }
    }
}
