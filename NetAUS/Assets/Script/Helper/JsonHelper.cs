using System.Collections;
using UnityEngine;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    internal class JsonHelper
    {
        //我们需要两个接口
        public static string ToJson(object x)       //1.1一个接口就将我们的Object转换成Json格式的字符串
        {
            string str = JsonMapper.ToJson(x);      //1.2我们直接调用JsonMapper.ToJson()就可以了，他会返回一个字符串
            return str;                             //1.3然后我们将字符串返回出去就可以了
        }

        public static T ToObject<T>(string x)       // 1.4第二个接口，将字符串转换成object,这里使用泛型，可代表任意类型的对象
        {
            return JsonMapper.ToObject<T>(x);       //1.5将转换成的对象直接返回出去
        }

        public static T ToObject<T>(byte[] b)       //1.6如果我们拿到字节数组的情况下，我们需要将其转换为string类型，在进行转， 我们可以写一个重载
        {
            string x = Encoding.UTF8.GetString(b, 0, b.Length);
            return ToObject<T>(x);
        }

        public static string GetTestToString()
        {
            JsonTest jsonTest = new JsonTest();
            jsonTest.id = 1;
            jsonTest.name = "jsonTest";
            var jsonStr = ToJson(jsonTest);

            var jsonTestObj = ToObject<JsonTest>(ToJson(jsonTest));
            Debug.Log($"{jsonTestObj.id}  /  {jsonTestObj.name}");
            return jsonStr;
        }
    }
    public class JsonTest
    {
        public int id;
        public string name;
    }

