using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Client.Instance.Start();

        var loginPrefab = Resources.Load<GameObject>("LoginView");
        var loginView = GameObject.Instantiate<GameObject>(loginPrefab);
        loginView.AddComponent<LoginView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Client.Instance.Send(Encoding.UTF8.GetBytes("login..."));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            LoginS2C loginS2C = new LoginS2C();
            loginS2C.Account = "xxx";
            loginS2C.Password = "yyy";
            //List<int> test = loginS2C.Test.ToList();
            loginS2C.Test.Add(1);
            loginS2C.Test.Add(2);
            loginS2C.Test.Add(3);
            loginS2C.Test.Add(4);
            byte[] data= ProtoHelper.ToBytes(loginS2C);
            Debug.Log(data.Length);

            //
            LoginS2C loginS2C_test = ProtoHelper.ToObject<LoginS2C>(data);
            var act = loginS2C_test.Account;
            var pwd = loginS2C.Password;
            Debug.Log(act+"  "+pwd);

            //json字符串 byte[]数组
            //自定义class

            //proto -> byte
            //message 2 .cs
        }
       



    }
}
