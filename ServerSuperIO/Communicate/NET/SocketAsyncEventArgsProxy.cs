using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    internal class SocketAsyncEventArgsProxy:ISocketAsyncEventArgsProxy
    {
        public SocketAsyncEventArgsProxy(SocketAsyncEventArgs saea)
        {
            SocketReceiveEventArgs = saea;
            ReceiveOffset = saea.Offset;
            SocketSendEventArgs=new SocketAsyncEventArgs();
        }

        public SocketAsyncEventArgs SocketReceiveEventArgs { get; set; }

        public SocketAsyncEventArgs SocketSendEventArgs { get; set; }

        public int ReceiveOffset { get; private set; }

        public void Initialize(ISocketSession session)
        {
            SocketReceiveEventArgs.UserToken = session;
            SocketSendEventArgs.UserToken = session;
        }

        public void Reset()
        {
            SocketReceiveEventArgs.UserToken = null;
            SocketSendEventArgs.UserToken = null;
        }
    }
}
