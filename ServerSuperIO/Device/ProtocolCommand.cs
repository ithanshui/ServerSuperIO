using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Protocol;

namespace ServerSuperIO.Device
{
    public abstract class ProtocolCommand : IProtocolCommand
    {
        protected ProtocolCommand()
        {}
        /// <summary>
        /// 命令名称，唯一
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract object Analysis(byte[] data, object obj);

        /// <summary>
        /// 打包数据
        /// </summary>
        /// <param name="devaddr"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract byte[] Package(int devaddr, object obj);

        /// <summary>
        /// 安装协议驱动
        /// </summary>
        /// <param name="driver"></param>
        public void Setup(IProtocolDriver driver)
        {
            ProtocolDriver = driver;
        }

        /// <summary>
        /// 协议驱动实例
        /// </summary>
        public IProtocolDriver ProtocolDriver { get; private set; }
    }
}
