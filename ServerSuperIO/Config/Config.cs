using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;

namespace ServerSuperIO.Config
{
    public class Config:IConfig
    {
        public Config()
        {
            ComReadBufferSize = 1024;
            ComWriteBufferSize = 1024;
            ComReadTimeout = 1000;
            ComWriteTimeout = 1000;
            ComLoopInterval = 1000;
            NetReceiveBufferSize = 1024;
            NetSendBufferSize = 1024;
            NetReceiveTimeout = 1024;
            NetSendTimeout = 1024;
            NetLoopInterval = 1000;
            MaxConnects = 10000;
            ControlMode=ControlMode.Loop;
            ListenPort = 6699;
            BackLog = 1000;
            IsCheckSameSocketSession = true;
            SocketMode=SocketMode.Tcp;
            DeliveryMode=DeliveryMode.DeviceIP;
        }

        public int ComReadBufferSize { get; set; }

        public int ComWriteBufferSize { get; set; }

        public int ComReadTimeout { get; set; }

        public int ComWriteTimeout { get; set; }

        public int ComLoopInterval { get; set; }

        public int NetReceiveBufferSize { get; set; }

        public int NetSendBufferSize { get; set; }

        public int NetReceiveTimeout { get; set; }

        public int NetSendTimeout { get; set; }

        public int NetLoopInterval { get; set; }

        public int MaxConnects { get; set; }

        public ControlMode ControlMode { get; set; }

        public uint KeepAlive { get; set; }

        public int ListenPort { get; set; }

        public int BackLog { get; set; }

        public bool IsCheckSameSocketSession { get; set; }

        public SocketMode SocketMode { get; set; }

        public DeliveryMode DeliveryMode { get; set; }
    }
}
