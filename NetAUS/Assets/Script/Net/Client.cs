using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client 
{
    static Client instance = new Client();            //1.5�������䷽���������ط�����
    public static Client Instance => instance;         //1.4�ڰ�������������ɵ��ص�

    TcpClient client;                                   //1.6�ͷ����ͨѶ��������Ҫ�õ�TCPClient�������   
    public void Start()
    {
        client = new TcpClient();                       //1.7��Ҫ���ӷ���������������Ҫ�����������
        Connect();                                     //1.8�ڵ���Connect����ӿ�
        
    }
    public async void Connect()                       //1.1������Ҫһ�����ӷ������Ľӿ�
    {
        try
        {
            await client.ConnectAsync("127.0.0.1",7788);    // 1.9�����IP���Ǵ�������ַ�Ϳ����ˣ���������Ŀ�����к�˻��һ��IP�Ͷ˿ڣ��������ȥ�Ϳ�����
            Debug.Log("TCP ���ӳɹ�");
            Receive();                                          //1.10�������֮��������Ҫ��������
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public async void Receive()                     //1.3�������ݵĽӿ�,ʹ���첽����ʽ��������awaitʱ��ͻ᷵�����߳�
    {
        while (client.Connected)                    //1.11�ͷ������һ���ģ��ж����client�����ӵ�״̬�Ļ��ͽ��в�ͣ�Ľ���
        {
            try
            {
                byte[] buffer = new byte[4096];     //1.13������Ҫһ��buffer����������յ�������,����������һ����ʼ������4096. ��ʾ������ÿ����Ϣ������4096��1024�Լ���
                int length = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);      //1.12�����������������ʵ�ֽ��գ����������ҪһЩ���������������ǾͰ���Щ������������,

                if (length > 0)                                                              //1.14 �����յĴ���0�ŷ��أ������п����ǿͻ��˹ر���
                {
                    Debug.Log($"���յ������ݳ��ȣ�{length}");                                     //1.15 Ȼ����԰�������ȴ�ӡ����

                    MessageHelper.Instance.CopyToData(buffer, length);                          //msg3.6 ����CopyToData��buffer���ǽ��յ���Ϣ��length���ǳ���




                    Debug.Log(Encoding.UTF8.GetString(buffer, 0, length));                      //1.16 ����ת��Ϊ�ַ���Encoding���ܣ���ָ���ֽ������е������ֽڽ���Ϊһ���ַ�����

                  

                }
                else client.Close();                                                            //1.17 �����յĲ�����0���п����ǿͻ��˹ر��ˣ�����ֱ�ӽ���ر�
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                client.Close();                                                                 //1.18 �����쳣Ҳ���ͻ��˹رյ�
            }
        }

    }
    public async void Send(byte[] data)                                            //1.2�������ݸ������
    {
        try
        {
            await client.GetStream().WriteAsync(data, 0, data.Length);              //1.19���ͽӿ�������client.GetStream().WriteAsync()����ӿڽ��з��͡������ڵĲ���a.Ҫ������Ϣb.���Ŀ�ʼд��c.��Ϣ����
                                                                                    // ���͵ľ�������Ҫ��������ӿڵ����ݣ�
            Debug.Log("���ͳɹ�");                                                   //1.20���ͳɹ������Ǵ�ӡһ��


        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            client.Close();
        }
    }

   
}
