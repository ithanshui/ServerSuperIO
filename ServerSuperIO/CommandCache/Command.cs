using System;
using ServerSuperIO.CommandCache;

namespace ServerSuperIO.CommandCache
{
    public class Command : ServerSuperIO.CommandCache.ICommand 
    {
        private byte[] _CommandBytes = new byte[] { };

        /// <summary>
        /// 命令
        /// </summary>
        public byte[] CommandBytes
        {
            get { return _CommandBytes; }
        }

        private string _CommandKey = String.Empty;
        /// <summary>
        /// 命令名称
        /// </summary>
        public string CommandKey
        {
            get { return _CommandKey; }
        }

        private CommandPriority _Priority = CommandPriority.Normal;
        /// <summary>
        /// 发送优先级，暂时不用
        /// </summary>
        public CommandPriority Priority
        {
            get { return _Priority; }
        }

        /// <summary>
        /// 设备命令
        /// </summary>
        /// <param name="cmdkeys">命令名称</param>
        /// <param name="cmdbytes">命令字节数组</param>
        public Command(string cmdkey, byte[] cmdbytes)
        {
            this._CommandKey = cmdkey;
            this._CommandBytes = cmdbytes;
            this._Priority = CommandPriority.Normal;
        }

        /// <summary>
        /// 设备命令
        /// </summary>
        /// <param name="cmdkeys">命令名称</param>
        /// <param name="cmdbytes">命令字节数组</param>
        public Command(string cmdkey, byte[] cmdbytes,CommandPriority priority)
        {
            this._CommandKey = cmdkey;
            this._CommandBytes = cmdbytes;
            this._Priority = priority;
        }
    }
}
