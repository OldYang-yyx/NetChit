using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class PlayerData       //S1.1，新建一个类，用来存放用户数据，和用户的管理， 一般我们会用数据库 但现在临时用一个类来存放
    {
        private static PlayerData instance = new PlayerData();
        public static PlayerData Instance => instance;
        //下面我们来模拟数据库的功能，在这里定义一个字典
        Dictionary<string, RegisterMsgS2C> userMsg = new Dictionary<string, RegisterMsgS2C>();   //key：就是客户发过来的账号，Value：存储的信息就是我们要返回改客户端的
        
        public bool Contain(string account)
        {
            return userMsg.ContainsKey(account);        //判断是否存再相同账户，如果account(账户)在字典中存再就返回一个true，如果不存在就返回一个false
        }

        public RegisterMsgS2C Add(RegisterMsgC2S msg)       //我们需要一个接口，客户端会将RegisterMsgC2S的数据返回给我们
        {
            var item = new RegisterMsgS2C();
            userMsg[msg.account] = item;
            item.account = msg.account;
            item.password = msg.password;
            item.result = 0;
            return item;
        }

        //我们的流程就是先 Contain 看看有没有一样的账号，没有的话就调用add这个方法，把RegisterMsgS2C类型的实例“item”返回给客户端就可以了

        //维护已经登录的用户
        Dictionary<string, Client> LoginUser = new Dictionary<string, Client>();
        public void AddLogginUser(string account,Client client)     //添加用户接口，一个是账号，一个是用户 ，通过账号存储用户就好
        {
            LoginUser[account] = client;
        }

        public Dictionary<string, Client> GetAllLoginUser()         //获取所有的客户端
        {
            return LoginUser;
        }

    }
}
