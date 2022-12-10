using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class MessageHelper 
{
    private static MessageHelper instance = new MessageHelper();
    public static MessageHelper Instance => instance;

    byte[] data = new byte[4096];
    int msgLenth = 0;

    public void CopyToData(byte[] buffer, int length)
    {
        Array.Copy(buffer, 0, data, msgLenth, length);
        msgLenth += length;
        Handle();
    }

    private void Handle()
    {
        //�����С(4) Э��ID(4) ����(byte[])
        if (msgLenth >= 8)
        {
            byte[] _size = new byte[4];
            Array.Copy(data, 0, _size, 0, 4);
            int size = BitConverter.ToInt32(_size, 0);

            //����Ҫ�õĳ���
            var _length = 8 + size;

            if (msgLenth >= _length)
            {
                //�ó�id
                byte[] _id = new byte[4];
                Array.Copy(data, 4, _id, 0, 4);
                int id = BitConverter.ToInt32(_id, 0);

                //����
                byte[] body = new byte[size];
                Array.Copy(data, 8, body, 0, size);

                if (msgLenth > _length)
                {
                    for (int i = 0; i < msgLenth - _length; i++)
                    {
                        data[i] = data[_length + i];
                    }
                }

                msgLenth -= _length;
                Debug.Log($"�յ���������Ϣ:{id}");
                switch (id)
                {
                    case 1001://ע������
                        RigisterMsgHandle(body);
                        break;
                    case 1002://��¼ҵ��
                        LoginMsgHandle(body);
                        break;
                    case 1003://����ҵ��
                        ChatMsgHandle(body);
                        break;

                }
            }
        }
    }


    public void SendToServer(int id, string str)
    {
        Debug.Log("ID:" + id);
        var body = Encoding.UTF8.GetBytes(str);
        
        byte[] send_buff = new byte[body.Length + 8];

        int size = body.Length;

        var _size = BitConverter.GetBytes(size);
        var _id = BitConverter.GetBytes(id);

        Array.Copy(_size, 0, send_buff, 0, 4);
        Array.Copy(_id, 0, send_buff, 4, 4);
        Array.Copy(body, 0, send_buff, 8, body.Length);

        Client.Instance.Send(send_buff);
    }


    //���͵�¼����Ϣ�������� 1002
    public void SendLoginMsg(string account, string pwd)
    {
        LoginMsgC2S msg = new LoginMsgC2S();
        msg.account = account;
        msg.password = pwd;
        var str = JsonHelper.ToJson(msg);
        SendToServer(1002, str);
    }

    public Action<LoginMsgS2C> loginHandle;
    //�����¼(���)����
    public void LoginMsgHandle(byte[] obj)
    {
        var str = Encoding.UTF8.GetString(obj);
        LoginMsgS2C msg = JsonHelper.ToObject<LoginMsgS2C>(str);
        loginHandle?.Invoke(msg);
    }

    //����������Ϣ��������
    public void SendChatMsg(string account, string chat)
    {
        ChatMsgC2S msg = new ChatMsgC2S();
        msg.player = account;
        msg.msg = chat;
        msg.type = 0;

        var str = JsonHelper.ToJson(msg);
        SendToServer(1003, str);
    }

    public Action<ChatMsgS2C> chatHandle;
    //��������(ת��)ҵ��
    public void ChatMsgHandle(byte[] obj)
    {
        var str = Encoding.UTF8.GetString(obj);
        ChatMsgS2C msg = JsonHelper.ToObject<ChatMsgS2C>(str);
        chatHandle?.Invoke(msg);
    }


    //����ע�����Ϣ�������� 1001
    public void SendRegisterMsg(string account, string email, string pwd)
    {
        RegisterMsgC2S msg = new RegisterMsgC2S();
        msg.account = account;
        msg.email = email;
        msg.password = pwd;
        var str = JsonHelper.ToJson(msg);
        SendToServer(1001, str);
    }

    public Action<RegisterMsgS2C> rigisterHandle;
    //����ע��(���)ҵ��
    public void RigisterMsgHandle(byte[] obj)
    {
        var str = Encoding.UTF8.GetString(obj);
        RegisterMsgS2C msg = JsonHelper.ToObject<RegisterMsgS2C>(str);
        rigisterHandle?.Invoke(msg);
    }


}

//1002
public class LoginMsgC2S {
    public string account;
    public string password;
}

public class LoginMsgS2C {

    public string account;
    public string password;
    public int result;//0�ɹ� 1ʧ��:�ʺŻ����������
}

//1001
public class RegisterMsgC2S
{
    public string account;
    public string email;
    public string password;
}

public class RegisterMsgS2C
{
    public string account;
    public string email;
    public string password;
    public int result;//0�ɹ� 1�ѱ�ע����˺�
}
//1003
public class ChatMsgC2S
{
    public string player;
    public string msg;
    public int type;//0�������� 
}

//������ת�����������ߵĿͻ���
public class ChatMsgS2C
{
    public string player;
    public string msg;
    public int type;//0�������� 
}