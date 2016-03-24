using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    internal interface ISocketListener
    {
        IPEndPoint EndPoint { get; }

        ListenerInfo ListenerInfo { get; }

        bool Start();

        void Stop();

        event NewClientAcceptHandler NewClientAccepted;

        event ErrorHandler Error;

        event EventHandler Stopped;
    }
}
