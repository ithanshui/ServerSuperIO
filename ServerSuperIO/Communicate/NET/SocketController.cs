﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerSuperIO.Device;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    internal class SocketController : ServerProvider, ISocketController
    {
        private Thread _Thread = null; //控制器线程

        private bool _IsDisposed = false;//是否释放资源

        private readonly object _SyncLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SocketController()
            : base()
        {
            IsExited = false;

            IsWorking = false;
        }

        /// <summary>
        /// 是否退出
        /// </summary>
        private bool IsExited { get; set; }

        /// <summary>
        /// 对于网络通讯来说，只有一个控制器，以“127.0.0.1”作为关键字
        /// </summary>
        internal static string ConstantKey
        {
            get { return "127.0.0.1"; }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="data"></param>
        public void Send(IRunDevice dev, byte[] data)
        {
            int counter = this.Server.DeviceManager.GetCounter(dev.DeviceParameter.DeviceID);

            ISocketSession socketSession =(ISocketSession)this.Server.ChannelManager.GetChannel(dev.DeviceParameter.NET.RemoteIP, CommunicateType.NET);

            if (socketSession != null)
            {
                int sendNum = 0;
                lock (socketSession.SyncLock)
                {
                    sendNum = socketSession.Write(data);
                }
                
                if (sendNum == data.Length && sendNum != 0)
                {
                    Interlocked.Increment(ref counter);
                    this.Server.Logger.Info(false, dev.DeviceParameter.DeviceName+">>发送请求数据");
                }
                else
                {
                    Interlocked.Increment(ref counter);
                    this.Server.Logger.Info(false, dev.DeviceParameter.DeviceName + ">>尝试发送数据失败");
                }

                dev.ShowMonitorData(data, "发送");

                if (counter >= 3)
                {
                    try
                    {
                        dev.Run(dev.DeviceParameter.NET.RemoteIP, null, new byte[] { });
                    }
                    catch (Exception ex)
                    {
                        this.Server.Logger.Info(true, dev.DeviceParameter.DeviceName + "," + ex.Message);
                    }

                    Interlocked.Exchange(ref counter, 0);
                }

                this.Server.DeviceManager.SetCounter(dev.DeviceParameter.DeviceID, counter);
            }
            else
            {
                try
                {
                    dev.Run((IChannel)null);   //如果没有找到连接，则传递空值
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, "网络控制器", ex);
                }
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socketSession"></param>
        /// <param name="data"></param>
        public void Receive(ISocketSession socketSession, byte[] data)
        {
            if (this.Server.Config.ControlMode == ControlMode.Loop
                || this.Server.Config.ControlMode == ControlMode.Self
                || this.Server.Config.ControlMode == ControlMode.Parallel)
            {
                #region
                IRunDevice[] list = this.Server.DeviceManager.GetDevices(socketSession.RemoteIP, CommunicateType.NET);

                if (list == null || list.Length <= 0)
                    return;

                int counter = 0;
                bool isDelivery = false;
                foreach (IRunDevice dev in list)
                {
                    
                    if (this.Server.Config.DeliveryMode == DeliveryMode.DeviceIP)
                    {
                        isDelivery = String.CompareOrdinal(dev.DeviceParameter.NET.RemoteIP, socketSession.RemoteIP) == 0 ? true : false;
                    }
                    else if (this.Server.Config.DeliveryMode == DeliveryMode.DeviceAddress)
                    {
                        if (dev.Protocol != null
                                && dev.Protocol.CheckData(data)
                                && dev.Protocol.GetAddress(data) == dev.DeviceParameter.DeviceAddr)
                        {
                            isDelivery = true;
                        }
                        else
                        {
                            isDelivery = false;
                        }
                    }

                    if (isDelivery)
                    {
                        dev.ShowMonitorData(data, "接收");

                        try
                        {
                            if (this.Server.Config.SocketMode == SocketMode.Tcp)
                            {
                                dev.Run(socketSession.Key, null, data);
                            }
                            else if (this.Server.Config.SocketMode == SocketMode.Udp)
                            {
                                dev.Run(socketSession.Key, socketSession.Channel, data);
                            }
                        }
                        catch (Exception ex)
                        {
                            Server.Logger.Error(true, "", ex);
                        }

                        counter = this.Server.DeviceManager.GetCounter(dev.DeviceParameter.DeviceID);

                        Interlocked.Decrement(ref counter);

                        if (counter < 0)
                        {
                            Interlocked.Exchange(ref counter, 0);
                        }

                        this.Server.DeviceManager.SetCounter(dev.DeviceParameter.DeviceID, counter);

                        break;
                    }
                }
                #endregion
            }
            else if (this.Server.Config.ControlMode == ControlMode.Singleton)
            {
                #region
                IRunDevice[] list = this.Server.DeviceManager.GetDevices(CommunicateType.NET);
                if (list == null || list.Length <= 0)
                    return;
                try
                {
                    list[0].Run(socketSession.Key, (IChannel)socketSession, data);
                }
                catch (Exception ex)
                {
                    Server.Logger.Error(true, "", ex);
                }
                #endregion
            }
        }

        /// <summary>
        /// 是否工作
        /// </summary>
        public bool IsWorking { get; set; }

        /// <summary>
        /// 关键字，只有一个控制器，所以关键字是固定的。
        /// </summary>
        public string Key
        {
            get { return ConstantKey; }
        }

        /// <summary>
        /// 启动控制器
        /// </summary>
        public void StartController()
        {
            if (_Thread == null || !_Thread.IsAlive)
            {
                this.IsWorking = true;
                this.IsExited = false;
                this._Thread = new Thread(new ThreadStart(RunController))
                {
                    IsBackground = true,
                    Name = "NetControllerThread"
                };
                this._Thread.Start();
            }
        }

        /// <summary>
        /// 停止控制器
        /// </summary>
        public void StopController()
        {
            Dispose(true);
        }

        /// <summary>
        /// 通讯类型
        /// </summary>
        public CommunicateType ControllerType
        {
            get { return CommunicateType.NET; }
        }

        /// <summary>
        /// 是否释放资源
        /// </summary>
        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_IsDisposed)
            {
                if (disposing)
                {

                }

                lock (_SyncLock)
                {
                    if (this._Thread != null && this._Thread.IsAlive)
                    {
                        this.IsExited = true;
                        this._Thread.Join(1500);
                        if (this._Thread.IsAlive)
                        {
                            try
                            {
                                _Thread.Abort();
                            }
                            catch
                            {

                            }
                        }
                    }
                }

                _IsDisposed = true;
            }
        }

        /// <summary>
        /// 运行器
        /// </summary>
        private void RunController()
        {
            while (!IsExited)
            {
                if (!IsWorking)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }
                ControlMode mode = Server.Config.ControlMode;
                if (mode == ControlMode.Singleton)
                {
                    System.Threading.Thread.Sleep(100); //不进行调度
                }
                else if (mode == ControlMode.Self)
                {
                    System.Threading.Thread.Sleep(100);//不进行调度
                }
                else if (mode == ControlMode.Parallel)
                {
                    #region
                    IRunDevice[] list = this.Server.DeviceManager.GetDevices(CommunicateType.NET);

                    if (list.Length <= 0)
                    {
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }

                    for (int i = 0; i < list.Length; i++)
                    {
                        if (IsExited || !IsWorking)
                        {
                            break;
                        }

                        Send(list[i], list[i].GetSendBytes());
                    }

                    System.Threading.Thread.Sleep(1000);
                    #endregion
                }
                else if (mode == ControlMode.Loop)
                {
                    #region
                    IRunDevice[] devList = this.Server.DeviceManager.GetDevices(CommunicateType.NET);

                    if (devList.Length <= 0)
                    {
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }

                    //检测当前控制器的运行优化级
                    IRunDevice dev = this.Server.DeviceManager.GetPriorityDevice(devList);

                    if (dev != null) //如果有优先级设备，则直接调度设备
                    {
                        this.RunDevice(dev);
                    }
                    else //如果没有优先级设备，则轮询调度设备
                    {
                        for (int i = 0; i < devList.Length; i++)
                        {
                            if (IsExited || !IsWorking)
                            {
                                break;
                            }

                            //---------每次循环都检测优先级，保证及时响应----------//
                            dev = this.Server.DeviceManager.GetPriorityDevice(devList);

                            if (dev != null)
                            {
                                this.RunDevice(dev);
                            }
                            //-------------------------------------------------//

                            this.RunDevice(devList[i]);
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 运行调度设备
        /// </summary>
        /// <param name="dev"></param>
        private void RunDevice(IRunDevice dev)
        {
            ISocketSession io = null;
            try
            {
                io =(ISocketSession)this.Server.ChannelManager.GetChannel(dev.DeviceParameter.NET.RemoteIP, CommunicateType.NET);
                if (io != null)
                {
                    dev.Run(io);
                }
                else
                {
                    dev.Run((IChannel)null);   //如果没有找到连接，则传递空值
                    //没有找到可用的设备，加延时，局免循环过快。
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (SocketException ex)
            {
                this.Server.Logger.Error(true, "网络控制器", ex);
            }
            catch (Exception ex)
            {
                this.Server.Logger.Error(true, "网络控制器", ex);
            }
        }
    }
}
