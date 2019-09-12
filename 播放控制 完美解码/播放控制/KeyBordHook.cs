using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 播放控制
{
    class KeyBordHook
    {
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private const int WM_SYSKEYDOWN = 260;
        private const int WM_SYSKEYUP = 261;
        public const int WH_KEYBOARD_LL = 13;
        private static int hKeyboardHook = 0;
        private KeyBordHook.HookProc KeyboardHookProcedure;
        public event KeyEventHandler OnKeyDownEvent;
        public event KeyEventHandler OnKeyUpEvent;
        public event KeyPressEventHandler OnKeyPressEvent;
        [DllImport("user32.dll ", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int SetWindowsHookEx(int idHook, KeyBordHook.HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll ", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll ", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        [DllImport("user32 ")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
        [DllImport("user32 ")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        public KeyBordHook()
        {
            this.Start();
        }
        ~KeyBordHook()
        {
            this.Stop();
        }
        public void Start()
        {
            if (KeyBordHook.hKeyboardHook == 0)
            {
                this.KeyboardHookProcedure = new KeyBordHook.HookProc(this.KeyboardHookProc);
                Process currentProcess = Process.GetCurrentProcess();
                ProcessModule mainModule = currentProcess.MainModule;
                KeyBordHook.hKeyboardHook = KeyBordHook.SetWindowsHookEx(13, this.KeyboardHookProcedure, KeyBordHook.GetModuleHandle(mainModule.ModuleName), 0);
                if (KeyBordHook.hKeyboardHook == 0)
                {
                    this.Stop();
                }
            }
        }
        public void Stop()
        {
            bool flag = true;
            if (KeyBordHook.hKeyboardHook != 0)
            {
                flag = KeyBordHook.UnhookWindowsHookEx(KeyBordHook.hKeyboardHook);
                KeyBordHook.hKeyboardHook = 0;
            }
            if (!flag)
            {
                throw new Exception("UnhookWindowsHookEx   failed. ");
            }
        }
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (this.OnKeyDownEvent != null || this.OnKeyUpEvent != null || this.OnKeyPressEvent != null))
            {
                KeyBordHook.KeyboardHookStruct keyboardHookStruct = (KeyBordHook.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBordHook.KeyboardHookStruct));
                if (this.OnKeyDownEvent != null && (wParam == 256 || wParam == 260))
                {
                    Keys vkCode = (Keys)keyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(vkCode);
                    this.OnKeyDownEvent(this, e);
                }
                if (this.OnKeyPressEvent != null && wParam == 256)
                {
                    byte[] array = new byte[256];
                    KeyBordHook.GetKeyboardState(array);
                    byte[] array2 = new byte[2];
                    if (KeyBordHook.ToAscii(keyboardHookStruct.vkCode, keyboardHookStruct.scanCode, array, array2, keyboardHookStruct.flags) == 1)
                    {
                        KeyPressEventArgs e2 = new KeyPressEventArgs((char)array2[0]);
                        this.OnKeyPressEvent(this, e2);
                    }
                }
                if (this.OnKeyUpEvent != null && (wParam == 257 || wParam == 261))
                {
                    Keys vkCode = (Keys)keyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(vkCode);
                    this.OnKeyUpEvent(this, e);
                }
            }
            return KeyBordHook.CallNextHookEx(KeyBordHook.hKeyboardHook, nCode, wParam, lParam);
        }
    }
}
