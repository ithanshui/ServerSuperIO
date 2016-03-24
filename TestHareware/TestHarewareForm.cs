using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDevice
{
    public partial class TestHarewareForm : Form
    {
        private bool _IsNetConnect = false;

        private byte[] _ComBuffer = new byte[1024];
        private byte[] _NetBuffer = new byte[1024];

        private TcpClient _tcpClient = null;

        public TestHarewareForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private byte[] GetBackData()
        {
            byte[] backData = new byte[14];
            backData[0] = 0x55;
            backData[1] = 0xaa;//协议头
            backData[2] = 0x00;//从机地址
            backData[3] = 0x61;//命令

            Random rand = new Random();
            //模拟流量数据
            byte[] flow = BitConverter.GetBytes((float)(rand.NextDouble() * 1000));
            Array.Reverse(flow);

            backData[4] = flow[0];
            backData[5] = flow[1];
            backData[6] = flow[2];
            backData[7] = flow[3];

            //模拟信号数据
            byte[] signal = BitConverter.GetBytes((float)(rand.NextDouble() * 1000));
            Array.Reverse(signal);

            backData[8] = signal[0];
            backData[9] = signal[1];
            backData[10] = signal[2];
            backData[11] = signal[3];

            byte[] checkSum = new byte[10];
            Buffer.BlockCopy(backData, 2, checkSum, 0, checkSum.Length);

            backData[12] = (byte)checkSum.Sum(b => b);//计算校验和

            backData[13] = 0x0d;
            return backData;
        }

        private void WriteLog(string log)
        {
            if (listBox1.Items.Count >= 5000)
            {
                this.listBox1.Items.Clear();
            }

            this.listBox1.Items.Add(log);
            this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
        }

        private void btCom_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.serialPort1.IsOpen)
                {
                    this.serialPort1.PortName = this.cbbCom.Text;
                    this.serialPort1.BaudRate = Convert.ToInt32(this.cbbBaud.Text);
                    this.serialPort1.DataBits = 8;
                    this.serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    this.serialPort1.Parity = System.IO.Ports.Parity.None;
                    this.serialPort1.ReadBufferSize = 128;
                    this.serialPort1.ReceivedBytesThreshold = 1;
                    this.serialPort1.Open();
                    this.serialPort1.DiscardInBuffer();
                    this.serialPort1.DiscardOutBuffer();
                    this.btCom.Text = "关闭串口";
                    this.WriteLog("已经打开串口");
                }
                else
                {
                    this.serialPort1.Close();
                    this.btCom.Text = "打开串口";
                    this.WriteLog("已经关闭串口");
                }
            }
            catch (System.Exception ex)
            {
                this.WriteLog(ex.Message);
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int num = this.serialPort1.Read(_ComBuffer, 0, _ComBuffer.Length);
            if (num == 6
                && _ComBuffer[0] == 0x55
                && _ComBuffer[1] == 0xaa
                && _ComBuffer[3] == 0x61
                && _ComBuffer[num - 1] == 0x0d)
            {
                byte[] backData = GetBackData();
                this.serialPort1.Write(backData, 0, backData.Length);
                WriteLog("串口设备已经返回数据");
            }
            else
            {
                WriteLog("校验串口接收的数据失败");
            }
        }

        private void btNet_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this._IsNetConnect)
                {
                    this._tcpClient = new TcpClient();
                    this._tcpClient.Connect(this.cbbIP.Text, int.Parse(this.cbbPort.Text));
                    this._tcpClient.Client.BeginReceive(_NetBuffer, 0, _NetBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _tcpClient);
                    this._IsNetConnect = true;
                    this.btNet.Text = "断开网络";
                    WriteLog("连接到服务器");
                }
                else
                {
                    this.Disconnect();
                }
            }
            catch (SocketException ex)
            {
                WriteLog(ex.Message);
                this.Disconnect();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = (TcpClient)ar.AsyncState;

                if (_tcpClient != null && _tcpClient.Client != null)
                {
                    int read = client.Client.EndReceive(ar);

                    if (read > 0)
                    {
                        if (read == 6
                            && _NetBuffer[0] == 0x55
                            && _NetBuffer[1] == 0xaa
                            && _NetBuffer[3] == 0x61
                            && _NetBuffer[read - 1] == 0x0d)
                        {
                            byte[] backData = GetBackData();
                            client.Client.Send(backData);
                            WriteLog("网络设备已经返回数据");
                        }
                        else
                        {
                            WriteLog("校验网络接收的数据失败");
                        }

                        client.Client.BeginReceive(_NetBuffer, 0, _NetBuffer.Length, SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), client);
                    }
                    else
                    {
                        this.Disconnect();
                    }
                }
            }
            catch (SocketException ex)
            {
                WriteLog(ex.Message);
                this.Disconnect();
            }
        }

        private void Disconnect()
        {
            if (_tcpClient != null)
            {
                this._tcpClient.Close();
                this._tcpClient = null;
            }
            this._IsNetConnect = false;
            this.btNet.Text = "连接网络";
            WriteLog("断开了服务器");
            
        }
    }
}
