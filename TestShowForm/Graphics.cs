using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Show;
using System.Windows.Forms;

namespace TestShowForm
{
    public class Graphics:GraphicsShow
    {
        private ShowForm _showForm;

        public Graphics()
        {
            _showForm=new ShowForm();
            _showForm.FormClosed+=showForm_FormClosed;
        }

        private void showForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
           this.OnGraphicsShowClosed(this.ThisKey);
        }

        public override string ThisKey
        {
            get { return "显示视图"; }
        }

        public override string ThisName
        {
            get { return "显示视图"; }
        }

        public override void Show()
        {
            Application.Run(_showForm);
        }

        public override void Close()
        {
            _showForm.Close();
        }

        public override void UpdateDevice(int devid, object obj)
        {
            string[] arr = (string[]) obj;
            string content = String.Format("{0}:流量>>{1},信号>>{2}", arr[0], arr[1], arr[2]);
            _showForm.Update(content);
        }

        public override void RemoveDevice(int devid)
        {
            //不操作
        }

        public override void Dispose()
        {
            _showForm.Dispose();
        }


        public override bool IsDisposed
        {
            get { return _showForm.IsDisposed; }
        }
    }
}
