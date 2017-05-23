using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using ServerSuperIO.Common;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    public class TcpSocketSession : SocketSession
    {
        /// <summary>
        /// 无数状态下记数器
        /// </summary>
        private int _NoneDataCounter = 0;

        /// <summary>
        /// 设置多长时间后检测网络状态
        /// </summary>
        private byte[] _KeepAliveOptionValues;

        /// <summary>
        /// 设置检测网络状态间隔时间
        /// </summary>
        private byte[] _KeepAliveOptionOutValues;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="proxy"></param>
        public TcpSocketSession(Socket socket,  IPEndPoint remoteEndPoint,ISocketAsyncEventArgsProxy proxy)
            : base(socket,remoteEndPoint,proxy)
        {
        }

        public override void Initialize()
        {
            if (Client != null)
            {
                //-------------------初始化心跳检测---------------------//
                uint dummy = 0;
                _KeepAliveOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
                _KeepAliveOptionOutValues = new byte[_KeepAliveOptionValues.Length];
                BitConverter.GetBytes((uint)1).CopyTo(_KeepAliveOptionValues, 0);
                BitConverter.GetBytes((uint)(2000)).CopyTo(_KeepAliveOptionValues, Marshal.SizeOf(dummy));

                uint keepAlive = this.Server.Config.KeepAlive;

                BitConverter.GetBytes((uint)(keepAlive)).CopyTo(_KeepAliveOptionValues, Marshal.SizeOf(dummy) * 2);

                Client.IOControl(IOControlCode.KeepAliveValues, _KeepAliveOptionValues, _KeepAliveOptionOutValues);

                Client.NoDelay = true;
                Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                //----------------------------------------------------//

                Client.ReceiveTimeout = Server.Config.NetReceiveTimeout;
                Client.SendTimeout = Server.Config.NetSendTimeout;
                Client.ReceiveBufferSize = Server.Config.NetReceiveBufferSize;
                Client.SendBufferSize = Server.Config.NetSendBufferSize;
            }

            if (SocketAsyncProxy != null)
            {
                SocketAsyncProxy.Initialize(this);
                SocketAsyncProxy.SocketReceiveEventArgs.Completed += SocketEventArgs_Completed;
                SocketAsyncProxy.SocketSendEventArgs.Completed += SocketEventArgs_Completed;
            }
        }

        /// <summary>
        /// 读操作
        /// </summary>
        /// <returns></returns>
        public override byte[] Read()
        {
            if (!this.IsDisposed)
            {
                System.Threading.Thread.Sleep(Server.Config.NetLoopInterval);
                if (this.Client.Connected)
                {
                    if (this.Client.Poll(10, SelectMode.SelectRead))
                    {
                        try
                        {
                            byte[] buffer = SocketAsyncProxy.SocketReceiveEventArgs.Buffer;

                            #region

                            int num = this.Client.Receive(buffer, SocketAsyncProxy.ReceiveOffset, Client.ReceiveBufferSize, SocketFlags.None);

                            if (num <= 0)
                            {
                                throw new SocketException((int) SocketError.HostDown);
                            }
                            else
                            {
                                this._NoneDataCounter = 0;
                                byte[] data = new byte[num];
                                Buffer.BlockCopy(buffer, SocketAsyncProxy.ReceiveOffset, data, 0, data.Length);
                                return data;
                            }

                            #endregion
                        }
                        catch (SocketException)
                        {
                            OnCloseSocket();
                            throw;
                        }
                    }
                    else
                    {
                        this._NoneDataCounter++;
                        if (this._NoneDataCounter >= 60)
                        {
                            this._NoneDataCounter = 0;
                            OnCloseSocket();
                            throw new SocketException((int)SocketError.HostDown);
                        }
                        else
                        {
                            return new byte[] { };
                        }
                    }
                }
                else
                {
                    OnCloseSocket();
                    throw new SocketException((int)SocketError.HostDown);
                }
            }
            else
            {
                return new byte[] { };
            }
        }

        /// <summary>
        /// 写操作
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Write(byte[] data)
        {
            if (!this.IsDisposed)
            {
                if (this.Client.Connected
                    &&
                    this.Client.Poll(10, SelectMode.SelectWrite))
                {
                    try
                    {
                        int successNum = 0;
                        int num = 0;
                        while (num < data.Length)
                        {
                            int remainLength = data.Length - num;
                            int sendLength = remainLength >= this.Client.SendBufferSize? this.Client.SendBufferSize : remainLength;

                            SocketError error;
                            successNum += this.Client.Send(data, num, sendLength, SocketFlags.None, out error);

                            num += sendLength;

                            if (successNum <= 0 || error != SocketError.Success)
                            {
                                OnCloseSocket();
                                throw new SocketException((int) SocketError.HostDown);
                            }
                        }

                        return successNum;
                    }
                    catch (SocketException)
                    {
                        OnCloseSocket();
                        throw;
                    }
                }
                else
                {
                    OnCloseSocket();
                    throw new SocketException((int)SocketError.HostDown);
                }
            }
            else
            {
                return 0;
            }
        }

        public override void TryReceive()
        {
            if (Client != null)
            {
                try
                {
                    bool willRaiseEvent = this.Client.ReceiveAsync(this.SocketAsyncProxy.SocketReceiveEventArgs);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(this.SocketAsyncProxy.SocketReceiveEventArgs);
                    }
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, ex.Message);
                }
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ISocketSession socketSession = (ISocketSession)e.UserToken;
            if (socketSession != null && socketSession.Client!=null)
            {
                try
                {
                    if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                    {
                        byte[] data = new byte[e.BytesTransferred];
                        Buffer.BlockCopy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);

                        bool willRaiseEvent =
                            socketSession.Client.ReceiveAsync(this.SocketAsyncProxy.SocketReceiveEventArgs);
                        if (!willRaiseEvent)
                        {
                            ProcessReceive(this.SocketAsyncProxy.SocketReceiveEventArgs);
                        }

                        OnSocketReceiveData(data);
                    }
                    else
                    {
                        OnCloseSocket();
                    }
                }
                catch (SocketException ex)
                {
                    OnCloseSocket();
                    this.Server.Logger.Error(true, ex.Message);
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, ex.Message);
                }
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    byte[] data = (byte[])e.UserToken;

                    if (e.BytesTransferred < data.Length)
                    {
                        e.SetBuffer(data,e.BytesTransferred,data.Length-e.BytesTransferred);
                        bool willRaiseEvent = this.Client.SendAsync(e);
                        if (!willRaiseEvent)
                        {
                            ProcessSend(e);
                        }
                    }
                    else
                    {
                        e.UserToken = null;
                    }
                }
                else
                {
                    OnCloseSocket();
                }
            }
            catch (SocketException ex)
            {
                OnCloseSocket();
                this.Server.Logger.Error(true, ex.Message);
            }
            catch (Exception ex)
            {
                this.Server.Logger.Error(true, ex.Message);
            }
        }

        protected override void SendAsync(byte[] data)
        {
            if (Client != null)
            {
                try
                {
                    this.SocketAsyncProxy.SocketSendEventArgs.UserToken = data;
                    this.SocketAsyncProxy.SocketSendEventArgs.SetBuffer(data, 0, data.Length);
                    bool willRaiseEvent = this.Client.SendAsync(this.SocketAsyncProxy.SocketSendEventArgs);
                   
                    if (!willRaiseEvent)
                    {
                        ProcessSend(this.SocketAsyncProxy.SocketSendEventArgs);
                    }
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true,ex.Message);
                }
            }
        }

        protected override void SendSync(byte[] data)
        {
            if (Client != null)
            {
                try
                {
                    this.Client.SendData(data);
                }
                catch (SocketException ex)
                {
                    OnCloseSocket();
                    this.Server.Logger.Error(true, ex.Message);
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, ex.Message);
                }
            }
        }

        protected override void SocketEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    this.Server.Logger.Info(false, "不支持接收和发送的操作");
                    break;
            }
        }
    }
}
