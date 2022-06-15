using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using VisualTrans.Driver;

namespace VisualTrans.Module
{
    class ActionModule: WebApiController
    {
        private readonly Keyboard keyboard;
        private readonly Mouse mouse;

        public ActionModule() {
            keyboard = new Keyboard();
            mouse = new Mouse();
        }

        [Route(HttpVerbs.Get, "/action/left/click")]
        public object LeftClick()
        {
            mouse.LeftClick();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/right/click")]
        public object RightClick()
        {
            mouse.RightClick();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/move/{x}/{y}")]
        public object Move(int x, int y)
        {
            mouse.Move(x, y);
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/left/down")]
        public object LeftDown()
        {
            mouse.LeftButtonDown();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/left/up")]
        public object LeftUp()
        {
            mouse.LeftButtonUp();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/right/down")]
        public object RightDown()
        {
            mouse.RightButtonDown();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/right/up")]
        public object RightUp()
        {
            mouse.RightButtonUp();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/middle/down")]
        public object MiddleDown()
        {
            mouse.MiddleButtonDown();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/middle/up")]
        public object MiddleUp()
        {
            mouse.MiddleButtonUp();
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/scroll/{step}")]
        public object Scroll(int step)
        {
            mouse.Scroll(step);
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/keydown/{code}")]
        public object KeyDown(int code)
        {
            keyboard.KeyDown(code);
            return new { success = true };
        }

        [Route(HttpVerbs.Get, "/action/keyup/{code}")]
        public object KeyUp(int code)
        {
            keyboard.KeyUp(code);
            return new { success = true };
        }
    }
}
