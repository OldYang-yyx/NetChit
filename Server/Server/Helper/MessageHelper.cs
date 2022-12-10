using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class MessageHelper
{ }

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


