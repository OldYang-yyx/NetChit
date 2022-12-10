using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    internal class UDPServer
    {
        UdpClient udpClient;
        public void Start() {
            try
            {
                udpClient = new UdpClient(8899);
                Console.WriteLine("UDP Server 启动成功 8899");
                Receive();
            }
            catch (Exception e)
            {
                Console.WriteLine("UDP Server Error:"+e.Message);
            }
           
        }
        IPEndPoint remote;
        private async void Receive()
        {

            while (udpClient!=null)
            {
                try
                {
                  var result= await udpClient.ReceiveAsync();
                   remote = result.RemoteEndPoint;
                   var text= Encoding.UTF8.GetString(result.Buffer);
                    Console.WriteLine($"收到客户端发送过来的数据:{text}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("接收异常:"+e.Message);
                    udpClient.Close();
                    udpClient=null;
                }
            }
        }

        public async void Send(byte[] data) {
           int length = await udpClient.SendAsync(data,data.Length, remote);
            Console.WriteLine($"UDP Send:{length}");
        }
    }
}
