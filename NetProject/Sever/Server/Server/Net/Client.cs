using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Server.Helper;

namespace Server.Net
{
    internal class Client
    {
        TcpClient client;
        public Client(TcpClient tcpClient) {

            client = tcpClient;
            Receive();
        }

        byte[] data = new byte[4096];
        int msgLenth = 0;

        //接收消息
        public async void Receive() {
            //处于连接状态
            while (client.Connected) {

                try
                {
                    byte[] buffer = new byte[4096];
                    int length = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);

                    if (length > 0) {
                        Console.WriteLine($"接收到的数据长度:{length}");
                        Console.WriteLine($"接收到的数据内容:{Encoding.UTF8.GetString(buffer, 0, length)}");
                        //Send(Encoding.UTF8.GetBytes("测试返回..."));
                        Array.Copy(buffer, 0, data, msgLenth, length);
                        msgLenth += length;
                        Handle();//alt+enter
                    }
                    else
                    {
                        //客户端关闭了
                        client.Close();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Receive Error:{e.Message}");
                    client.Close();
                }
            }
           

        }

        private void Handle()
        {
            //包体大小(4) 协议ID(4) 包体(byte[])
            if (msgLenth>=8)
            {
                byte[] _size= new byte[4];
                Array.Copy(data, 0, _size, 0, 4);
                int size = BitConverter.ToInt32(_size, 0);

                //本次要拿的长度
                var _length = 8 + size;

                if (msgLenth>=_length)
                {
                    //拿出id
                    byte[] _id = new byte[4];
                    Array.Copy(data, 4, _id, 0, 4);
                    int id = BitConverter.ToInt32(_id, 0);

                    //包体
                    byte[] body= new byte[size];
                    Array.Copy(data, 8, body, 0, size);

                    if (msgLenth>_length)
                    {
                        for (int i = 0; i < msgLenth-_length; i++)
                        {
                            data[i] = data[_length + i];
                        }
                    }

                    msgLenth -= _length;
                    Console.WriteLine($"收到客户端请求:{id}");
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

        //处理注册请求
        public void RigisterMsgHandle(byte[] obj) {
          var msg= JsonHelper.ToObject<RegisterMsgC2S>(obj);
            RegisterMsgS2C response = null;
            if (PlayerData.Instance.Contain(msg.account)) {
                response = new RegisterMsgS2C();
                response.result = 1;//已经包含该账号了
            }
            else
            {
                response = PlayerData.Instance.Add(msg);
            }
            SendToClient(1001,JsonHelper.ToJson(response));
        }

        //处理登录请求
        public void LoginMsgHandle(byte[] obj)
        {
            //判断账号和密码是否跟缓存的一致 如果是 返回账号和密码
            var msg = JsonHelper.ToObject<LoginMsgC2S>(obj);
            LoginMsgS2C response = new LoginMsgS2C();
            if (PlayerData.Instance.Contain(msg.account))
            {
                response.result = 0;//已包含该账号
                response.account = msg.account;
                response.password = msg.password;

                //将用户存储起来
                PlayerData.Instance.AddLoginUser(msg.account, this);
            }
            else
            {
                response.result = 1;//账号或者密码错误
            }
            SendToClient(1002, JsonHelper.ToJson(response));
        }

        //处理聊天业务
        public void ChatMsgHandle(byte[] obj)
        {
            //转发给所有在线的用户
            var msg = JsonHelper.ToObject<ChatMsgC2S>(obj);
            ChatMsgS2C sendMsg = new ChatMsgS2C();
            sendMsg.msg = msg.msg;
            sendMsg.player = msg.player;
            sendMsg.type = msg.type;

            var dct = PlayerData.Instance.GetAllLoginUser();
            var json = JsonHelper.ToJson(sendMsg);
            foreach (var item in dct)
            {
                item.Value.SendToClient(1003, json);
            }
        }

        //发送接口
        public async void Send(byte[] data) {
            try
            {
                await client.GetStream().WriteAsync(data, 0, data.Length);
                Console.WriteLine("发送成功");
            }
            catch (Exception e)
            {
                client.Close();
                Console.WriteLine($"send error:{e.Message}");
            }
        }

        //按格式封装后 发送消息
        public void SendToClient(int id, string str)
        {
            //转换成byte[]
            var body = Encoding.UTF8.GetBytes(str);

            //包体大小(4) 消息ID(4) 包体内容
            byte[] send_buff = new byte[body.Length + 8];

            int size = body.Length;
            var _size = BitConverter.GetBytes(size);
            var _id = BitConverter.GetBytes(id);

            Array.Copy(_size, 0, send_buff, 0, 4);
            Array.Copy(_id, 0, send_buff, 4, 4);
            Array.Copy(body, 0, send_buff, 8, body.Length);

            Send(send_buff);
        }
    }
}


