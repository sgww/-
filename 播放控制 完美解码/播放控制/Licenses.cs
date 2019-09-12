using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 播放控制
{
    class Licenses
    {
        public static bool Getli()
        {
            try
            {
                string result = System.IO.File.ReadAllText(Application.StartupPath + "\\license.licx");
                string decrp = Encryption.Decrypt(result);
                string[] info = decrp.Split(',');
                string macb = info[0];
                string sy = info[1];
                string Ip = info[2];
                string dateTime = info[3];
                DateTime dt = Convert.ToDateTime(dateTime);

                string mac = ComputerInfo.getMacAddr_Local();
                if (mac != macb)
                {
                    MessageBox.Show("软件未授权！请联系管理员");
                    return false;
                }

                //string date = GetNetDateTime();
                //DateTime dt2 = Convert.ToDateTime(date).AddDays(-1);
                //if (dt < dt2)
                //{
                //    MessageBox.Show("软件授权信息已过期");
                //    return false;
                //}

                if (sy != "sgw")
                {
                    MessageBox.Show("非授权软件！");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("软件未授权！请联系管理员");
                return false;
            }
        }

        public static string GetNetDateTime()
        {//获取网络时间
            WebRequest request = null;
            WebResponse response = null;
            WebHeaderCollection headerCollection = null;
            string datetime = string.Empty;
            try
            {
                request = WebRequest.Create("https://www.baidu.com");
                request.Timeout = 3000;
                request.Credentials = CredentialCache.DefaultCredentials;
                response = request.GetResponse();
                headerCollection = response.Headers;
                foreach (var h in headerCollection.AllKeys)
                {
                    if (h == "Date")
                    {
                        datetime = headerCollection[h];
                    }
                }
                return datetime;
            }
            catch (Exception) { return datetime; }
            finally
            {
                if (request != null)
                { request.Abort(); }
                if (response != null)
                { response.Close(); }
                if (headerCollection != null)
                { headerCollection.Clear(); }
            }
        }
    }
}
