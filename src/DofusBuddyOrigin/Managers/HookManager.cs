﻿using Gma.System.MouseKeyHook;
using SharpHook;

namespace DofusBuddy.Managers
{
    /// <summary>
    /// - Gma.System.MouseKeyHook to setup keyboards hooks
    /// - SharpHook to setup mouse hooks
    /// </summary>
    public class HookManager
    {
        public IKeyboardMouseEvents KeyboardMouseEvents { get; }

        public TaskPoolGlobalHook GlobalHook { get; }

        public HookManager()
        {
            KeyboardMouseEvents = Hook.GlobalEvents();
            GlobalHook = new TaskPoolGlobalHook(new TaskPoolGlobalHookOptions(true));
            GlobalHook.RunAsync();
        }
    }
}
