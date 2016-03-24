using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Log
{
    public class LogFactory:ILogFactory
    {
        /// <summary>
        /// 创建日志实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILog GetLog(string name)
        {
            return new ConsoleLog(name);
        }
    }
}
