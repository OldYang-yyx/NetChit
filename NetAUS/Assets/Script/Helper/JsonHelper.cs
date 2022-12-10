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
        //������Ҫ�����ӿ�
        public static string ToJson(object x)       //1.1һ���ӿھͽ����ǵ�Objectת����Json��ʽ���ַ���
        {
            string str = JsonMapper.ToJson(x);      //1.2����ֱ�ӵ���JsonMapper.ToJson()�Ϳ����ˣ����᷵��һ���ַ���
            return str;                             //1.3Ȼ�����ǽ��ַ������س�ȥ�Ϳ�����
        }

        public static T ToObject<T>(string x)       // 1.4�ڶ����ӿڣ����ַ���ת����object,����ʹ�÷��ͣ��ɴ����������͵Ķ���
        {
            return JsonMapper.ToObject<T>(x);       //1.5��ת���ɵĶ���ֱ�ӷ��س�ȥ
        }

        public static T ToObject<T>(byte[] b)       //1.6��������õ��ֽ����������£�������Ҫ����ת��Ϊstring���ͣ��ڽ���ת�� ���ǿ���дһ������
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

