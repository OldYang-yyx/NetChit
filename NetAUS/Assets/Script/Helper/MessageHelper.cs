using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MessageHelper 
{
    private static MessageHelper instance = new MessageHelper();
    public static MessageHelper Instance => instance;       //�����ʾֻ��Getû��Set

    byte[] data = new byte[4096];       //msg3.2���ǻ���Ҫһ��������
    int msgLenth = 0;                   //msg3.1�ȶ���һ���Ѿ����յ������ݴ�С

    public void CopyToData(byte[] buffer,int length)           //msg3.4����ӿ� �������յ���buffer���ݿ��������data��ȥ�������߼�
    {
        Array.Copy(buffer,  0, data, msgLenth, length);
        msgLenth += length;                                      //msg3.5 ����+=һ��
        Handle();

    }



    private void Handle()               //msg3.3
    {
        //msg1.6 �������ݵķ��� ���ݰ����С(4)  Э��ID(4)  ����(byte[])
        if (msgLenth >= 8)        //msg1.7��һ��������������Ƿ���ã����ǰ���8���ֽ��оͿ��Լ����жϣ������Ƿ�Ϊ���Ȳ���
        {
            byte[] _size = new byte[4];        //msg1.9����һ����ʱ������
            Array.Copy(data, 0, _size, 0, 4);           //msg1.8���ó�ǰ4���ֽڣ����ǰ���Ĵ�С��  �˾���ǽ�data�ĵ�0λ��ʼ�����õ�4λ��_size��
            int size = BitConverter.ToInt32(_size, 0);         //msg1.10�ڽ�byteת��Ϊint����,ͨ�����size����ȷ���ð����ж����

            var _length = 8 + size;         //msg1.11_length�ĳ��Ⱦ͵���ǰ��8���ֽڼ���size

            if (msgLenth >= _length)      //msg1.12������յ�����Ϣ�ֽ���>=������ܳ���  ������Ҫ����
            {
                //����Ҫ��id
                byte[] _id = new byte[4];        //msg1.13����һ����ʱ������
                Array.Copy(data, 4, _id, 0, 4);           //msg1.14���ó�ǰ4���ֽڣ����ǰ���Ĵ�С��  �˾���ǽ�data�ĵ�0λ��ʼ�����õ�4λ��_size��
                int id = BitConverter.ToInt32(_id, 0);         //msg1.14�ڽ�byteת��Ϊint����,�õ�����id

                //����Ҫ�ð���
                byte[] body = new byte[size];               //msg1.15 ����Ĵ�С��������ˣ����Ǹ���ʱ����
                Array.Copy(data, 8, body, 0, size);         //msg1.18 �ð�������Ҫ�ӵھ�λ��ʼ��

                if (msgLenth > _length)                       //msg1.19�ж�һ�£��յ��ĳ���������ڱ���Ҫ�õĳ��ȣ�˵�����滹ʣ������  ճ����
                {
                    for (int i = 0; i < msgLenth - _length; i++)  //msg1.20������Ҫ��ʣ�µ�������ǰŲ
                    {
                        data[i] = data[_length + i];
                    }
                }
                msgLenth -= _length;                      //msg1.21������֮������Ҫ�������յ��ĳ��ȸ���һ�£��������δӻ������ó����ĳ��ȣ�
                Debug.Log($"�յ��ͻ�������{id}"); //msg1.22��ӡһ�¿���id��ʲô
                switch (id)                                //msg1.23֮�����ǿ��Ը���id����Ӧ�Ĵ����ˣ�ͨ������Ŀ�У�����һ���ù۲���ģʽ���������ģʽ��id�ʹ����Action������ ˼·��key��id ֵ��Action���˼·
                {
                    case 1001:                      //msg1.24ע������
                        RigisterMsgHandle(body);
                        break;
                    case 1002:                      //msg1.25��¼ҵ��
                        LoginMsgHandle(body);
                        break;
                    case 1003:                      //msg1.26����ҵ��
                        ChatMsgHandle(body);
                        break;
                }
            }
        }
    }

    //msg new  ����ʽ��װ�� ������Ϣ
    public void SendToServer(int id, string str)
    {
        var body = Encoding.UTF8.GetBytes(str);             //�Ȼ�ȡ������ĳ���
                                                            //�����С(4) ��ϢID(4) ��������
        byte[] send_buff = new byte[body.Length + 8];         //����ĳ��ȼ���8��������ʵ��Ҫ���͵Ĵ�С

        int size = body.Length;
        var _size = BitConverter.GetBytes(size);
        var _id = BitConverter.GetBytes(id);

        Array.Copy(_size, 0, send_buff, 0, 4);              //���µ���Ϣ������send_buff��ȥ
        Array.Copy(_id, 0, send_buff, 4, 4);
        Array.Copy(body, 0, send_buff, 8, body.Length);

        Client.Instance.Send(send_buff);                                    //������֮�������ڵ���Send���;Ϳ����� 
    }


    public Action<RegisterMsgS2C> registerHandle;   //netKey2.4.3.1 �����Action�����ڽ����߼�
    //msg2.1����ע�ᣨ��������������ص�ҵ��
    //netKey 2.00000  ����������������Щ�ӿڵ��߼�
    //netKey 2.1 ����ӵ�ע��ɹ����������Ǿ���ת����¼����ȥ
    private void RigisterMsgHandle(byte[] obj)
    {   
        var str = Encoding.UTF8.GetString(obj);     //netKey2.4.1�ȰѰ���ת����json 
        RegisterMsgS2C msg = JsonHelper.ToObject<RegisterMsgS2C>(str);      //netKey2.4.2�����鷴���л�   
        registerHandle?.Invoke(msg);                //netKey2.4.3 ������߼� ���ǲ������ﴦ������һ��Action ����Invoke  ����ͨ���ⲿ����������Ϣ����ʵ��
    
    }   

    public Action<LoginMsgS2C> loginMsgHandle;
    //msg2.2�����¼�������ҵ���
    //netKey 2.2 ��������ǽӵ���¼����Ϣ�������¼��ʧ���͵��ǳɹ��Ļ����ͻ���ת������Ľ���ȥ
    private void LoginMsgHandle(byte[] obj)
    {
        //netKey 2.5.1�յ���¼��Ľ��������Ҫ�����߼� //�ֱ��� ��תjson���ڷ����л������ѽ����߼�����ȥ
        var str = Encoding.UTF8.GetString(obj);
        LoginMsgS2C msg = JsonHelper.ToObject<LoginMsgS2C>(str);
        loginMsgHandle?.Invoke(msg);


    }


    public Action<ChatMsgS2C> chatMsgHandle;
    //msg2.3�������죨ת����ҵ���     //�����ת�������ͻ��˷�������������
    //netKey 2.3 ���췢�ͺ󣬷���˻����Ϣת�������пͻ���
    private void ChatMsgHandle(byte[] obj)
    {
        //netKey 2.6.1�յ�����˷��͵�����ʱ������Ҫ�����߼� //ͬ����ԭ������ֻ�����ݵĴ����������߼�������ȥ��������
        var str = Encoding.UTF8.GetString(obj);
        ChatMsgS2C msg = JsonHelper.ToObject<ChatMsgS2C>(str);
        chatMsgHandle?.Invoke(msg);
    }

    //netKey 2.4 ����ע�����Ϣ������� 1001
    public void SendRegisterMsg(string account,string email,string pwd)
    {
        RegisterMsgC2S msg = new RegisterMsgC2S();      //ʵ�� �ոն����ע����Ķ���
        msg.account = account;
        msg.email = email;
        msg.password = pwd;
        var str = JsonHelper.ToJson(msg); //�����ǵ���ʵ��������� ת����Json��ʽ���ַ��������ڷ���
        SendToServer(1001,str);     //ת¼ΪTcpճ�������ͨ�ø�ʽ�����л���һ��byte���鷢�͸����
    }
    //netKey 2.5 �����¼�ɹ�������ô������
                //���͵�¼����Ϣ������� 1002
    public void SendLoginMsg(string account,string pwd)
    {
        LoginMsgC2S msg = new LoginMsgC2S();    //ʵ�� �ոն���ĵ�¼�� �Ķ���
        msg.account=account;
        msg.password=pwd;  
        var str = JsonHelper.ToJson(msg);       //��ʵ��ת�����ַ���
        SendToServer(1002,str);                 //��ʽ���� ���͸����
    }

    //netKey 2.6 ����Ĵ�������������Ϣ��������  1003
    public void SendChatMsg(string account, string chat)
    {
        ChatMsgC2S msg = new ChatMsgC2S();
        msg.player = account;         //����Ϊ�˷��㣬���ǾͰ��˺ŵ���������
        msg.msg = chat;
        msg.type = 0;

        var str = JsonHelper.ToJson(msg);
        SendToServer(1003, str);
    }
}

//netKey 1.1002   һ�������Ǻͺ�˶���� ���� ID��1002 ����Ͷ�ӦLoginMsgC2S��һ�� �������Ӧ����LoginMsgS2C���� ��   1001����ע��ͬ��
public class LoginMsgC2S        //netKey 1.0����Э�飬�����¼�����
{
    public string account;      //netKey 1.1��Ҫ��ʲô���������أ�����˵�˺ź�����
    public string password;
}
public class LoginMsgS2C        //netKey 1.2f���������أ��᷵�ص�¼�Ľ������
{
    public string account;      //netKey 1.3���᷵���˺ź����룬ԭ�ⲻ���ķ��ظ�����
    public string password;

    public int result;          //netKey 1.4��Ҫ����һ����¼�����0�ɹ� 1ʧ�ܣ��˺Ż��������
}
//netKey 1.5 ���涨��ע���Э��   1001
public class RegisterMsgC2S
{
    public string account;
    public string email;
    public string password;
}
//netKey 1.6 �������˷���ע���Э��
public class RegisterMsgS2C
{
    public string account;
    public string email;
    public string password;
    public int result;      //0�ɹ�  1�ѱ�ע����˺�
}
//netKey 1.7 ���涨�巢�������Э��    1003
public class ChatMsgC2S
{
    public string player;   //���ǳ� �������� �������ͷ��ͳ�ȥ
    public string msg;
    public int type;
}

//netKey 1.8 ������ת�����������ߵĿͻ���
public class ChatMsgS2C
{
    public string player;
    public string msg;
    public int type;
}