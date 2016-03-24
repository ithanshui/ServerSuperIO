using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Base;

namespace ServerSuperIO.Show
{
    public interface IGraphicsShow : IPlugin
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
        /// 显示窗体
        /// </summary>
        void Show();

        /// <summary>
        /// 关闭窗体
        /// </summary>
        void Close();

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
        ///     关闭窗体事件时发生
        /// </summary>
        event GraphicsShowClosedHandler GraphicsShowClosed;

        /// <summary>
        ///     单击右键
        /// </summary>
        event MouseRightContextMenuHandler MouseRightContextMenu;

        /// <summary>
        /// 是否被释放
        /// </summary>
        bool IsDisposed { get; }
    }
}
