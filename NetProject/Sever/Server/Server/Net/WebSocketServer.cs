using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Net
{
    internal class WebSocketServer
    {
        HttpListener httpListener;
        public void Start()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://127.0.0.1:8080/");
            httpListener.Start();
            Console.WriteLine("服务端已启动");
            AcceptConnect();
        }


        WebSocket ws;
        //等待客户端连接
        public async void AcceptConnect()
        {
            while (true)
            {
                HttpListenerContext httpListenerContext = await httpListener.GetContextAsync();
                if (httpListenerContext.Request.IsWebSocketRequest)
                {
                    var wsContext = await httpListenerContext.AcceptWebSocketAsync(null);
                    ws = wsContext.WebSocket;
                    Console.WriteLine("收到客户端连接");

                    //实际上还是要跟TCP做法一样 实例化一个Client
                    //需要管理多个客户端
                    Receive();
                }
            }
        }

        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        //接收消息
        public async void Receive()
        {
            try
            {
                while (ws != null && ws.State == WebSocketState.Open)
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048 * 2]);
                    WebSocketReceiveResult result = await ws.ReceiveAsync(buffer, cancellationToken.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        ws.Abort();
                        ws = null;
                        return;
                    }
                    else
                    {
                        byte[] temp = new byte[result.Count];
                        Array.Copy(buffer.Array, temp, result.Count);
                        Console.WriteLine(Encoding.UTF8.GetString(temp));
                    }
                }
            }
            catch (Exception e)
            {
                ws.Abort();
                ws = null;
                return;
            }
          
        }

        //发送消息
        public async void Send(byte[] data)
        {
            await ws.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, cancellationToken.Token);
            Console.Write("已发送成功");
        }

    }
}
