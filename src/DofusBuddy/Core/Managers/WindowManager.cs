using System;
using PInvoke;

namespace DofusBuddy.Core.Managers
{
    public class WindowManager
    {
        public WindowManager()
        {
        }

        public void SetForegroundWindow(IntPtr windowHandle)
        {
            IntPtr foregroundWindowHandle = User32.GetForegroundWindow();
            if (windowHandle != foregroundWindowHandle)
            {
                int foregroundWindowThreadProcessId = User32.GetWindowThreadProcessId(foregroundWindowHandle, out _);
                int currentThreadId = Kernel32.GetCurrentThreadId();

                // Mandatory to avoid issues regarding User32.SetForegroundWindow limits
                // see https://learn.microsoft.com/en-gb/windows/win32/api/winuser/nf-winuser-setforegroundwindow?redirectedfrom=MSDN#remarks
                // https://stackoverflow.com/questions/19136365/win32-setforegroundwindow-not-working-all-the-time
                User32.AttachThreadInput(foregroundWindowThreadProcessId, currentThreadId, true);

                User32.BringWindowToTop(windowHandle);
                User32.ShowWindow(windowHandle, User32.WindowShowStyle.SW_SHOWMAXIMIZED);

                // Detach the 2 threads to avoid other potential issues
                User32.AttachThreadInput(foregroundWindowThreadProcessId, currentThreadId, false);
            }
        }

        public void SendLeftClickToWindow(IntPtr windowHandle, int x, int y)
        {
            var lParam = (IntPtr)(x | (y << 16));
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(0x0001), lParam);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(0x0000), lParam);
        }

        public User32.WINDOWINFO GetWindowInfo(IntPtr windowHandle)
        {
            var windowInfo = new User32.WINDOWINFO();
            User32.GetWindowInfo(windowHandle, ref windowInfo);
            return windowInfo;
        }
    }
}
