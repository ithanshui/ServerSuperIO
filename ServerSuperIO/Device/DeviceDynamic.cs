using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;

namespace ServerSuperIO.Device
{
    [Serializable]
    public abstract class DeviceDynamic:IDeviceDynamic
    {
        protected DeviceDynamic()
        {
            DeviceID = -1;
            Remark = String.Empty;
            RunState=RunState.None;
            CommunicateState=CommunicateState.None;
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public int DeviceID { get; set; }

        /// <summary>
        /// 实时状态备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        public RunState RunState { get; set; }

        /// <summary>
        /// 通讯状态
        /// </summary>
        public CommunicateState CommunicateState { get; set; }

        /// <summary>
        /// IO状态
        /// </summary>
        public ChannelState ChannelState { get; set; }

        /// <summary>
        /// 获得报警状态
        /// </summary>
        /// <returns></returns>
        public abstract string GetAlertState();

        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "ServerSuperIO/Dynamic/";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return String.Format("{0}/{1}.xml", path, this.DeviceID.ToString());
            }
        }

        /// <summary>
        /// 修复实体
        /// </summary>
        /// <returns></returns>
        public abstract object Repair();

        /// <summary>
        /// 保存实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Save<T>(T t)
        {
            SerializeUtil.XmlSerialize<T>(this.SavePath, t);
        }

        /// <summary>
        /// 加载实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>()
        {
            try
            {
                return SerializeUtil.XmlDeserailize<T>(this.SavePath);
            }
            catch
            {
                return (T)Repair();
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        public void Delete()
        {
            if (System.IO.File.Exists(this.SavePath))
            {
                System.IO.File.Delete(this.SavePath);
            }
        }
    }
}
