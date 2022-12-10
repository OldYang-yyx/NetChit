using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class MessageHelper
{
}


//1002
public class LoginMsgC2S
{
    public string account;
    public string password;
}

public class LoginMsgS2C
{

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

