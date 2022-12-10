using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Helper;

namespace Server.Net
{
    internal class Client       //3.每一个客户端都是一个独立的Client
    {
        TcpClient client; 

        //3.2给这个缓存连接到的客户端的客户端类，一个构造函数。 函数里面有什么，有一个TcpClient
        public Client(TcpClient tcpClient)
        {
            client = tcpClient;   //3.3将这个tcpClient缓存起来
            Receive();            //3.3.1调用接收信息的方法，不然是不会收到消息的
        }




        byte[] data = new byte[4096];       //msg1.3我们还需要一个缓冲区
        int msgLenth = 0;       //msg1.1先定义一个已经接收到的数据大小

        //3.4 那么这个客户端类主要负责什么工作呢，一个是接收消息，一个是发送消息
        //3.5 一个是接收消息的接口
        public async void Receive()
        {
            while (client.Connected)                      //5.9我们需要一个持续的接收从客户端发送过来的消息。 可以让条件为client.Connected(客户端处于连接状态就持续接收)
            {
                try                                     //5.10同样我们可以用其捕获异常
                { 
                    //5.3 下面我们需要将Receive方法完成，不然我们是没办法接收消息的
                    byte[] buffer = new byte[4096];                                  //5.5我们需要一个buffer用来储存接收到的数据,并给他定义一个初始的容量4096. 表示传输中每条消息不超过4096或1024自己定



                    int length = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);      //5.4这里我们用这个方法实现接收，这个方法需要一些参数，接下来我们就把这些参数创建出来,
                                                                                                    //5.6括号里的参数分别是a.那个buffer(缓存区)；b.第二个表示把这条消息传到缓冲区中从第几个字节开始存；c.存的数量是多少
                                                                                                    //   返回一个length表示，接收了多少数据
                    if (length > 0)                                                                 //5.12 当接收的大于0才返回，否则，有可能是客户端关闭了
                    {
                        Console.WriteLine($"接收到的数据长度：{length}");                               //5.7然后可以把这个长度打印出来
                        Console.WriteLine($"接收到的数据内容：{Encoding.UTF8.GetString(buffer, 0, length)}");             //5.8 将其转换为字符串Encoding功能：将指定字节数字中的所有字节解码为一个字符串。
                        
                       
                        Array.Copy(buffer, 0, data, msgLenth , length); //msg1.4我们要将每次收到的数据拷贝到缓冲区data里面
                        msgLenth += length;           //msg1.4.2每次收数据到时候我们都加一下数据长度   需要拷贝之后在+=

                        Handle();      //msg1.5写一个接口处理这些数据
                    }
                    else client.Close();                                                             //5.12.1 当接收的不大于0，有可能是客户端关闭了，我们直接将其关闭
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Receive Error:{e.Message}");
                    client.Close();                             //5.11出现错误的时候我们可以主动的把这个客户端断开，给关闭掉
                }
            }


        }

        private void Handle()
        {
            //msg1.6 处理数据的方法 依据包体大小(4)  协议ID(4)  包体(byte[])
            if (msgLenth>=8)        //msg1.7第一步看看这个数据是否可用，如果前两项共8个字节有就可以继续判断，包体是否为空先不管
            {
                byte[] _size = new byte[4];        //msg1.9创建一个临时的容器
                Array.Copy(data,0,_size,0,4);           //msg1.8先拿出前4个字节，就是包体的大小啦  此句就是将data的第0位开始拷贝得到4位到_size里
                int size = BitConverter.ToInt32(_size,0);         //msg1.10在将byte转换为int数组,通过这个size就能确定好包体有多大了
                var _length = 8 + size;         //msg1.11_length的长度就等于前面8个字节加上size

                if (msgLenth>=_length)      //msg1.12如果接收到的信息字节数>=包体的总长度  我们需要处理
                {
                    //我们要拿id
                    byte[] _id = new byte[4];        //msg1.13创建一个临时的容器
                    Array.Copy(data, 4, _id, 0, 4);           //msg1.14先拿出前4个字节，就是包体的大小啦  此句就是将data的第0位开始拷贝得到4位到_size里
                    int id = BitConverter.ToInt32(_id, 0);         //msg1.14在将byte转换为int数组,拿到它的id
                    //我们要拿包体
                    byte[] body = new byte[size];               //msg1.15 包体的大小我们求过了，这是个临时容器
                    Array.Copy(data, 8, body, 0, size);         //msg1.18 拿包体我们要从第九位开始拿

                    if (msgLenth>_length)                       //msg1.19判断一下，收到的长度如果大于本次要拿的长度，说明里面还剩有数据  粘包了
                    {
                        for (int i = 0; i < msgLenth-_length; i++)  //msg1.20我们需要把剩下的数据往前挪
                        {
                            data[i] = data[_length+i];
                        }
                    }
                    msgLenth -= _length;                      //msg1.21操作完之后，我们要将本次收到的长度给减一下，减掉本次从缓冲区拿出来的长度，
                    Console.WriteLine($"收到客户端请求：{id}"); //msg1.22打印一下看看id是什么
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

        //S2.0------------写业务啦

        //msg2.1处理注册请求的
        private void RigisterMsgHandle(byte[] obj)
        {
            //RigisterMsgHandle方法被调用后 会得到将网络收到的数据解完的包体 obj
            var msg = JsonHelper.ToObject<RegisterMsgC2S>(obj);       //将obj反序列化一下 ，反序列成c2s注册协议类型
            RegisterMsgS2C response = null;                           //我们需要有一个s2c的协议返回给客户端
            if (PlayerData.Instance.Contain(msg.account))
            {
                response = new RegisterMsgS2C();
                response.result = 1;                        //已经包含这个账号了
            }
            else
            {
                response = PlayerData.Instance.Add(msg);    //如果没有包含这个账号就把整个数据传进字典里
            }
            SendToClient(1001,JsonHelper.ToJson(response)); //并将s2c数据序列化，在格式化后发给客户端
        }

        //msg2.2处理登录业务的
        private void LoginMsgHandle(byte[] obj)
        {
            //判断账号和密码是否跟缓存的一致 如果是 换回账号和密码
            var msg = JsonHelper.ToObject<LoginMsgC2S>(obj);    //反序列化
            LoginMsgS2C response = new LoginMsgS2C();
            if (PlayerData.Instance.Contain(msg.account))       //看看是否包含了，只有包含了才进行登录的业务
            {
                response.result = 0;            //已包含 将数据原封不动的换回回去，并result设置为0
                response.account = msg.account;
                response.password = msg.password;

                //将用户存储起来   ，在聊天时候 对全员广播就会用到它
                PlayerData.Instance.AddLogginUser(msg.account,this);    
            }
            else
            {
                response.result = 1;    //账号或密码错误
            }
            SendToClient(1002,JsonHelper.ToJson(response));     //将它序列化成Json，并格式化发送
        }
        //msg2.3处理聊天业务的
        private void ChatMsgHandle(byte[] obj)
        {
            throw new NotImplementedException();
        }


        //msg new  按格式封装后 发送信息
        public void SendToClient(int id, string str)
        {
            var body = Encoding.UTF8.GetBytes(str);             //先获取到包体的长度
            //包体大小(4) 消息ID(4) 包体内容
            byte[] send_buff = new byte[body.Length+8];         //包体的长度加上8就是我们实际要发送的大小

            int size = body.Length;
            var _size = BitConverter.GetBytes(size);
            var _id = BitConverter.GetBytes(id);

            Array.Copy(_size, 0, send_buff, 0, 4);              //将新的信息拷贝到send_buff中去
            Array.Copy(_id, 0, send_buff, 4, 4);
            Array.Copy(body, 0, send_buff, 8, body.Length);

            Send(send_buff);                                    //拷贝完之后，我们在调用Send发送就可以了 
        }




        //3.6 一个是发送消息的接口
        public async void Send(byte[] data)            //6.1如果我们想发送消息的话，我们可以封装一个接口  发送接口同样是将byte数组发送给客户端
        {
            try                                           //6.4同样用try捕获异常
            {
                await client.GetStream().WriteAsync(data, 0, data.Length);               //6.2发送接口我们用client.GetStream().WriteAsync()这个接口进行发送。括号内的参数a.要发的信息b.从哪开始写入c.信息长度
                                                                                         //   发送的就是我们要传给这个接口的内容，
                Console.WriteLine("发送成功");                                            //6.3发送成功后我们打印一下
            }
            catch (Exception e)
            {
                client.Close();                     //6.5 出现错误或客户端已经关闭情况下导致为发送成功的，我们主动关闭客户端
                Console.WriteLine($"send error:{e.Message}");   //6.6 将出错的消息打印出来
            }
        }


    }
}
