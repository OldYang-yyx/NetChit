using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client 
{
    static Client instance = new Client();            //1.5单例让其方便在其他地方调用
    public static Client Instance => instance;         //1.4在把这个服务器做成当地的

    TcpClient client;                                   //1.6和服务端通讯，我们需要用到TCPClient这个对象   
    public void Start()
    {
        client = new TcpClient();                       //1.7想要连接服务器，我们首先要构造这个对象
        Connect();                                     //1.8在调用Connect这个接口
        
    }
    public async void Connect()                       //1.1我们需要一个连接服务器的接口
    {
        try
        {
            await client.ConnectAsync("127.0.0.1",7788);    // 1.9这里的IP我们传本机地址就可以了，真正的项目开发中后端会给一个IP和端口，将其填进去就可以了
            Debug.Log("TCP 连接成功");
            Receive();                                          //1.10连接完成之后，我们需要接受数据
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public async void Receive()                     //1.3接受数据的接口,使用异步的形式，这样在await时候就会返回主线程
    {
        while (client.Connected)                    //1.11和服务端是一样的，判断如果client是连接的状态的话就进行不停的接收
        {
            try
            {
                byte[] buffer = new byte[4096];     //1.13我们需要一个buffer用来储存接收到的数据,并给他定义一个初始的容量4096. 表示传输中每条消息不超过4096或1024自己定
                int length = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);      //1.12这里我们用这个方法实现接收，这个方法需要一些参数，接下来我们就把这些参数创建出来,

                if (length > 0)                                                              //1.14 当接收的大于0才返回，否则，有可能是客户端关闭了
                {
                    Debug.Log($"接收到的数据长度：{length}");                                     //1.15 然后可以把这个长度打印出来

                    MessageHelper.Instance.CopyToData(buffer, length);                          //msg3.6 调用CopyToData，buffer就是接收的信息，length就是长度




                    Debug.Log(Encoding.UTF8.GetString(buffer, 0, length));                      //1.16 将其转换为字符串Encoding功能：将指定字节数字中的所有字节解码为一个字符串。

                  

                }
                else client.Close();                                                            //1.17 当接收的不大于0，有可能是客户端关闭了，我们直接将其关闭
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                client.Close();                                                                 //1.18 出现异常也将客户端关闭掉
            }
        }

    }
    public async void Send(byte[] data)                                            //1.2发送数据给服务端
    {
        try
        {
            await client.GetStream().WriteAsync(data, 0, data.Length);              //1.19发送接口我们用client.GetStream().WriteAsync()这个接口进行发送。括号内的参数a.要发的信息b.从哪开始写入c.信息长度
                                                                                    // 发送的就是我们要传给这个接口的内容，
            Debug.Log("发送成功");                                                   //1.20发送成功后我们打印一下


        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            client.Close();
        }
    }

   
}
