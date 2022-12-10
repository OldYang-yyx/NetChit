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
        //包体大小(4) 协议ID(4) 包体(byte[])
        if (msgLenth >= 8)
        {
            byte[] _size = new byte[4];
            Array.Copy(data, 0, _size, 0, 4);
            int size = BitConverter.ToInt32(_size, 0);

            //本次要拿的长度
            var _length = 8 + size;

            if (msgLenth >= _length)
            {
                //拿出id
                byte[] _id = new byte[4];
                Array.Copy(data, 4, _id, 0, 4);
                int id = BitConverter.ToInt32(_id, 0);

                //包体
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
                Debug.Log($"收到服务器消息:{id}");
                switch (id)
                {
                    case 1001://注册请求
                        RigisterMsgHandle(body);
                        break;
                    case 1002://登录业务
                        LoginMsgHandle(body);
                        break;
                    case 1003://聊天业务
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


    //发送登录的消息给服务器 1002
    public void SendLoginMsg(string account, string pwd)
    {
        LoginMsgC2S msg = new LoginMsgC2S();
        msg.account = account;
        msg.password = pwd;
        var str = JsonHelper.ToJson(msg);
        SendToServer(1002, str);
    }

    public Action<LoginMsgS2C> loginHandle;
    //处理登录(结果)请求
    public void LoginMsgHandle(byte[] obj)
    {
        var str = Encoding.UTF8.GetString(obj);
        LoginMsgS2C msg = JsonHelper.ToObject<LoginMsgS2C>(str);
        loginHandle?.Invoke(msg);
    }

    //发送聊天信息给服务器
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
    //处理聊天(转发)业务
    public void ChatMsgHandle(byte[] obj)
    {
        var str = Encoding.UTF8.GetString(obj);
        ChatMsgS2C msg = JsonHelper.ToObject<ChatMsgS2C>(str);
        chatHandle?.Invoke(msg);
    }


    //发送注册的消息给服务器 1001
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
    //处理注册(结果)业务
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
    public int result;//0成功 1失败:帐号或者密码错误
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
    public int result;//0成功 1已被注册的账号
}
//1003
public class ChatMsgC2S
{
    public string player;
    public string msg;
    public int type;//0世界聊天 
}

//服务器转发给所有在线的客户端
public class ChatMsgS2C
{
    public string player;
    public string msg;
    public int type;//0世界聊天 
}