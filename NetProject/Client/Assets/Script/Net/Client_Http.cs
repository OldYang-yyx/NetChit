using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using UnityEngine;

public class Client_Http : MonoBehaviour
{
    HttpClient client;
    // Start is called before the first frame update
    void Start()
    {
        client = new HttpClient();
        
    }

    public async void Get(string data)
    {
        string page = "http://127.0.0.1:8080/" + data;
        HttpResponseMessage response = await client.GetAsync(page);
        Debug.Log("http  get:" + data);
        string text = await response.Content.ReadAsStringAsync();
        Debug.Log("http get. 返回的数据是:" + text);
    }


    public async void Post(string data)
    {
        string page = "http://127.0.0.1:8080/";
        StringContent stringContent = new StringContent(data);

        Dictionary<string, string> dct = new Dictionary<string, string>();
        dct["account"] = "xxx";
        dct["pwd"] = data;
        //account&xxx
        FormUrlEncodedContent content = new FormUrlEncodedContent(dct);
        //var responese= await client.PostAsync(page, stringContent);
        var responese = await client.PostAsync(page, content);

        if (data=="download")
        {
            //下载文件 不同的处理
            //读取到的文件数据
            byte[] fileData = await responese.Content.ReadAsByteArrayAsync();
            string path = @"D:\NetProject\Dowload\unity.png";
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
            await fileStream.WriteAsync(fileData, 0, fileData.Length);
            Debug.Log("下载完毕了");
        }
        else 
        {
            string str= await responese.Content.ReadAsStringAsync();
            Debug.Log($"post responese:{str}");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Get("login");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            //Post("login");
            Post("download");
        }
    }


}
