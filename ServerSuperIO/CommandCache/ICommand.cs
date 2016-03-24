using System;

namespace ServerSuperIO.CommandCache
{
    public interface ICommand
    {
        /// <summary>
        /// 命令字节
        /// </summary>
        byte[] CommandBytes { get; }

        /// <summary>
        /// 命令关键字
        /// </summary>
        string CommandKey { get; }

        /// <summary>
        /// 命令优先级
        /// </summary>
        CommandPriority Priority { get; }
    }
}
