using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client_WebRequest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Get("login"));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Post("login"));
            //StartCoroutine(Post("download"));
        }
    }

    public IEnumerator Get(string data) {
        //PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
        var request = UnityWebRequest.Get("http://127.0.0.1:8080/" + data);
        Debug.Log($"������Get����:{data}");
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
    }


    public IEnumerator Post(string data)
    {
        //Unity2022����,��Ҫ��http��������,��Ҫ�������.��Ϊ����Ĭ�Ϲر��˷ǰ�ȫ��http����.
        //PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;

        var request = UnityWebRequest.Post("http://localhost:8080/", data);
        Debug.Log($"����Post����:{data}");
        request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
    }
}
