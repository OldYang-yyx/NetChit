using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    internal class HttpServer
    {
        string page = "http://127.0.0.1:8080/";
        HttpListener httpListener;
        public void Start() {

            httpListener=new HttpListener();
            httpListener.Prefixes.Add(page);
            httpListener.Start();
            Console.WriteLine("http server start");
            //HandleGet();
            HandlePost();
        }

        //处理Get请求
        public async void HandleGet() {

            var context= await httpListener.GetContextAsync();
            var text = context.Request.RawUrl;
            Console.WriteLine(context.Request.RawUrl);
            var stream =  context.Response.OutputStream;
            if (text.Contains("/login"))
            {
                byte[] data= Encoding.UTF8.GetBytes("登录成功");
                await stream.WriteAsync(data, 0, data.Length);
                stream.Close();
            }
            else if (text.Contains("/download"))
            {
                
            }
            HandleGet();
        }

        //处理Post请求
        public async void HandlePost()
        {
            HttpListenerContext context =
           await httpListener.GetContextAsync();

            Stream inpuStream = context.Request.InputStream;
            byte[] readData = new byte[context.Request.ContentLength64];
            int length = await inpuStream.ReadAsync(readData,
                0, readData.Length);
            inpuStream.Close();

            string text = Encoding.UTF8.GetString(readData);
            Console.WriteLine("请求的内容:" + text);
            Stream stream = context.Response.OutputStream;
            if (text.Contains("login"))
            {
                Console.WriteLine("收到客户端的登录请求");
                byte[] data = Encoding.UTF8.GetBytes("登录成功");
                //await stream.WriteAsync("登录成功");
                await stream.WriteAsync(data, 0, data.Length);
                //每次都需要调用close 才能够将数据返回给客户端
                stream.Close();
            }
            else if (text.Contains("download"))
            {
                //1.获取到路径下的文件 byte[] 
                //2.将它读取后 响应给客户端
                //3.客户端拿到之后->写入到本地就可以了
                string path = @"D:\NetProject\DB\unity.png";
                FileStream file = new FileStream(path, FileMode.Open);
                byte[] data = new byte[file.Length];
                int fileLength = await file.ReadAsync(data, 0, data.Length);
                Console.WriteLine("文件大小:" + fileLength);
                //1M=1024K 1K=1024b
                await stream.WriteAsync(data, 0, data.Length);
                //每次都需要调用close 才能够将数据返回给客户端
                stream.Close();
            }
            HandlePost();

        }
     }
}
