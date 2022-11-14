using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using PInvoke;

namespace DofusBuddy.Core
{
    public class WindowManager
    {
        public WindowManager()
        {
        }

        /// <summary>
        /// Mandatory to avoid issues regarding User32.SetForegroundWindow limits
        /// see https://learn.microsoft.com/en-gb/windows/win32/api/winuser/nf-winuser-setforegroundwindow?redirectedfrom=MSDN#remarks
        /// </summary>
        public void AttachThreadInput(IntPtr windowHandle)
        {
            int windowThreadProcessId = User32.GetWindowThreadProcessId(windowHandle, out _);
            int currentThreadId = Kernel32.GetCurrentThreadId();
            User32.AttachThreadInput(windowThreadProcessId, currentThreadId, true);
        }

        public void SetForegroundWindow(IntPtr windowHandle)
        {
            if (windowHandle != User32.GetForegroundWindow())
            {
                User32.SetForegroundWindow(windowHandle);
            }
        }

        public string GetCharacterNameFromProcessWindowTitle(Process process)
        {
            var regex = new Regex("(.*?) \\- ");
            return regex.Match(process.MainWindowTitle)
                .Groups[1]
                .Value;
        }

        public void SendLeftClickToWindow(IntPtr windowHandle, int x, int y)
        {
            var lParam = (IntPtr)(x | (y << 16));
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(0x0001), lParam);
            User32.SendMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(0x0000), lParam);
        }
    }
}
