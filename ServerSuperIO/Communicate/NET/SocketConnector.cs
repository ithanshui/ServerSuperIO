using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerSuperIO.Common;
using ServerSuperIO.Device;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    internal class SocketConnector : ServerProvider, ISocketConnector
    {
        private bool _IsDisposed = false;
        private bool _IsExited = false;
        private Thread _Thread = null;

        public SocketConnector()
        {
        }

        ~SocketConnector()
        {
            Dispose(false);
        }

        public void Start()
        {
            if (_Thread == null || !_Thread.IsAlive)
            {
                this._Thread = new Thread(new ThreadStart(RunConnector))
                {
                    IsBackground = true,
                    Name = "NetConnectorThread"
                };
                this._Thread.Start();
            }
        }

        public void Stop()
        {
            Dispose(true);
        }

        public event NewClientAcceptHandler NewClientConnected;

        private void OnNewClientConnected(Socket socket, object state)
        {
            if (NewClientConnected != null)
                NewClientConnected.BeginInvoke(this, socket, state, null, null);
        }

        public event ErrorHandler Error;

        private void OnError(Exception e)
        {
            if (Error != null)
                Error(this, e);
        }

        private void OnError(string errorMessage)
        {
            OnError(new Exception(errorMessage));
        }

        private void RunConnector()
        {
            while (!_IsExited)
            {
                IRunDevice[] devList = this.Server.DeviceManager.GetDevices(WorkMode.TcpClient);
                if (devList.Length > 0)
                {
                    #region
                    for (int i = 0; i < devList.Length; i++)
                    {
                        IRunDevice dev = devList[i];
                        IChannel channel = this.Server.ChannelManager.GetChannel(dev.DeviceParameter.NET.RemoteIP,CommunicateType.NET);
                        if (channel == null)
                        {
                            Socket client = null;
                            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            IAsyncResult ar = client.BeginConnect(dev.DeviceParameter.NET.RemoteIP,
                                dev.DeviceParameter.NET.RemotePort, null, null);
                            bool waitSuccess = ar.AsyncWaitHandle.WaitOne(2000);
                            if (waitSuccess)
                            {
                                try
                                {
                                    client.EndConnect(ar);

                                    if (client.Connected)
                                    {
                                        OnNewClientConnected(client, null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    client.Close();
                                    client.Dispose();
                                    client = null;
                                    OnError(ex);
                                }
                            }

                            ar.AsyncWaitHandle.Close();
                            ar.AsyncWaitHandle.Dispose();
                            ar = null;
                        }
                    }
                    #endregion

                }
                System.Threading.Thread.Sleep(2000);
            }
        }

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

                if (this._Thread != null && this._Thread.IsAlive)
                {
                    this._IsExited = true;
                    this._Thread.Join(1000);
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

                _IsDisposed = true;
            }
        }
    }
}
