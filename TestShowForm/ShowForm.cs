using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestShowForm
{
    public partial class ShowForm : Form
    {
        private Action<string> _action;
        public ShowForm()
        {
            InitializeComponent();

            _action = new Action<string>(Update);
        }

        public void Update(string content)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(_action, new object[] {content});
            }
            else
            {
                this.listBox1.Items.Add(content);
                this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
                this.listBox1.Update();
            }
        }
    }
}
