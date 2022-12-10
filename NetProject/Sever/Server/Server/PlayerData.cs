using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Net;

namespace Server
{
    internal class PlayerData
    {
        static PlayerData instance = new PlayerData();
        public static PlayerData Instance => instance;

        Dictionary<string, RegisterMsgS2C> userMsg = new Dictionary<string, RegisterMsgS2C>();

        public RegisterMsgS2C Add(RegisterMsgC2S msg)
        {
            var item = new RegisterMsgS2C();
            userMsg[msg.account] = item;
            item.account = msg.account;
            item.password= msg.password;
            item.result = 0;
            return item;
        }

        //判断是否存在相同的账号
        public bool Contain(string account) { 
            return userMsg.ContainsKey(account); ;
        }

        //维护已经登录的用户
        Dictionary<string, Client> LoginUser = new Dictionary<string, Client>();
        public void AddLoginUser(string account,Client client) {
            LoginUser[account] = client;  
        }

        public Dictionary<string, Client> GetAllLoginUser() {
            return LoginUser;
        }
    }
}
