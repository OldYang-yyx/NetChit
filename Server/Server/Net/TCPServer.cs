using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Server.Net
{

    //TCP服务端的管理类
    internal class TCPServer
    {
        TcpListener tcpListener;
        //----------------启动服务端，创建监听器-------------------------
        //1.1我们需要两个接口，一个是启动服务器的接口，一个是监听客户端连接的接口
        public void Start()
        {
            try                                         //1.4 在用try Catch进行捕捉，看看在启动时候有没有出现异常
            {
                //1.2.我们要创建一个监听器
                tcpListener = TcpListener.Create(7788); //1-65535   

                tcpListener.Start(500);                 //1.3.启动监听,()客户里面可以填入这样一个重载，表示他最大可接收多少客户端的连接

                Console.WriteLine("TCP Server Start!");//1.3.1我们可以打印一条日志，表示TCP Server 已经启动了！

                Accpet();                              //4.11这里启动服务器之后，我们要进行调用监听方法，不然是没办法进行监听的


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);           //1.4.1 如果有我们就给他打印出来
            }
        }
        public Client tempClient;                   //new 缓存客户端

        //----------------监听客户端的连接-------------------------
        //1.1.1监听客户端连接的接口
        //2.2 因为方法需要等待监听，所有我们讲方法改为异步的 加async关键字 。里面才可以用await等待
        public async void Accpet()
        {
            try                                                                             //3.9 在用try Catch进行捕捉，看看在接收连接时候有没有出现异常
            {
                //2.1 进行监听客户端的连接

                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();             //2.3 等他监听到客户端返回之后呢，我们要得到一个TcpClient对象

                Console.WriteLine("客户端已连接：" + tcpClient.Client.RemoteEndPoint);      //2.4 得到TcpClient对象之后呢，我们可以把连接过来的用户的IP打印出来

                //3.1下面我们要新建一个类。构建一个客户端类来缓存监听到的tcpClient(连接服务端的客户端)

                Client client = new Client(tcpClient);                                      //3.7我们在这里构建一个Client，把返回的tcpClient传进去
                tempClient = client;        			//new 让缓存的客户端等于当前连接的客户端

                Accpet();                                                               //3.8之后让本方法继续接收来自客户端的连接
            }
            catch (Exception e)                                                           
            {
                Console.WriteLine($"Accpet:{e.Message}");  //打印错误
                tcpListener.Stop();                                                     //3.10 当遇到错误的时候   停止监听客户端的连接
            }
        }
    }
}