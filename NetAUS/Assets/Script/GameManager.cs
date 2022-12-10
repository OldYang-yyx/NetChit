using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Client.Instance.Start();        //1.21 ��������ͻ������ӷ�����

        var loginPrefab = Resources.Load<GameObject>("LoginView");  //��ʼ��Ŀʱ����¼�������ɳ���
        var loginView = GameObject.Instantiate<GameObject>(loginPrefab);
        loginView.AddComponent<LoginView>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Client.Instance.Send(Encoding.UTF8.GetBytes("login...."));    //2.2 ���ǵ���Client��Send(��������)��������Ϊ��������byte���飬����������Ҫ��Encoding������ת��
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
