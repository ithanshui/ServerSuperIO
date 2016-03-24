using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ServerSuperIO.Communicate
{
    public interface IRequestInfo
    {
        string Key { get; }

        byte[] Data { get; }

        IChannel Channel { get;}
    }
}
