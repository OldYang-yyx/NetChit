using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData             //Ŀ�������������û����ݵ�
{
    static PlayerData instance = new PlayerData();
    public static PlayerData Instance => instance;

    public LoginMsgS2C loginMsgS2C;

}
