﻿using System;
using PInvoke;

namespace DofusBuddy.Managers
{
    public class WindowManager
    {
        public WindowManager()
        {
        }

        public void SetForegroundWindow(IntPtr windowHandle)
        {
            IntPtr foregroundWindowHandle = User32.GetForegroundWindow();
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

        public void MaximizedWindow(IntPtr windowHandle)
        {
            User32.ShowWindow(windowHandle, User32.WindowShowStyle.SW_SHOWMAXIMIZED);
        }

        public void SendLeftClickToWindow(IntPtr windowHandle, int x, int y)
        {
            nint lParam = x | (y << 16);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(0x0001), lParam);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(0x0000), lParam);
        }

        public void SendLeftClickToWindow(IntPtr windowHandle, double xratio, double yratio)
        {
            var windowInfo = new User32.WINDOWINFO();
            User32.GetWindowInfo(windowHandle, ref windowInfo);

            int x = (int)((windowInfo.rcClient.right - windowInfo.rcClient.left) * xratio);
            int y = (int)((windowInfo.rcClient.bottom - windowInfo.rcClient.top) * yratio);

            SendLeftClickToWindow(windowHandle, x, y);
        }

        public void SendKeyToWindow(IntPtr windowHandle, int virtualKeyCode)
        {
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_KEYDOWN, (IntPtr)virtualKeyCode, IntPtr.Zero);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_KEYUP, (IntPtr)virtualKeyCode, IntPtr.Zero);
        }

        public User32.WINDOWINFO GetWindowInfo(IntPtr windowHandle)
        {
            var windowInfo = new User32.WINDOWINFO();
            User32.GetWindowInfo(windowHandle, ref windowInfo);
            return windowInfo;
        }
    }
}
