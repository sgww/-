using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace 播放控制
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private Thread KeepConnect;
        private Thread InceptMsg;
        private byte[] MsgBuf;
        public string ServerIP;
        public int ServerPort;
        private ManageIni ini = new ManageIni(Application.StartupPath + "\\Config.ini");
        private ManageIni iniT = new ManageIni(Application.StartupPath + "\\Config2.ini");
        private ManageIni iniD = new ManageIni(Application.StartupPath + "\\Config3.ini");
        private ManageIni ini0 = new ManageIni(Application.StartupPath + "\\ConfigA.ini");
        private ManageIni ini1 = new ManageIni(Application.StartupPath + "\\ConfigB.ini");
        private ManageIni ini2 = new ManageIni(Application.StartupPath + "\\ConfigC.ini");
        private ManageIni ini3 = new ManageIni(Application.StartupPath + "\\ConfigD.ini");
        private ManageIni ini4 = new ManageIni(Application.StartupPath + "\\ConfigE.ini");
        private ManageIni ini5 = new ManageIni(Application.StartupPath + "\\ConfigF.ini");
        private ManageIni ini6 = new ManageIni(Application.StartupPath + "\\ConfigG.ini");
        private ManageIni ini7 = new ManageIni(Application.StartupPath + "\\ConfigH.ini");
        private ManageIni ini8 = new ManageIni(Application.StartupPath + "\\ConfigI.ini");
        private ManageIni ini9 = new ManageIni(Application.StartupPath + "\\ConfigJ.ini");
        private ManageIni iniShoudong = new ManageIni(Application.StartupPath + "\\手动点配置.ini");
        private SerialPort sp = new SerialPort();
        public string CD;
        private KeyBordHook KBH;
        private string[] IniText;
        private string[] times;
        private Thread THtime;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                this.IniText = File.ReadAllLines(Application.StartupPath + "\\Config.ini", Encoding.Default);
                this.times = new string[this.IniText.Length];
                int num = 0;
                for (int i = 0; i < this.IniText.Length; i++)
                {
                    if (this.IniText[i] != "")
                    {
                        this.IniText[i] = this.IniText[i].Replace("[", "");
                        this.IniText[i] = this.IniText[i].Replace("]", "");
                        if (this.IniText[i].LastIndexOf(":") > 0)
                        {
                            int num2 = 0;
                            if (num2 < this.IniText.Length)
                            {
                                this.times[num] = this.IniText[i];
                                num++;
                            }
                        }
                    }
                }
                this.ServerIP = this.ini.ReadValue("控制端IP地址", "IP");
                this.ServerPort = 3000;
                this.KeepConnect = new Thread(new ThreadStart(this.keepmethod));
                this.KeepConnect.SetApartmentState(ApartmentState.STA);
                this.KeepConnect.Start();
                this.InceptMsg = new Thread(new ThreadStart(this.inceptmethod));
                this.InceptMsg.SetApartmentState(ApartmentState.STA);
                this.InceptMsg.Start();
                this.sp.PortName = this.ini.ReadValue("灯光com", "Com");
                this.sp.Open();
            }
            catch
            {

            }
            
        }


        [DllImport("USER32.DLL")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public void keepmethod()
        {           
            while (true)
            {
                Thread.Sleep(200);
                try
                {
                    this.client = new TcpClient();
                    this.client.Connect(new IPEndPoint(IPAddress.Parse(this.ServerIP), this.ServerPort));
                    this.client.Client.Send(Encoding.UTF8.GetBytes("con"));

                    //回传系统声音设置
                    //this.client.Client.Send(Encoding.UTF8.GetBytes("voice" + SystemVolume.GetMasterVolume().ToString()));
                }
                catch (Exception e)
                {
                    try
                    {
                        this.client = new TcpClient();
                        this.client.Connect(new IPEndPoint(IPAddress.Parse(this.ServerIP), this.ServerPort));
                    }
                    catch
                    {
                    }
                }
            }
        }

        //控制client端
        public void inceptmethod()
        {
            while (true)
            {
                Thread.Sleep(20);
                try
                {
                    if (this.client.Client != null && this.client.Client.Connected)
                    {
                        this.MsgBuf = new byte[50];
                        this.client.Client.Receive(this.MsgBuf);
                        string @string = Encoding.UTF8.GetString(this.MsgBuf);
                        
                        this.MsgMethod(@string);
                    }
                }
                catch
                {
                }
            }
        }

        public void MsgMethod(string msg)
        {
            try
            {
                msg = msg.Remove(msg.IndexOf("\0"));
                if (!(msg == "") && msg.IndexOf("connect") <= -1)
                {
                    string text = msg;                   
                    if (text != null)
                    {      
                        if (!(text == "play1"))
                        {
                            if (text == "stop")
                            {
                                try
                                {
                                    this.THtime.Abort();
                                }
                                catch
                                {
                                }
                                this.sp.Write(this.GetByteData(this.ini.ReadValue("灯光关", "cmd")), 0, this.GetByteData(this.ini.ReadValue("灯光关", "cmd")).Length);
                            }

                      
                            if (text == "play2")
                            {
                                try
                                {
                                    this.THtime.Abort();
                                }
                                catch
                                {
                                }
                                this.THtime = new Thread(new ThreadStart(this.TS_time));
                                this.THtime.SetApartmentState(ApartmentState.STA);
                                this.THtime.Start();
                            }
                            if (text == "play3")
                            {
                                this.IniText = File.ReadAllLines(Application.StartupPath + "\\Config2.ini", Encoding.Default);
                                this.times = new string[this.IniText.Length];
                                int num = 0;
                                for (int i = 0; i < this.IniText.Length; i++)
                                {
                                    if (this.IniText[i] != "")
                                    {
                                        this.IniText[i] = this.IniText[i].Replace("[", "");
                                        this.IniText[i] = this.IniText[i].Replace("]", "");
                                        if (this.IniText[i].LastIndexOf(":") > 0)
                                        {
                                            int num2 = 0;
                                            if (num2 < this.IniText.Length)
                                            {
                                                this.times[num] = this.IniText[i];
                                                num++;
                                            }
                                        }
                                    }
                                }
                                try
                                {
                                    this.THtime.Abort();
                                }
                                catch
                                {
                                }
                                this.THtime = new Thread(new ThreadStart(this.TS_timeT));
                                this.THtime.SetApartmentState(ApartmentState.STA);
                                this.THtime.Start();
                            }
                            if (text == "play4")
                            {
                                this.IniText = File.ReadAllLines(Application.StartupPath + "\\Config3.ini", Encoding.Default);
                                this.times = new string[this.IniText.Length];
                                int num = 0;
                                for (int i = 0; i < this.IniText.Length; i++)
                                {
                                    if (this.IniText[i] != "")
                                    {
                                        this.IniText[i] = this.IniText[i].Replace("[", "");
                                        this.IniText[i] = this.IniText[i].Replace("]", "");
                                        if (this.IniText[i].LastIndexOf(":") > 0)
                                        {
                                            int num2 = 0;
                                            if (num2 < this.IniText.Length)
                                            {
                                                this.times[num] = this.IniText[i];
                                                num++;
                                            }
                                        }
                                    }
                                }
                                try
                                {
                                    this.THtime.Abort();
                                }
                                catch
                                {
                                }
                                this.THtime = new Thread(new ThreadStart(this.TS_timeD));
                                this.THtime.SetApartmentState(ApartmentState.STA);
                                this.THtime.Start();
                            }
                            MorePlay(text);

                        }
                        else
                        {
                            try
                            {
                                this.THtime.Abort();
                            }
                            catch
                            {
                            }
                            this.THtime = new Thread(new ThreadStart(this.TS_time));
                            this.THtime.SetApartmentState(ApartmentState.STA);
                            this.THtime.Start();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }
        private void MorePlay(string text2)
        {
            try
            {
                switch (text2)
                {
                    case "0":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigA.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int num = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[num] = this.IniText[i];
                                        num++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time0));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "1":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigB.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int numB = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[numB] = this.IniText[i];
                                        numB++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time1));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "2":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigC.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int numC = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[numC] = this.IniText[i];
                                        numC++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time2));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "3":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigD.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int numD = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[numD] = this.IniText[i];
                                        numD++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time3));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "4":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigE.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int numE = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[numE] = this.IniText[i];
                                        numE++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time4));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "5":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigF.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int numF = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[numF] = this.IniText[i];
                                        numF++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time5));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "6":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigG.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int numG = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[numG] = this.IniText[i];
                                        numG++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time6));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "7":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigH.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        int numH = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[numH] = this.IniText[i];
                                        numH++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time7));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "8":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigI.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        num = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[num] = this.IniText[i];
                                        num++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time8));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "9":
                        this.IniText = File.ReadAllLines(Application.StartupPath + "\\ConfigJ.ini", Encoding.Default);
                        this.times = new string[this.IniText.Length];
                        num = 0;
                        for (int i = 0; i < this.IniText.Length; i++)
                        {
                            if (this.IniText[i] != "")
                            {
                                this.IniText[i] = this.IniText[i].Replace("[", "");
                                this.IniText[i] = this.IniText[i].Replace("]", "");
                                if (this.IniText[i].LastIndexOf(":") > 0)
                                {
                                    int num2 = 0;
                                    if (num2 < this.IniText.Length)
                                    {
                                        this.times[num] = this.IniText[i];
                                        num++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            this.THtime.Abort();
                        }
                        catch
                        {
                        }
                        this.THtime = new Thread(new ThreadStart(this.TS_time9));
                        this.THtime.SetApartmentState(ApartmentState.STA);
                        this.THtime.Start();
                        break;
                    case "10":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点11名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点11名称", "name"), "cmd")).Length);
                        break;
                    case "11":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点12名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点12名称", "name"), "cmd")).Length);
                        break;
                    case "M":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点13名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点13名称", "name"), "cmd")).Length);
                        break;
                    case "N":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点14名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点14名称", "name"), "cmd")).Length);
                        break;
                    case "O":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点15名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点15名称", "name"), "cmd")).Length);
                        break;
                    case "P":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点16名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点16名称", "name"), "cmd")).Length);
                        break;
                    case "Q":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点17名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点17名称", "name"), "cmd")).Length);
                        break;
                    case "R":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点18名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点18名称", "name"), "cmd")).Length);
                        break;
                    case "S":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点19名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点19名称", "name"), "cmd")).Length);
                        break;
                    case "T":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点20名称", "name"), "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.iniShoudong.ReadValue("手动点20名称", "name"), "cmd")).Length);
                        break;
                    case "2V":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue("灯泡开", "cmd")), 0, this.GetByteData(this.ini.ReadValue("灯泡开", "cmd")).Length);
                        break;
                    case "2W":
                        this.sp.Write(this.GetByteData(this.ini.ReadValue("灯泡关", "cmd")), 0, this.GetByteData(this.ini.ReadValue("灯泡关", "cmd")).Length);
                        break;
                }
            }
            catch
            {
            }
        }
        public void TS_time()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_timeT()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.iniT.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.iniT.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_timeD()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.iniD.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.iniD.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time0()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini0.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini0.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time1()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini1.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini1.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time2()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini2.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini2.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time3()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini3.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini3.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time4()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini4.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini4.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time5()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini5.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini5.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time6()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini6.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini6.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time7()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini7.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini7.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time8()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini8.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini8.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public void TS_time9()
        {
            //灯光从1秒开始
            Thread.Sleep(1000);
            int num = 1;
            int num2 = 0;
            try
            {
                while (true)
                {
                    int num3 = num / 3600;
                    int num4 = (num - 3600 * num3) / 60;
                    int num5 = (num - 3600 * num3) % 60;
                    string a = num4.ToString().PadLeft(2, '0') + ":" + num5.ToString().PadLeft(2, '0');
                    if (a == this.times[num2])
                    {
                        this.sp.Write(this.GetByteData(this.ini9.ReadValue(this.times[num2], "cmd")), 0, this.GetByteData(this.ini9.ReadValue(this.times[num2], "cmd")).Length);
                        num2++;
                    }
                    Thread.Sleep(1000);

                    num++;
                }
            }
            catch
            {
            }
        }
        public byte[] GetByteData(string cmd)
        {
            byte[] array = null;
            if (cmd.IndexOf(' ') > -1)
            {
                string[] array2 = cmd.Split(new char[]
                {
                    ' '
                }, StringSplitOptions.RemoveEmptyEntries);
                array = new byte[array2.Length];
                for (int i = 0; i < array2.Length; i++)
                {
                    array[i] = (byte)Convert.ToInt32(array2[i].Trim(), 16);
                }
            }
            return array;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            MP1.Width = this.Width;
            MP1.Height = this.Height;
            MP1.Left = 0;
            MP1.Top = 0;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);          
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //this.WindowState = FormWindowState.Minimized;
            //this.Hide();
            //Environment.Exit(0);
        }

       


        //线程内添加委托
        private delegate void changeform();
        private void changef()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            this.Show();
        }

        private void Hid()
        {
            this.WindowState = FormWindowState.Minimized;
           // this.Hide();
        }

        private void SetTimer()
        {
            this.timer1.Start();
        }

        //关闭网页
        public static void KillProcess(string strProcessesByName)//关闭线程
        {
            foreach (Process p in Process.GetProcesses())//GetProcessesByName(strProcessesByName))
            {
                if (p.ProcessName.ToUpper().Contains(strProcessesByName))
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(); // possibly with a timeout
                    }
                    catch (Win32Exception e)
                    {
                        MessageBox.Show(e.Message.ToString());   // process was terminating or can't be terminated - deal with it
                    }
                    catch (InvalidOperationException e)
                    {
                        MessageBox.Show(e.Message.ToString()); // process has already exited - might be able to let this one go
                    }
                }
            }
        }

        //播放完毕最小化
        private void MP1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if ((int)MP1.playState == 1)
            {             
               Invoke(new changeform(Hid));                                        
            }
        }
    }
}
