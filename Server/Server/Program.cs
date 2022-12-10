
using Server.Helper;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //5.1在这里我们调用一下
            TCPServer tCPServer = new TCPServer();
            tCPServer.Start();                  //5.2调用Start方法，这样就启用这个服务端了
            while (true)                        //7.1防跳出的操作
            {
                var str = Console.ReadLine();
                //tCPServer.tempClient.Send(Encoding.UTF8.GetBytes($"测试主动发送数据：{str}"));   	//new 如果想主动给客户端发消息，我们可以调用tCPServer

                //var jsonStr = JsonHelper.GetTestToString();                       //2.5测试将测试的数据在这里拿到，在下面一行通过Send发送给客户端
                //tCPServer.tempClient.Send(Encoding.UTF8.GetBytes(jsonStr));       //2.6将Json数据发送给客户端
             
                
            }                                   //7.2目前我们还没写客户端，没办法测试消失的接收和发送。下一站写客户端
        }
    }
}
