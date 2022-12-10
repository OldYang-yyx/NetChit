using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Helper;
using Server.Net;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TCPServer tCPServer = new TCPServer();
            tCPServer.Start();

            while (true)
            {
                var str = Console.ReadLine();
                //tCPServer.tempClient.Send(Encoding.UTF8.GetBytes($"测试主动发送数据:{str}") );
                //var jsonStr=JsonHelper.GetTestToString();
                //tCPServer.tempClient.Send(Encoding.UTF8.GetBytes(jsonStr));

                //var jsonStr = JsonHelper.GetTestToString();
                //tCPServer.tempClient.SendToClient(3001, jsonStr);
            }


            //UDPServer server = new UDPServer();
            //server.Start();
            //while (true)
            //{
            //    string text = Console.ReadLine();
            //    server.Send(Encoding.UTF8.GetBytes(text));
            //}

            //WebSocketServer webSocketServer = new WebSocketServer();
            //webSocketServer.Start();    
            //while (true)
            //{
            //    string text = Console.ReadLine();
            //    webSocketServer.Send(Encoding.UTF8.GetBytes(text));
            //}

            //HttpServer httpServer= new HttpServer();    
            //httpServer.Start();
            //while (true)
            //{
            //    string text = Console.ReadLine();
            //    //httpServer.Send(Encoding.UTF8.GetBytes(text));
            //}
        }
    }
}
