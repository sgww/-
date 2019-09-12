using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 播放控制
{
    internal class ManageIni
    {
        private string sPath = null;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        public ManageIni(string path)
        {
            this.sPath = path;
        }
        public void WritValue(string section, string key, string value)
        {
            ManageIni.WritePrivateProfileString(section, key, value, this.sPath);
        }
        public string ReadValue(string section, string key)
        {
            StringBuilder stringBuilder = new StringBuilder(255);
            ManageIni.GetPrivateProfileString(section, key, "", stringBuilder, 255, this.sPath);
            return stringBuilder.ToString();
        }
    }
}
