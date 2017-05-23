﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;

namespace ServerSuperIO.Config
{
    public interface IConfig
    {
        /// <summary>
        /// 串口读缓存
        /// </summary>
        int ComReadBufferSize { get; set; }

        /// <summary>
        /// 串口写缓存
        /// </summary>
        int ComWriteBufferSize { get; set; }

        /// <summary>
        /// 串口读超时
        /// </summary>
        int ComReadTimeout { get; set; }

        /// <summary>
        /// 串口写超时
        /// </summary>
        int ComWriteTimeout { get; set; }

        /// <summary>
        /// 轮询模式下的中断间隔时间
        /// </summary>
        int ComLoopInterval { get; set; }

        /// <summary>
        /// 网络读缓存
        /// </summary>
        int NetReceiveBufferSize { get; set; }

        /// <summary>
        /// 网络写缓存
        /// </summary>
        int NetSendBufferSize { get; set; }

        /// <summary>
        /// 网络读超时
        /// </summary>
        int NetReceiveTimeout { get; set; }

        /// <summary>
        /// 网络写超时
        /// </summary>
        int NetSendTimeout { get; set; }

        /// <summary>
        /// 轮询模式下的中断间隔时间
        /// </summary>
        int NetLoopInterval { get; set; }

        /// <summary>
        /// 是否检测相同的SocketSession
        /// </summary>
        bool IsCheckSameSocketSession { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        int MaxConnects { get; set; }

        /// <summary>
        /// 设置网络心跳检测 
        /// </summary>
        uint KeepAlive { get; set; }

        /// <summary>
        /// Socket侦听的端口
        /// </summary>
        int ListenPort { get; set; }

        /// <summary>
        /// Socket侦听的最大队列
        /// </summary>
        int BackLog { get; set; }

        /// <summary>
        /// 控制模式
        /// </summary>
        ControlMode ControlMode { get; set; }

        /// <summary>
        /// socket模式
        /// </summary>
        SocketMode SocketMode { get; set; }

        /// <summary>
        /// 分发模式
        /// </summary>
        DeliveryMode DeliveryMode { get; set; }
    }
}
