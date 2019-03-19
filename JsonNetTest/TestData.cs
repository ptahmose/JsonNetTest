using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Converters;

namespace JsonNetTest
{
    public class TypeConverter<T, TSerialized> : CustomCreationConverter<T>
        where TSerialized : T, new()
    {
        public override T Create(Type objectType)
        {
            return new TSerialized();
        }
    }

    public interface ITestData
    {
        int? Test { get; }

        IReadOnlyDictionary<int, ITestDataItem> Dictionary { get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class TestData : ITestData
    {
        /// <summary>
        /// The dictionary (which we instruct to serialize).
        /// This magic is trickery here is explained here -> https://stackoverflow.com/questions/40620323/using-json-net-converter-to-deserialize-dictionary-with-interface-value .
        /// The root problem we have to work around here is that IDictionary is not covariant -> https://stackoverflow.com/questions/13593900/how-to-get-around-lack-of-covariance-with-ireadonlydictionary .
        /// </summary>
        [JsonProperty(PropertyName = "Dictionary", ItemConverterType = typeof(TypeConverter<ITestDataItem, TestDataItem>))]
        private Dictionary<int, ITestDataItem> dictionary = new Dictionary<int, ITestDataItem>();

        [JsonProperty]
        public int? Test { get; set; }

        public Dictionary<int, ITestDataItem> DictionaryWriteable
        {
            get { return this.dictionary; }
        }

        public IReadOnlyDictionary<int, ITestDataItem> Dictionary { get { return this.dictionary; } }
    }

    public struct RgbNormalized
    {
        //[JsonIgnoreAttribute()]
        public float R;
        //[JsonIgnoreAttribute()]
        public float G;
        //[JsonIgnoreAttribute()]
        public float B;

        //public float[] Rgb
        //{
        //    get { return new float[] { this.R, this.G, this.B }; }
        //    set
        //    {
        //        this.R = value[0];
        //        this.G = value[1];
        //        this.B = value[2];
        //    }
        //}
    }

    public interface ITestDataItem
    {
        bool Boolean { get; }

        int? NullableInt { get; }

        string Text { get; }

        IReadOnlyList<RgbNormalized> MyList { get; }
    }

    [JsonObject(MemberSerialization.OptIn)] // instruct to only deal with explictly marked properties
    public class TestDataItem : ITestDataItem
    {
        private List<RgbNormalized> list;

        public TestDataItem()
        {
            this.list = new List<RgbNormalized>();
        }

        [JsonProperty]
        public bool Boolean { get; set; }

        [JsonProperty]
        public int? NullableInt { get; set; }

        [JsonProperty]
        public string Text { get; set; }

        public IReadOnlyList<RgbNormalized> MyList
        {
            get { return this.list; }
        }

        public List<RgbNormalized> ListWriteable
        {
            get { return this.list; }
        }

        /// <summary>
        /// Gets or sets the list "MyList" as an array of floats.
        /// If serializing IReadOnlyList&lt;RgbNormalized&gt; directly, we would end up with something like
        /// <code>
        /// "MyList": [
        ///        {
        ///            "R": 1.0,
        ///            "G": 0.0,
        ///            "B": 1.0
        ///        },
        ///        {
        ///            "R": 0.0,
        ///            "G": 1.0,
        ///            "B": 1.0
        ///        }
        ///     ]
        /// </code>
        /// This is considered to be to chatty and too wasteful. So, the idea is to serialize the data as a plain
        /// array of floats, where three consecutive floats give one RGB-struct (with 1st giving 'R', 2nd gives 'G' and
        /// 3rd gives 'B'). If the number of floats is not a multiple of 3, then the remaining elements are discarded.
        /// The serialization now looks like this:
        /// <code>
        ///       "List": [
        ///        1.0,
        ///        0.0,
        ///        1.0,
        ///        0.0,
        ///        1.0,
        ///        1.0
        ///        ]
        /// </code>
        /// Maybe there is a better way to achieve this, a drawback of this embodiment is that the data gets copied.
        /// A "JsonConverter" (https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonConverter.htm) might be what
        /// also could do the job.
        /// </summary>
        /// <value>
        /// The data from the property "MyList" as a plain array of floats.
        /// </value>
        [JsonProperty("MyList")]  // the name of the JSON-property to store this property is specified as "MyList"
        private List<float> ListAsFloats
        {
            get
            {
                if (this.list == null || this.list.Count == 0)
                {
                    return null;

                }

                var l = new List<float>();
                foreach (var rgb in this.list)
                {
                    l.Add((rgb.R)); l.Add((rgb.G)); l.Add((rgb.B));
                }

                return l;
            }

            set
            {
                if (value == null)
                {
                    this.list = null;
                }
                else
                {
                    var l = new List<RgbNormalized>();

                    if (value.Count % 3 != 0)
                    {
                        Debug.WriteLine("List is expected to have an element-count which is a multiple of 3.");
                    }

                    for (int i = 0; i < value.Count / 3; ++i)
                    {
                        l.Add(new RgbNormalized() { R = value[i * 3], G = value[i * 3 + 1], B = value[i * 3 + 2] });
                    }

                    this.list = l.Count > 0 ? l : null;
                }
            }
        }
    }
}
