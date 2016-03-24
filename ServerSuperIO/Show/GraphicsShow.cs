using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Show
{
    public abstract class GraphicsShow:IGraphicsShow
    {
        protected GraphicsShow()
        {
            
        }

        public abstract string ThisKey { get; }

        public abstract string ThisName { get; }

        public abstract void Show();

        public abstract void Close();

        public abstract void UpdateDevice(int devid, object obj);

        public abstract void RemoveDevice(int devid);

        public event GraphicsShowClosedHandler GraphicsShowClosed;

        protected void OnGraphicsShowClosed(object key)
        {
            if (GraphicsShowClosed != null)
            {
                GraphicsShowClosed(key);
            }
        }

        public event MouseRightContextMenuHandler MouseRightContextMenu;

        protected void OnMouseRightContextMenu(int devid)
        {
            if (MouseRightContextMenu != null)
            {
                MouseRightContextMenu(devid);
            }
        }

        public abstract bool IsDisposed { get;  }

        public abstract void Dispose();
    }
}
