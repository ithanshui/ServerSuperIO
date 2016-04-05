using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Xsl;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;

namespace ServerSuperIO.Server
{
    public class Server : AppServer
    {
        private List<ISocketListener> _Listeners;
        private BufferManager _BufferManager;
        private SocketAsyncEventArgsProxyPool _SocketAsyncPool;
        private ISocketConnector _SocketConnector;
        public Server(string serverName, IConfig config)
            : base(serverName, config)
        {

        }

        public override void Start()
        {
            InitSocketAsyncPool();
            InitListener();
            InitConnector();
            base.Start();
        }

        private void InitListener()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] ipasAddresses = Dns.GetHostAddresses(hostName);
            List<IPAddress> list = new List<IPAddress>(ipasAddresses) { IPAddress.Parse("127.0.0.1") };
            _Listeners = new List<ISocketListener>(list.Count);
            foreach (IPAddress ipa in list)
            {
                ListenerInfo info = new ListenerInfo()
                {
                    BackLog = Config.BackLog,
                    EndPoint = new IPEndPoint(ipa, Config.ListenPort)
                };

                ISocketListener socketListener;
                if (this.Config.SocketMode == SocketMode.Tcp)
                {
                    socketListener = new TcpSocketListener(info);
                }
                else
                {
                    socketListener = new UdpSocketListener(info);
                }

                socketListener.NewClientAccepted += tcpSocketListener_NewClientAccepted;
                socketListener.Error += tcpSocketListener_Error;
                socketListener.Stopped += tcpSocketListener_Stopped;
                socketListener.Start(this.Config);

                _Listeners.Add(socketListener);
            }
        }

        private void InitSocketAsyncPool()
        {
            int netReceiveBufferSize = Config.NetReceiveBufferSize;
            if (netReceiveBufferSize <= 0)
                netReceiveBufferSize = 1024;

            _BufferManager = new BufferManager(netReceiveBufferSize * Config.MaxConnects, netReceiveBufferSize);

            try
            {
                _BufferManager.InitBuffer();
            }
            catch (Exception ex)
            {
                Logger.Error(true, "", ex);
                throw;
            }

            _SocketAsyncPool = new SocketAsyncEventArgsProxyPool();

            SocketAsyncEventArgs socketEventArg;
            for (int i = 0; i < Config.MaxConnects; i++)
            {
                socketEventArg = new SocketAsyncEventArgs();
                _BufferManager.SetBuffer(socketEventArg);
                _SocketAsyncPool.Push(new SocketAsyncEventArgsProxy(socketEventArg));
            }
        }

        private void InitConnector()
        {
            if (Config.ControlMode == ControlMode.Loop
                || Config.ControlMode == ControlMode.Self
                || Config.ControlMode == ControlMode.Parallel)
            {
                _SocketConnector = new SocketConnector();
                _SocketConnector.NewClientConnected += tcpSocketListener_NewClientAccepted;
                _SocketConnector.Error += tcpSocketListener_Error;
                _SocketConnector.Setup(this);
                _SocketConnector.Start();
            }
        }

        public override void Stop()
        {
            if (_SocketConnector != null)
            {
                _SocketConnector.Stop();
                _SocketConnector.Dispose();
            }

            if (_Listeners != null && _Listeners.Count > 0)
            {
                foreach (ISocketListener socketListener in _Listeners)
                {
                    socketListener.Stop();
                    socketListener.NewClientAccepted -= tcpSocketListener_NewClientAccepted;
                    socketListener.Error -= tcpSocketListener_Error;
                    socketListener.Stopped -= tcpSocketListener_Stopped;
                }
                _Listeners.Clear();
                _Listeners = null;
            }

            if (_SocketAsyncPool != null)
            {
                _SocketAsyncPool.Clear();
                _SocketAsyncPool = null;
            }

            _BufferManager = null;

            base.Stop();
        }

        private void tcpSocketListener_Stopped(object sender, EventArgs e)
        {
            Logger.Info(true, "网络侦听停止");
        }

        private void tcpSocketListener_Error(object sender, Exception e)
        {
            Logger.Info(true, e.Message);
        }

        private void tcpSocketListener_NewClientAccepted(object sender, System.Net.Sockets.Socket client, object state)
        {
            if (this.Config.SocketMode == SocketMode.Tcp)
            {
                if (this.Config.ControlMode == ControlMode.Loop
                    || this.Config.ControlMode == ControlMode.Self
                    || this.Config.ControlMode == ControlMode.Parallel)
                {
                    if (Config.IsCheckSameSocketSession)
                    {
                        string[] ipInfo = client.RemoteEndPoint.ToString().Split(':');
                        IChannel socketSession = ChannelManager.GetChannel(ipInfo[0], CommunicateType.NET);
                        if (socketSession != null)
                        {
                            RemoveTcpSocketSession((ISocketSession) socketSession);
                        }
                    }
                }

                AddTcpSocketSession(client);
            }
            else if (this.Config.SocketMode == SocketMode.Udp)
            {
                object[] arr = (object[])state;
                ISocketSession socketSession=new UdpSocketSession(client,(IPEndPoint)arr[1],null);
                socketChannel_SocketReceiveData(socketSession, socketSession, (byte[])arr[0]);
            }
        }

        private void socketChannel_SocketReceiveData(object source, ISocketSession socketSession, byte[] data)
        {
            ISocketController netController = (ISocketController)ControllerManager.GetController(SocketController.ConstantKey);
            if (netController != null)
            {
                netController.Receive(socketSession, data);
            }
            else
            {
                Logger.Info(false, SocketController.ConstantKey + ",无法找到对应的网络控制器");
            }
        }

        private void socketChannel_CloseSocket(object source, ISocketSession socketSession)
        {
            RemoveTcpSocketSession(socketSession);
        }

        private void AddTcpSocketSession(Socket client)
        {
            if (client == null)
                return;

            lock (ChannelManager.SyncLock)
            {
                ISocketAsyncEventArgsProxy socketProxy = this._SocketAsyncPool.Pop();
                if (socketProxy == null)
                {
                    AsyncUtil.AsyncRun(client.SafeClose);
                    Logger.Info(false, "已经到达最大连接数");
                    return;
                }

                ISocketSession socketSession = new TcpSocketSession(client,(IPEndPoint)client.RemoteEndPoint, socketProxy);
                socketSession.Setup(this);
                socketSession.Initialize();

                if (ChannelManager.AddChannel(socketSession.SessionID, socketSession))
                {
                    socketSession.CloseSocket += socketChannel_CloseSocket;
                    if (Config.ControlMode == ControlMode.Self
                        || Config.ControlMode == ControlMode.Parallel
                        || Config.ControlMode == ControlMode.Singleton)
                    {
                        socketSession.SocketReceiveData += socketChannel_SocketReceiveData;
                        AsyncUtil.AsyncRun(socketSession.TryReceive);
                    }

                    OnSocketConnected(socketSession.RemoteIP, socketSession.RemotePort);

                    OnChannelChanged(socketSession.RemoteIP, CommunicateType.NET, ChannelState.Open);
                }
                else
                {
                    ISocketAsyncEventArgsProxy proxy = socketSession.SocketAsyncProxy;
                    proxy.Reset();
                    if (proxy.ReceiveOffset != proxy.SocketReceiveEventArgs.Offset)
                    {
                        proxy.SocketReceiveEventArgs.SetBuffer(proxy.ReceiveOffset, Config.NetReceiveBufferSize);
                    }
                    _SocketAsyncPool.Push(proxy);
                    socketSession.Close();
                    socketSession = null;
                    Logger.Info(true, "增加网络连接实例失败");
                }
            }
        }

        private void RemoveTcpSocketSession(ISocketSession socketSession)
        {
            if (socketSession == null)
                return;

            lock (ChannelManager.SyncLock)
            {
                if (ChannelManager.ContainChannel(socketSession.SessionID))
                {
                    if (ChannelManager.RemoveChannel(socketSession.SessionID))
                    {
                        ISocketAsyncEventArgsProxy proxy = socketSession.SocketAsyncProxy;

                        if (proxy.ReceiveOffset != proxy.SocketReceiveEventArgs.Offset)
                        {
                            proxy.SocketReceiveEventArgs.SetBuffer(proxy.ReceiveOffset, Config.NetReceiveBufferSize);
                        }

                        _SocketAsyncPool.Push(proxy);

                        socketSession.Close();

                        OnSocketClosed(socketSession.RemoteIP, socketSession.RemotePort);

                        OnChannelChanged(socketSession.RemoteIP, CommunicateType.NET, ChannelState.Close);

                        socketSession = null;
                    }
                    else
                    {
                        Logger.Info(true, "关闭网络连接失败");
                    }
                }
            }
        }
    }
}
