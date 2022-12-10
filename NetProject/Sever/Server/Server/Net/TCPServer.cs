using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    internal class TCPServer
    {
        TcpListener tcpListener;
        //启动服务端 创建监听器
        public void Start() {

            try
            {
                //创建监听器
                tcpListener = TcpListener.Create(7788);//1-65535
                tcpListener.Start(500);//

                Console.WriteLine("TCP Server Start!");

                Accpet();
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
        public Client tempClient;
        //监听客户端的连接
        public async void Accpet() {
            try
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                Console.WriteLine("客户端已连接:" + tcpClient.Client.RemoteEndPoint);
                Client client = new Client(tcpClient);
                tempClient = client;
                Accpet();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Accpet:{e.Message}");
                tcpListener.Stop();//停止监听客户端的连接
            }
         

        }
    }
}
