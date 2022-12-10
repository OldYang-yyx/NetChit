using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class Client 
{
    static Client instance = new Client();
    public static Client Instance => instance;

    TcpClient client;
    public void Start()
    {
        client = new TcpClient();
        Connect();
    }

    public async void Connect() {
        try
        {
            await client.ConnectAsync("127.0.0.1",7788);
            Debug.Log("TCP 连接成功");
            Receive();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    
    }

    public async void Receive()
    {
        while (client.Connected) {
            try
            {
                byte[] buff = new byte[4096];
                int length = await client.GetStream().ReadAsync(buff, 0, buff.Length);
                if (length > 0)
                {
                    Debug.Log($"接收到数据了:{length}");
                    MessageHelper.Instance.CopyToData(buff, length);

                    //var str = Encoding.UTF8.GetString(buff, 0, length);
                    var str = Encoding.UTF8.GetString(buff, 8, length);
                    Debug.Log(str);
                   
                    //var jsonTest= JsonHelper.ToObject<JsonTest>(str);
                   // Debug.Log(jsonTest.id);
                   // Debug.Log(jsonTest.name);
                    //str->jsonTest 消息ID

                }
                else
                {
                    client.Close();
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                client.Close();
            }
           
        }
    }

    public async void Send(byte[] data) {
        try
        {
            await client.GetStream().WriteAsync(data, 0, data.Length);
            Debug.Log("发送成功");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            client.Close();
        }
    
    }




}
