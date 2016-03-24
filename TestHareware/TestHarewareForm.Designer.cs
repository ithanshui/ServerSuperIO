namespace TestDevice
{
    partial class TestHarewareForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btCom = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbbBaud = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbCom = new System.Windows.Forms.ComboBox();
            this.btNet = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbbPort = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbbIP = new System.Windows.Forms.ComboBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.SuspendLayout();
            // 
            // btCom
            // 
            this.btCom.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btCom.Location = new System.Drawing.Point(367, 11);
            this.btCom.Name = "btCom";
            this.btCom.Size = new System.Drawing.Size(84, 26);
            this.btCom.TabIndex = 33;
            this.btCom.Text = "打开串口";
            this.btCom.UseVisualStyleBackColor = true;
            this.btCom.Click += new System.EventHandler(this.btCom_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(180, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 32;
            this.label2.Text = "波特率";
            // 
            // cbbBaud
            // 
            this.cbbBaud.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbBaud.FormattingEnabled = true;
            this.cbbBaud.Items.AddRange(new object[] {
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600"});
            this.cbbBaud.Location = new System.Drawing.Point(241, 11);
            this.cbbBaud.Name = "cbbBaud";
            this.cbbBaud.Size = new System.Drawing.Size(120, 24);
            this.cbbBaud.TabIndex = 31;
            this.cbbBaud.Text = "9600";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 30;
            this.label1.Text = "串口";
            // 
            // cbbCom
            // 
            this.cbbCom.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbCom.FormattingEnabled = true;
            this.cbbCom.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12"});
            this.cbbCom.Location = new System.Drawing.Point(54, 11);
            this.cbbCom.Name = "cbbCom";
            this.cbbCom.Size = new System.Drawing.Size(120, 24);
            this.cbbCom.TabIndex = 29;
            this.cbbCom.Text = "COM1";
            // 
            // btNet
            // 
            this.btNet.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btNet.Location = new System.Drawing.Point(367, 43);
            this.btNet.Name = "btNet";
            this.btNet.Size = new System.Drawing.Size(84, 26);
            this.btNet.TabIndex = 38;
            this.btNet.Text = "连接网络";
            this.btNet.UseVisualStyleBackColor = true;
            this.btNet.Click += new System.EventHandler(this.btNet_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(180, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 37;
            this.label3.Text = "Port";
            // 
            // cbbPort
            // 
            this.cbbPort.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbPort.FormattingEnabled = true;
            this.cbbPort.Items.AddRange(new object[] {
            "6688"});
            this.cbbPort.Location = new System.Drawing.Point(241, 43);
            this.cbbPort.Name = "cbbPort";
            this.cbbPort.Size = new System.Drawing.Size(120, 24);
            this.cbbPort.TabIndex = 36;
            this.cbbPort.Text = "6688";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(8, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 16);
            this.label4.TabIndex = 35;
            this.label4.Text = "IP";
            // 
            // cbbIP
            // 
            this.cbbIP.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbIP.FormattingEnabled = true;
            this.cbbIP.Items.AddRange(new object[] {
            "127.0.0.1"});
            this.cbbIP.Location = new System.Drawing.Point(54, 43);
            this.cbbIP.Name = "cbbIP";
            this.cbbIP.Size = new System.Drawing.Size(120, 24);
            this.cbbIP.TabIndex = 34;
            this.cbbIP.Text = "127.0.0.1";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBox1.Font = new System.Drawing.Font("Rod", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(0, 81);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(458, 324);
            this.listBox1.TabIndex = 39;
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // TestHarewareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 405);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btNet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbbPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbbIP);
            this.Controls.Add(this.btCom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbbBaud);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbbCom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestHarewareForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模拟终端设备";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btCom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbbBaud;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbCom;
        private System.Windows.Forms.Button btNet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbbPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbbIP;
        private System.Windows.Forms.ListBox listBox1;
        private System.IO.Ports.SerialPort serialPort1;
    }
}

