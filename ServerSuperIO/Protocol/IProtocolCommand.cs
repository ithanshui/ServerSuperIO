using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ServerSuperIO.Protocol
{
    public interface IProtocolCommand
    {

        /// <summary>
        /// 安装协议驱动
        /// </summary>
        /// <param name="driver"></param>
        void Setup(IProtocolDriver driver);

        /// <summary>
        /// 获得协议驱动
        /// </summary>
        IProtocolDriver ProtocolDriver { get; }

        /// <summary>
        /// 命令名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        object Analysis(byte[] data, object obj);

        /// <summary>
        /// 打包数据
        /// </summary>
        /// <param name="devaddr"></param>
        /// <param name="cmd"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Package(int devaddr,object obj);
    }
}
