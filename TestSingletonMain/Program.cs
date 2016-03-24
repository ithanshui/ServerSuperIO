using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Communicate;
using ServerSuperIO.Config;
using ServerSuperIO.Server;
using ServerSuperIO.Service;
using ServerSuperIO.Show;
using TestDeviceDriver;
using TestService;
using TestShowForm;

namespace TestSingletonMain
{
    class Program
    {
        private static object _obj=new object();
        private static int _counter = 0;
        static void Main(string[] args)
        {
            DeviceSingletonDriver dev3 = new DeviceSingletonDriver();
            dev3.DeviceParameter.DeviceName = "设备3";
            dev3.DeviceParameter.DeviceAddr = 0;
            dev3.DeviceParameter.DeviceID = 2;
            dev3.DeviceDynamic.DeviceID = 2;
            dev3.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev3.DeviceParameter.NET.RemotePort = 9600;
            dev3.CommunicateType = CommunicateType.NET;
            dev3.Initialize(2);

            Server server = new Server("服务1", new Config()
            {
                ControlMode = ControlMode.Singleton
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.SocketConnected+=server_SocketConnected;
            server.SocketClosed+=server_SocketClosed;
            server.Start();

            server.AddDevice(dev3);

            Service s = new Service();
            s.AppServiceLog += s_AppServiceLog;
            server.AddAppService((IAppService)s);

            Graphics g = new Graphics();
            server.AddGraphicsShow((IGraphicsShow)g);

            while ("exit" == Console.ReadLine())
            {
                server.Stop();
            }
        }

        private static void server_SocketClosed(string ip, int port)
        {
            lock (_obj)
            {
                _counter--;
                Console.WriteLine(String.Format("{0},连接：{1}-{2} 断开", _counter, ip, port));
            }
        }

        private static void server_SocketConnected(string ip, int port)
        {
            lock (_obj)
            {
                _counter++;
                Console.WriteLine(String.Format("{0},连接：{1}-{2} 成功", _counter, ip, port));
            }
        }

        private static void s_AppServiceLog(string log)
        {
            Console.WriteLine(log);
        }

        private static void server_AddDeviceCompleted(int devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",增加:" + isSuccess.ToString());
        }

        private static void server_DeleteDeviceCompleted(int devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }
    }
}
