using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonNetTest
{
    public class TestData
    {
        public TestData()
        {
            this.Dictionary=new Dictionary<int, TestDataItem>();
        }

        public int? Test { get; set; }

        public Dictionary<int,TestDataItem> Dictionary { get;  }
    }

    public class TestDataItem
    {
        public bool Boolean { get; set; }

        public int? NullableInt { get; set; }

        public string Text { get; set; }
    }
}
