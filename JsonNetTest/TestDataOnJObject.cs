using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonNetTest
{
    public class TestDataOnJObject : ITestData
    {
        private JObject obj;
        private Dictionary<int, ITestDataItem> dict;
        private bool dictionaryConstructionAlreadyAttempted;

        public static ITestData Create(object o)
        {
            return new TestDataOnJObject(o as JObject);
        }

        public TestDataOnJObject(JObject obj)
        {
            this.obj = obj;
        }

        public int? Test
        {
            get
            {
                JToken token;
                if (!this.obj.TryGetValue(nameof(this.Test), out token))
                {
                    return null;
                }

                if (token.Type == JTokenType.Integer)
                {
                    return token.Value<int>();
                }

                return null;
            }
        }

        public IReadOnlyDictionary<int, ITestDataItem> Dictionary
        {
            get
            {
                if (this.dictionaryConstructionAlreadyAttempted == false)
                {
                    this.dict = this.GenerateDictionary();
                    this.dictionaryConstructionAlreadyAttempted = true;
                }

                return this.dict;
            }
        }

        private Dictionary<int, ITestDataItem> GenerateDictionary()
        {
            JToken token;
            if (!this.obj.TryGetValue(nameof(this.Dictionary), out token))
            {
                return null;
            }

            if (token.Type == JTokenType.Object)
            {
                var c = token.Value<IDictionary<string, JToken>>();

                var d = new Dictionary<int, ITestDataItem>(c.Count);
                foreach (var stringAndToken in c)
                {
                    int key = Convert.ToInt32(stringAndToken.Key);
                    var value = TestDataItemOnJObject.Create(stringAndToken.Value);
                    d.Add(key, value);
                }

                if (d.Count > 0)
                {
                    return d;
                }
            }

            return null;
        }
    }

    public class TestDataItemOnJObject : ITestDataItem
    {
        private JObject obj;

        private List<RgbNormalized> list;
        private bool listConstructionAlreadyAttempted;

        public static ITestDataItem Create(object o)
        {
            return new TestDataItemOnJObject(o as JObject);
        }

        public TestDataItemOnJObject(JObject obj)
        {
            this.obj = obj;
        }

        public bool Boolean
        {
            get
            {
                JToken token;
                if (!this.obj.TryGetValue(nameof(this.Boolean), out token))
                {
                    throw new InvalidOperationException("This value is not present.");
                }

                if (token.Type == JTokenType.Boolean)
                {
                    return token.Value<bool>();
                }

                throw new InvalidOperationException("This value is not present as a boolean.");
            }
        }

        public int? NullableInt
        {
            get
            {
                JToken token;
                if (!this.obj.TryGetValue(nameof(this.NullableInt), out token))
                {
                    return null;
                }

                if (token.Type == JTokenType.Integer)
                {
                    return token.Value<int>();
                }

                return null;
            }
        }

        public string Text
        {
            get
            {
                JToken token;
                if (!this.obj.TryGetValue(nameof(this.Text), out token))
                {
                    return null;
                }

                if (token.Type == JTokenType.String)
                {
                    return token.Value<string>();
                }

                return null;
            }
        }

        public IReadOnlyList<RgbNormalized> MyList
        {
            get
            {
                if (this.listConstructionAlreadyAttempted == false)
                {
                    this.list = this.ConstructList();
                    this.listConstructionAlreadyAttempted = true;
                }

                return this.list;
            }
        }

        private List<RgbNormalized> ConstructList()
        {
            JToken token;
            if (!this.obj.TryGetValue(nameof(this.MyList), out token))
            {
                return null;
            }

            if (token.Type == JTokenType.Array)
            {
                var jl = token.Values<JToken>().ToList();
                List<RgbNormalized> l = new List<RgbNormalized>(jl.Count);

                for (int i = 0; i < jl.Count / 3; ++i)
                {
                    var rgb = new RgbNormalized()
                    {
                        R = (float)jl[i*3].Value<float>(),
                        G = (float)jl[i*3+1].Value<float>(),
                        B = (float)jl[i*3+2].Value<float>()
                    };

                    l.Add(rgb);
                }

                if (l.Count > 0)
                {
                    return l;
                }
            }

            return null;
        }
    }
}
