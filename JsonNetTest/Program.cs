using Newtonsoft.Json;
using System.Diagnostics;

namespace JsonNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test1();
            //Test2();
            Test3();
            Test4();
        }

        private static void Test1()
        {
            var d = new TestData();
            d.DictionaryWriteable.Add(1, new TestDataItem() { Boolean = true });

            string json = JsonConvert.SerializeObject(d);

            var dDs = JsonConvert.DeserializeObject<TestData>(json);

            var dDsA = JsonConvert.DeserializeObject(json);
        }

        private static void Test2()
        {
            var d = new TestData();
            var tdi = new TestDataItem() { Boolean = true };
            tdi.ListWriteable.Add(new RgbNormalized() { R = 1, G = 0, B = 1 });
            d.DictionaryWriteable.Add(1, tdi);

            string json = JsonConvert.SerializeObject(d);

            var dDs = JsonConvert.DeserializeObject<TestData>(json);

            var dDsA = JsonConvert.DeserializeObject(json);
        }

        private static void Test3()
        {
            var d = new TestData();
            var tdi = new TestDataItem() { Boolean = true };
            tdi.ListWriteable.Add(new RgbNormalized() { R = 1, G = 0, B = 1 });
            tdi.ListWriteable.Add(new RgbNormalized() { R = 0, G = 1, B = 1 });
            d.DictionaryWriteable.Add(1, tdi);

            string json = JsonConvert.SerializeObject(
                d,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,   // ignore (=do not write) 'null' values, which works also fine for nullable types
                    Formatting = Formatting.Indented
                });

            var dDs = JsonConvert.DeserializeObject<TestData>(json);

        }

        private static void Test4()
        {
            var d = new TestData() { Test = 42 };
            var tdi = new TestDataItem() { Boolean = true };
            tdi.ListWriteable.Add(new RgbNormalized() { R = 1, G = 0, B = 1 });
            tdi.ListWriteable.Add(new RgbNormalized() { R = 0, G = 1, B = 1 });
            tdi.Text = "Testtext";
            d.DictionaryWriteable.Add(1, tdi);

            string json = JsonConvert.SerializeObject(
                d,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,   // ignore (=do not write) 'null' values, which works also fine for nullable types
                    Formatting = Formatting.Indented
                });


            var dDsA = JsonConvert.DeserializeObject(json);

            var testDataOnJObject = TestDataOnJObject.Create(dDsA);

            var v = testDataOnJObject.Test;
            Debug.Assert(v == 42);

            var dict = testDataOnJObject.Dictionary;

            var c = dict[1];
            Debug.Assert(c.Text == "Testtext");
            var ll = c.MyList;
        }
    }
}
