using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client_WebSocket : MonoBehaviour
{
    ClientWebSocket clientWebSocket;
    void Start()
    {
        clientWebSocket=new ClientWebSocket();
        Connect();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Send(Encoding.UTF8.GetBytes("websocket test send..."));
        }
    }

    CancellationTokenSource tokenSource=new CancellationTokenSource();
    //连接服务器
    public async void Connect() {
       
        await clientWebSocket.ConnectAsync(new Uri("ws://127.0.0.1:8080/"), tokenSource.Token);
        Receive();
    }


    //发送消息
    public async void Send(byte[] data)
    {
        await clientWebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, tokenSource.Token);
        Debug.Log("已发送成功");
    }


    //接收消息
    public async void Receive() {
        while (clientWebSocket != null && clientWebSocket.State == WebSocketState.Open)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048 * 2]);
            WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(buffer, tokenSource.Token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                clientWebSocket.Abort();
                clientWebSocket = null;
            }
            else
            {
                byte[] temp = new byte[result.Count];
                Array.Copy(buffer.Array, temp, result.Count);
                Debug.Log(Encoding.UTF8.GetString(temp));
            }
        }
    }

}
