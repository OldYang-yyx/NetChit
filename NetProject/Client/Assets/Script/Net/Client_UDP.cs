using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

using UnityEngine;

public class Client_UDP : MonoBehaviour
{
    UdpClient udpClient;
    void Start()
    {
        udpClient=new UdpClient();
        Receive();
    }

    private async void Receive()
    {
        while (udpClient != null)
        {
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                Debug.Log(Encoding.UTF8.GetString(result.Buffer));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                udpClient.Close();
                udpClient = null;
            }
        }
    }

    public async void Send(byte[] data)
    {
        if (udpClient != null)
        {
            try
            {
                int length = await udpClient.SendAsync(data, data.Length, "127.0.0.1", 8899);
                if (data.Length == length)
                {
                    Debug.Log("完整的发送!");
                }
            }
            catch (Exception error)
            {

                Debug.Log(error.Message);
                udpClient.Close();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Send(Encoding.UTF8.GetBytes("UDP Send Test..."));
        }
    }
}
