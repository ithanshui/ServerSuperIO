using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Device;

namespace TestDeviceDriver
{
    internal class DeviceCommand:ProtocolCommand
    {
        public override string Name
        {
            get { return "61"; }
        }

        public override object Analysis(byte[] data, object obj)
        {
            Dyn dyn = new Dyn
            {
                CurDT = DateTime.Now,
                ProHead = this.ProtocolDriver.GetProHead(data),
                DeviceAddr = this.ProtocolDriver.GetAddress(data),
                Command = this.ProtocolDriver.GetCommand(data),
                ProEnd = this.ProtocolDriver.GetProEnd(data)
            };

            //一般下位机是单片的话，接收到数据的高低位需要互换，才能正常解析。
            byte[] flow = BinaryUtil.SubBytes(data, 4, 4, true);
            dyn.Flow = BitConverter.ToSingle(flow, 0);
            byte[] signal = BinaryUtil.SubBytes(data, 8, 4, true);
            dyn.Signal = BitConverter.ToSingle(signal, 0);
            return dyn;
        }

        public override byte[] Package(int devaddr, object obj)
        {
            //发送：0x55 0xaa 0x00 0x61 0x61 0x0d
            byte[] data = new byte[6];
            data[0] = 0x55;
            data[1] = 0xaa;
            data[2] = (byte)devaddr;
            data[3] = 0x61;
            data[4] = this.ProtocolDriver.GetCheckData(data)[0];
            data[5] = 0x0d;
            return data;
        }
    }
}
