using PInvoke;

namespace DofusBuddy.Managers
{
    public class KeyboardManager
    {
        public KeyboardManager()
        {
        }

        public void SendSingleKeyPress(User32.ScanCode scanCode)
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
                            wScan = scanCode,
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
                            wScan = scanCode,
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
