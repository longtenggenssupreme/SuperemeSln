using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("启动测试：");
            try
            {
                WebTunnelProvider.Instance.InitialAcceptor(8084);
            }
            catch
            {
                throw;
            }
            Console.WriteLine("按Enter键退出。。。。。。");
            var end = Console.ReadLine(); 
            //if (end.Equals(end))
            {
                WebTunnelProvider.Instance.AbortManage();
            }
        }
    }
}
