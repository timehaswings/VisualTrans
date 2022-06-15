using System;
using System.Security.Principal;

namespace VisualTrans
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!IsAdministrator()) {
                Console.WriteLine("程序没有以管理员身份运行，这可能会导致出现某些异常，仍然要继续吗？");
                Console.WriteLine("输入y继续，输入其它任意键退出：");
                string key = Console.ReadKey(true).Key.ToString().ToLower();
                if ("y" != key)
                {
                    return;
                }
            }
            Server server = new Server();
            if (args.Length > 0)
            {
                server.Url = args[0];
            }
            using var webServer = server.CreateWebServer();
            webServer.RunAsync();
            Console.ReadKey(true);
        }

        static bool IsAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}
