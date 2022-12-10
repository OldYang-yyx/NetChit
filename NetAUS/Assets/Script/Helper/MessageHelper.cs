using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MessageHelper 
{
    private static MessageHelper instance = new MessageHelper();
    public static MessageHelper Instance => instance;       //这里表示只有Get没有Set

    byte[] data = new byte[4096];       //msg3.2我们还需要一个缓冲区
    int msgLenth = 0;                   //msg3.1先定义一个已经接收到的数据大小

    public void CopyToData(byte[] buffer,int length)           //msg3.4这个接口 将网络收到的buffer数据拷贝到这个data里去，在做逻辑
    {
        Array.Copy(buffer,  0, data, msgLenth, length);
        msgLenth += length;                                      //msg3.5 长度+=一下
        Handle();

    }



    private void Handle()               //msg3.3
    {
        //msg1.6 处理数据的方法 依据包体大小(4)  协议ID(4)  包体(byte[])
        if (msgLenth >= 8)        //msg1.7第一步看看这个数据是否可用，如果前两项共8个字节有就可以继续判断，包体是否为空先不管
        {
            byte[] _size = new byte[4];        //msg1.9创建一个临时的容器
            Array.Copy(data, 0, _size, 0, 4);           //msg1.8先拿出前4个字节，就是包体的大小啦  此句就是将data的第0位开始拷贝得到4位到_size里
            int size = BitConverter.ToInt32(_size, 0);         //msg1.10在将byte转换为int数组,通过这个size就能确定好包体有多大了

            var _length = 8 + size;         //msg1.11_length的长度就等于前面8个字节加上size

            if (msgLenth >= _length)      //msg1.12如果接收到的信息字节数>=包体的总长度  我们需要处理
            {
                //我们要拿id
                byte[] _id = new byte[4];        //msg1.13创建一个临时的容器
                Array.Copy(data, 4, _id, 0, 4);           //msg1.14先拿出前4个字节，就是包体的大小啦  此句就是将data的第0位开始拷贝得到4位到_size里
                int id = BitConverter.ToInt32(_id, 0);         //msg1.14在将byte转换为int数组,拿到它的id

                //我们要拿包体
                byte[] body = new byte[size];               //msg1.15 包体的大小我们求过了，这是个临时容器
                Array.Copy(data, 8, body, 0, size);         //msg1.18 拿包体我们要从第九位开始拿

                if (msgLenth > _length)                       //msg1.19判断一下，收到的长度如果大于本次要拿的长度，说明里面还剩有数据  粘包了
                {
                    for (int i = 0; i < msgLenth - _length; i++)  //msg1.20我们需要把剩下的数据往前挪
                    {
                        data[i] = data[_length + i];
                    }
                }
                msgLenth -= _length;                      //msg1.21操作完之后，我们要将本次收到的长度给减一下，减掉本次从缓冲区拿出来的长度，
                Debug.Log($"收到客户端请求：{id}"); //msg1.22打印一下看看id是什么
                switch (id)                                //msg1.23之后我们可以根据id做相应的处理了，通常在项目中，我们一般用观察者模式和他的设计模式将id和处理的Action绑定起来 思路有key是id 值是Action这个思路
                {
                    case 1001:                      //msg1.24注册请求
                        RigisterMsgHandle(body);
                        break;
                    case 1002:                      //msg1.25登录业务
                        LoginMsgHandle(body);
                        break;
                    case 1003:                      //msg1.26聊天业务
                        ChatMsgHandle(body);
                        break;
                }
            }
        }
    }

    //msg new  按格式封装后 发送信息
    public void SendToServer(int id, string str)
    {
        var body = Encoding.UTF8.GetBytes(str);             //先获取到包体的长度
                                                            //包体大小(4) 消息ID(4) 包体内容
        byte[] send_buff = new byte[body.Length + 8];         //包体的长度加上8就是我们实际要发送的大小

        int size = body.Length;
        var _size = BitConverter.GetBytes(size);
        var _id = BitConverter.GetBytes(id);

        Array.Copy(_size, 0, send_buff, 0, 4);              //将新的信息拷贝到send_buff中去
        Array.Copy(_id, 0, send_buff, 4, 4);
        Array.Copy(body, 0, send_buff, 8, body.Length);

        Client.Instance.Send(send_buff);                                    //拷贝完之后，我们在调用Send发送就可以了 
    }


    public Action<RegisterMsgS2C> registerHandle;   //netKey2.4.3.1 定义的Action，用于界面逻辑
    //msg2.1处理注册（结果）服务器返回的业务、
    //netKey 2.00000  下面我们来补充这些接口的逻辑
    //netKey 2.1 如果接到注册成功的内容我们就跳转到登录界面去
    private void RigisterMsgHandle(byte[] obj)
    {   
        var str = Encoding.UTF8.GetString(obj);     //netKey2.4.1先把包体转换成json 
        RegisterMsgS2C msg = JsonHelper.ToObject<RegisterMsgS2C>(str);      //netKey2.4.2将数组反序列化   
        registerHandle?.Invoke(msg);                //netKey2.4.3 界面的逻辑 我们不在这里处理，定义一个Action 进行Invoke  、、通过外部监听这条消息进行实现
    
    }   

    public Action<LoginMsgS2C> loginMsgHandle;
    //msg2.2处理登录（结果）业务的
    //netKey 2.2 如果我我们接到登录的消息，如果登录消失发送的是成功的话，就会跳转到聊天的界面去
    private void LoginMsgHandle(byte[] obj)
    {
        //netKey 2.5.1收到登录后的结果，我们要做的逻辑 //分别是 先转json，在反序列化，最后把界面逻辑传出去
        var str = Encoding.UTF8.GetString(obj);
        LoginMsgS2C msg = JsonHelper.ToObject<LoginMsgS2C>(str);
        loginMsgHandle?.Invoke(msg);


    }


    public Action<ChatMsgS2C> chatMsgHandle;
    //msg2.3处理聊天（转发）业务的     //服务端转发其他客户端发来的聊天数据
    //netKey 2.3 聊天发送后，服务端会把消息转发给所有客户端
    private void ChatMsgHandle(byte[] obj)
    {
        //netKey 2.6.1收到服务端发送的聊天时候，我们要做的逻辑 //同样的原则，我们只做数据的处理，其他的逻辑，传出去让外面做
        var str = Encoding.UTF8.GetString(obj);
        ChatMsgS2C msg = JsonHelper.ToObject<ChatMsgS2C>(str);
        chatMsgHandle?.Invoke(msg);
    }

    //netKey 2.4 发送注册的消息给服务端 1001
    public void SendRegisterMsg(string account,string email,string pwd)
    {
        RegisterMsgC2S msg = new RegisterMsgC2S();      //实例 刚刚定义的注册类的对象
        msg.account = account;
        msg.email = email;
        msg.password = pwd;
        var str = JsonHelper.ToJson(msg); //将我们的这实例类的内容 转换成Json格式的字符串，便于发送
        SendToServer(1001,str);     //转录为Tcp粘包拆包的通用格式，序列化成一个byte数组发送给后端
    }
    //netKey 2.5 如果登录成功我们怎么处理呢
                //发送登录的消息给服务端 1002
    public void SendLoginMsg(string account,string pwd)
    {
        LoginMsgC2S msg = new LoginMsgC2S();    //实例 刚刚定义的登录类 的对象
        msg.account=account;
        msg.password=pwd;  
        var str = JsonHelper.ToJson(msg);       //将实例转换成字符串
        SendToServer(1002,str);                 //格式化后 发送给后端
    }

    //netKey 2.6 聊天的处理，发送聊天消息给服务器  1003
    public void SendChatMsg(string account, string chat)
    {
        ChatMsgC2S msg = new ChatMsgC2S();
        msg.player = account;         //这里为了方便，我们就把账号当做名字了
        msg.msg = chat;
        msg.type = 0;

        var str = JsonHelper.ToJson(msg);
        SendToServer(1003, str);
    }
}

//netKey 1.1002   一般我们是和后端定义好 比如 ID是1002 请求就对应LoginMsgC2S这一条 服务端响应就是LoginMsgS2C这条 ；   1001代表注册同理
public class LoginMsgC2S        //netKey 1.0定义协议，比如登录请求的
{
    public string account;      //netKey 1.1我要发什么给服务器呢，比如说账号和密码
    public string password;
}
public class LoginMsgS2C        //netKey 1.2f服务器端呢，会返回登录的结果我们
{
    public string account;      //netKey 1.3它会返回账号和密码，原封不动的返回给我们
    public string password;

    public int result;          //netKey 1.4还要返回一个登录结果，0成功 1失败：账号或密码错误
}
//netKey 1.5 下面定义注册的协议   1001
public class RegisterMsgC2S
{
    public string account;
    public string email;
    public string password;
}
//netKey 1.6 服务器端返回注册的协议
public class RegisterMsgS2C
{
    public string account;
    public string email;
    public string password;
    public int result;      //0成功  1已被注册的账号
}
//netKey 1.7 下面定义发送聊天的协议    1003
public class ChatMsgC2S
{
    public string player;   //将昵称 聊天内容 数据类型发送出去
    public string msg;
    public int type;
}

//netKey 1.8 服务器转发给所有在线的客户端
public class ChatMsgS2C
{
    public string player;
    public string msg;
    public int type;
}