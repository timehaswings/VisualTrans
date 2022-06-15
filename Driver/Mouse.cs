using System.Windows.Forms;
using WindowsInput;

namespace VisualTrans.Driver
{
    class Mouse
    {
        private readonly InputSimulator simulator;
        private readonly int MAX_INT_16 = 65535;

        public Mouse() {
            simulator = new InputSimulator();
        }

        public void Move(double x, double y)
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            var rx = x * MAX_INT_16 / bounds.Width;
            var ry = y * MAX_INT_16 / bounds.Height;
            simulator.Mouse.MoveMouseTo(rx, ry);
        }

        public void LeftButtonDown()
        {
            simulator.Mouse.LeftButtonDown();
        }

        public void LeftButtonUp()
        {
            simulator.Mouse.LeftButtonUp();
        }

        public void RightButtonDown()
        {
            simulator.Mouse.RightButtonDown();
        }

        public void RightButtonUp()
        {
            simulator.Mouse.RightButtonUp();
        }

        public void LeftClick()
        {
            simulator.Mouse.LeftButtonClick();
        }

        public void RightClick()
        {
            simulator.Mouse.RightButtonClick();
        }

        public void MiddleButtonDown()
        {
            simulator.Mouse.MiddleButtonDown();
        }

        public void MiddleButtonUp()
        {
            simulator.Mouse.MiddleButtonUp();
        }

        public void Scroll(int step)
        {
            simulator.Mouse.VerticalScroll(step);
        }
    }
}
