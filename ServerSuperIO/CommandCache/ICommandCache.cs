using System;
namespace ServerSuperIO.CommandCache
{
    public interface ICommandCache
    {
        /// <summary>
        /// 增加命令
        /// </summary>
        /// <param name="cmd"></param>
        void Add(ICommand cmd);

        /// <summary>
        /// 增加命令
        /// </summary>
        /// <param name="cmdkey"></param>
        /// <param name="cmdbytes"></param>
        void Add(string cmdkey, byte[] cmdbytes);

        /// <summary>
        /// 增加命令
        /// </summary>
        /// <param name="cmdkey"></param>
        /// <param name="cmdbytes"></param>
        /// <param name="priority"></param>
        void Add(string cmdkey, byte[] cmdbytes, CommandPriority priority);

        /// <summary>
        /// 清空命令缓冲区
        /// </summary>
        void Clear();

        /// <summary>
        /// 命令总数 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <returns></returns>
        byte[] Get();

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        byte[] Get(CommandPriority priority);

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="cmdkey"></param>
        /// <returns></returns>
        byte[] Get(string cmdkey);

        /// <summary>
        /// 删除命令
        /// </summary>
        /// <param name="cmdkey"></param>
        void Remove(string cmdkey);
    }
}
