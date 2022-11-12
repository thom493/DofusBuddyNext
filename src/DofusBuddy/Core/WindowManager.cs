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
    }
}
