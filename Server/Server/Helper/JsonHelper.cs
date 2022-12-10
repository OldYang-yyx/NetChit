using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Helper
{
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
            string x = Encoding.UTF8.GetString(b, 0, b.Length);         //1.7将字节数组转换为字符串
            return ToObject<T>(x);                  //1.8在调用将字符串转换为object的函数
        }

        public static string GetTestToString()          //2.2测试的接口
        {
            JsonTest jsonTest = new JsonTest();         //2.3创建一个对象，下面两行分别给其对象的变量赋值
            jsonTest.id = 1;
            jsonTest.name = "jsonTest";
            var jsonStr = ToJson(jsonTest);             //2.4测试一下，用ToJson将object转换为Json格式的字符串，装到string类型的变量里

            var jsonTestObj = ToObject<JsonTest>(ToJson(jsonTest));     //2.7反序列化，将Json格式的字符串返序列化成<JsonTest>类型。
            Console.WriteLine($"{jsonTestObj.id}  /  {jsonTestObj.name}");  //2.8这时候jsonTestObj已经是<JsonTest>类型的并带有将Json数据中的信息了
            return jsonStr;
        }
    }
    public class JsonTest                           //2.1测试用的类。里面写几个变量
    {
        public int id;
        public string name;
    }
}
