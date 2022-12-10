using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Client.Instance.Start();        //1.21 启动这个客户端连接服务器

        var loginPrefab = Resources.Load<GameObject>("LoginView");  //开始项目时将登录界面生成出来
        var loginView = GameObject.Instantiate<GameObject>(loginPrefab);
        loginView.AddComponent<LoginView>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Client.Instance.Send(Encoding.UTF8.GetBytes("login...."));    //2.2 我们调用Client的Send(发送数据)方法，因为括号内是byte数组，所以我们需要用Encoding将内容转换
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            JsonTest jsonTest = new JsonTest();
            jsonTest.id = 2;
            jsonTest.name = "jsonTest2222";
            var jsonStr = JsonHelper.ToJson(jsonTest);
            Client.Instance.Send(Encoding.UTF8.GetBytes(jsonStr));
        }
    }
}
