using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.HabitatHome.Foundation.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sitecore.HabitatHome.Foundation.Search.Serialization
{
    public class DocumentConverter : JsonConverter
    {
        private static readonly object[] EmptyObjectArray = new object[0];

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Document).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Document document = new Document();
            foreach (JProperty property in serializer.Deserialize<JObject>(reader).Properties())
            {
                if (!property.Name.StartsWith("@search.", StringComparison.Ordinal))
                {
                    object obj = property.Value is JArray array ? ConvertArray(array, serializer) : ConvertToken(property.Value, serializer);
                    document[property.Name] = obj;
                }
            }
            return document;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static object ConvertToken(JToken token, JsonSerializer serializer)
        {
            switch (token)
            {
                //case JObject jobject:
                //    JTokenReader jtokenReader = new JTokenReader(jobject);
                //    if (!GeographyPointConverter.IsGeoJson(jobject))
                //        return serializer.Deserialize<Document>(jtokenReader);
                //    return (object)serializer.Deserialize<GeographyPoint>(jtokenReader);
                default:
                    return token.ToObject(typeof(object), serializer);
            }
        }

        private static Array ConvertArray(JArray array, JsonSerializer serializer)
        {
            if (array.Count < 1)
                return EmptyObjectArray;
            switch (array[0].Type)
            {
                //case JTokenType.Object:
                //    if (!GeographyPointConverter.IsGeoJson((JObject)array[0]))
                //        return ConvertToArrayOfReferenceType<Document>(false);
                //    return ConvertToArrayOfReferenceType<GeographyPoint>(false);
                case JTokenType.Integer:
                    return new long[0]; // ConvertToArrayOfValueType<long>();
                case JTokenType.Float:
                    return new double[0];// ConvertToArrayOfValueType<double>();
                case JTokenType.String:
                case JTokenType.Null:
                    return ConvertToArrayOfReferenceType<string>(true);
                case JTokenType.Boolean:
                    return new bool[0];// ConvertToArrayOfValueType<bool>();
                case JTokenType.Date:
                    return new DateTimeOffset[0];// ConvertToArrayOfValueType<DateTimeOffset>();
                default:
                    return array.Select(t => ConvertToken(t, serializer)).ToArray();
            }

            Tuple<bool, T> ConvertToReferenceType<T>(JToken token, bool allowNull) where T : class
            {
                switch (ConvertToken(token, serializer))
                {
                    case T obj:
                        return Tuple.Create<bool, T>(true, obj);
                    case null:
                        if (allowNull)
                            return Tuple.Create<bool, T>(true, default(T));
                        break;
                }
                return Tuple.Create<bool, T>(false, default(T));
            }

            Array ConvertToArrayOfType<T>(Func<JToken, Tuple<bool, T>> convert)
            {
                T[] objArray = new T[array.Count];
                for (int count = 0; count < objArray.Length; ++count)
                {
                    JToken jtoken = array[count];
                    Tuple<bool, T> tuple = convert(jtoken);
                    bool flag = tuple.Item1;
                    T obj = tuple.Item2;
                    if (flag)
                    {
                        objArray[count] = obj;
                    }
                    else
                    {
                        IEnumerable<object> second = array.Skip(count).Select(t => ConvertToken(t, serializer));
                        return ((IEnumerable<T>)objArray).Take<T>(count).Cast<object>().Concat(second).ToArray();
                    }
                }
                return objArray;
            }

            Array ConvertToArrayOfReferenceType<T>(bool allowNull) where T : class
            {
                return ConvertToArrayOfType<T>((Func<JToken, Tuple<bool, T>>)(t => ConvertToReferenceType<T>(t, allowNull)));
            }

            //Array ConvertToArrayOfValueType<T>() where T : struct
            //{
            //    return ConvertToArrayOfType<T>(new Func<JToken, Tuple<bool, T>>(t => ConvertArray<T>(t, null) ConvertToValueType<T>(t, null)));
            //}
    }
}
}