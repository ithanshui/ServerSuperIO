using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Device;

namespace ServerSuperIO.Protocol
{
    public interface IProtocolDriver
    {
        /// <summary>
        /// 初始化协议驱动
        /// </summary>
        void InitDriver(IRunDevice runDevice);

        /// <summary>
        /// 获得协议命令
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        IProtocolCommand GetProcotolCommand(string cmdName);

        /// <summary>
        /// 驱动解析
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        object DriverAnalysis(string cmdName, byte[] data, object obj);

        /// <summary>
        /// 驱动打包
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="cmdName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] DriverPackage(int addr, string cmdName, object obj);

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>true:校验成功 false:校验失败</returns>
        bool CheckData(byte[] data);

        /// <summary>
        /// 获得校验数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetCheckData(byte[] data);

        /// <summary>
        /// 获得命令集全，如果命令和命令参数
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>返回命令集合</returns>
        byte[] GetCommand(byte[] data);

        /// <summary>
        /// 获得该设备的地址
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>返回地址</returns>
        int GetAddress(byte[] data);

        /// <summary>
        /// 协议头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetProHead(byte[] data);

        /// <summary>
        /// 协议尾
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetProEnd(byte[] data);
    }
}
