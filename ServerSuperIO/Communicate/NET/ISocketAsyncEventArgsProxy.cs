using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    public interface ISocketAsyncEventArgsProxy
    {
        SocketAsyncEventArgs SocketReceiveEventArgs { get; set; }

        SocketAsyncEventArgs SocketSendEventArgs { get; set; }

        int ReceiveOffset { get; }

        void Initialize(ISocketSession session);

        void Reset();
    }
}
