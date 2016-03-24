using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate
{
    public class RequestInfo:IRequestInfo
    {
        public string Key { get; internal set; }

        public byte[] Data { get; internal set; }

        public IChannel Channel { get; internal set; }
    }
}
