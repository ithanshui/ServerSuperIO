using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Base;

namespace ServerSuperIO.Service
{
    /// <summary>
    ///     这个接口的UpdateDevice与设备的DeviceObjectChangedHandler事件和OnDeviceObjectChangedHandler函数关联
    /// </summary>
    public interface IAppService : IPlugin
    {
        /// <summary>
        ///     服务Key,要求唯一
        /// </summary>
        string ThisKey { get; }

        /// <summary>
        ///     服务名称
        /// </summary>
        string ThisName { get; }

        /// <summary>
        ///     更新设备
        /// </summary>
        /// <param name="devid">设备ID</param>
        /// <param name="obj">设备对象</param>
        void UpdateDevice(int devid, object obj);

        /// <summary>
        ///     移除设备
        /// </summary>
        /// <param name="devid">设备ID</param>
        void RemoveDevice(int devid);

        /// <summary>
        ///     启动服务
        /// </summary>
        void StartService();

        /// <summary>
        ///     释放服务
        /// </summary>
        void StopService();

        /// <summary>
        /// 服务日志
        /// </summary>
        event AppServiceLogHandler AppServiceLog;

        /// <summary>
        /// 是否被释放
        /// </summary>
        bool IsDisposed { get; }
    }
}
