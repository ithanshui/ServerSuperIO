using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Common
{
    public static class FileUtil
    {
        /// <summary>
        /// 追加内容到指定文件中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteAppend(string filePath, string content)
        {
            WriteAppend(filePath,new string[]{content});
        }

        public static void WriteAppend(string filePath, string[] contents)
        {
            //System.IO.StreamWriter sr = new System.IO.StreamWriter(filePath, true);
            //foreach (string c in contents)
            //{
            //    sr.WriteLine(c);
            //}
            //sr.Flush();
            //sr.Close();

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Seek(fs.Length, SeekOrigin.Current);

                string content = String.Join(Environment.NewLine, contents) + Environment.NewLine;

                byte[] data = System.Text.Encoding.UTF8.GetBytes(content);

                fs.Write(data, 0, data.Length);

                fs.Close();
            }
        }
    }
}
