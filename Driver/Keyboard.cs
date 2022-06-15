using System;
using WindowsInput;
using WindowsInput.Native;

namespace VisualTrans.Driver
{
    class Keyboard
    {
        private readonly InputSimulator simulator;

        public Keyboard() {
            simulator = new InputSimulator();
        }

        private VirtualKeyCode GetVirtualKeyCode(int keycode)
        {
            foreach (int v in Enum.GetValues(typeof(VirtualKeyCode)))
            {
                if (v == keycode)
                {
                    VirtualKeyCode code = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), Convert.ToString(v));
                    return code;
                }
            }
            return VirtualKeyCode.LBUTTON;
        }

        public void KeyDownAndUp(int keycode)
        {
            VirtualKeyCode code = GetVirtualKeyCode(keycode);
            simulator.Keyboard.KeyDown(code);
            simulator.Keyboard.KeyUp(code);
        }

        public void KeyDown(int keycode)
        {
            VirtualKeyCode code = GetVirtualKeyCode(keycode);
            simulator.Keyboard.KeyDown(code);
        }

        public void KeyUp(int keycode)
        {
            VirtualKeyCode code = GetVirtualKeyCode(keycode);
            simulator.Keyboard.KeyUp(code);
        }

        public void KeyPress(int keycode)
        {
            VirtualKeyCode code = GetVirtualKeyCode(keycode);
            simulator.Keyboard.KeyPress(code);
        }
    }
}
