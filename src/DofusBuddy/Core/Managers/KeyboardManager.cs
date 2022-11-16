using PInvoke;

namespace DofusBuddy.Core.Managers
{
    public class KeyboardManager
    {
        public KeyboardManager()
        {
        }

        public void SendReturnKey()
        {
            var inputs = new User32.INPUT[]
            {
                new User32.INPUT
                {
                    type = User32.InputType.INPUT_KEYBOARD,
                    Inputs = new User32.INPUT.InputUnion
                    {
                        ki = new User32.KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = User32.ScanCode.RETURN,
                            dwFlags = User32.KEYEVENTF.KEYEVENTF_SCANCODE
                        }
                    }
                },
                new User32.INPUT
                {
                    type = User32.InputType.INPUT_KEYBOARD,
                    Inputs = new User32.INPUT.InputUnion
                    {
                        ki = new User32.KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = User32.ScanCode.RETURN,
                            dwFlags = User32.KEYEVENTF.KEYEVENTF_SCANCODE | User32.KEYEVENTF.KEYEVENTF_KEYUP
                        }
                    }
                }
            };

            unsafe
            {
                User32.SendInput(inputs.Length, inputs, sizeof(User32.INPUT));
            }
        }
    }
}
