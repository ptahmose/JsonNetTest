using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = new TestData();
            d.Dictionary.Add(1,new TestDataItem(){Boolean = true});

            string json = JsonConvert.SerializeObject(d);

            var dDs = JsonConvert.DeserializeObject<TestData>(json);

            var dDsA = JsonConvert.DeserializeObject(json);


        }
    }
}
