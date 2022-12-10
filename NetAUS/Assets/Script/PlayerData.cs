using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData             //目的是用来缓存用户数据的
{
    static PlayerData instance = new PlayerData();
    public static PlayerData Instance => instance;

    public LoginMsgS2C loginMsgS2C;

}
