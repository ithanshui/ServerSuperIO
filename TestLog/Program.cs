using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLog
{
    class Program
    {
        static void Main(string[] args)
        {
            new ServerSuperIO.Log.LogFactory().GetLog("ddd").Debug(true,"ddd");
            Console.Read();
        }
    }
}
