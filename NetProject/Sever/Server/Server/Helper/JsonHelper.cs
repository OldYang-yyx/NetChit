using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using Newtonsoft.Json;

namespace Server.Helper
{
    internal class JsonHelper
    {
        public static string ToJson(object x) {

            var json=JsonConvert.SerializeObject(x);
            return json;

            string str = JsonMapper.ToJson(x);
            return str;
        }

        public static T ToObject<T>(string x) {
            return JsonConvert.DeserializeObject<T>(x);

            return JsonMapper.ToObject<T>(x);
        }

        public static T ToObject<T>(byte[] b) { 
            string x=Encoding.UTF8.GetString(b, 0, b.Length);
            return ToObject<T>(x);
        }

        public static string GetTestToString() {
            JsonTest jsonTest= new JsonTest();
            jsonTest.id = 1;
            jsonTest.name = "jsonTest";
            var jsonStr= ToJson(jsonTest);

            var jsonTestObj= ToObject<JsonTest>(ToJson(jsonTest));
            Console.WriteLine($"{jsonTestObj.id}   /  {jsonTestObj.name}");
            return jsonStr;

        }
    }

    public class JsonTest {
        public int id;
        public string name;
    }


}
