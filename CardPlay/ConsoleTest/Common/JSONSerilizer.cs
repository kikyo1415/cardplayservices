using System;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common;

namespace PokerCardPlay.Core
{
    /// <summary>
    /// JSON序列化工具类。
    /// </summary>
    public class JSONSerilizer
    {
        /// <summary>
        /// 将对象实体转换为JSON字符串。
        /// </summary>
        /// <param name="source">需要转换的对象实体。</param>
        /// <returns>转换后的JSON字符串。</returns>
        public static string ToJSON(Object source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            try
            {
                string xml = XmlSerilzerTool.Serializer(source);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.Trim());
                var result = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("对象转换失败，" + ex.Message);
            }

        }

        /// <summary>
        /// 将JSON字符串转换为指定类型的实体。
        /// </summary>
        /// <typeparam name="T">实体指定需要转换到的类型。</typeparam>
        /// <param name="source">JSON字符串。</param>
        /// <returns>形参<see cref="T"/>指定类型的实体。</returns>
        public static T ToObject<T>(string source) where T : class
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            try
            {
                var objXML = JsonConvert.DeserializeXmlNode(source);
                if (objXML == null)
                    throw new Exception("对象转换失败");
                T result = XmlSerilzerTool.Deserialize<T>(objXML.OuterXml);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("对象转换失败，" + ex.Message);
            }

        }

        /// <summary>
        /// 将JSON字符串转换为指定类型的实体。
        /// </summary>
        /// <param name="type">实体指定需要转换到的类型。</param>
        /// <param name="source">JSON字符串。</param>
        /// <returns>Object</returns>
        public static Object ToObject(Type type, string source)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            try
            {
                var objXML = JsonConvert.DeserializeXmlNode(source);
                if (objXML == null)
                    throw new Exception("对象转换失败");
                Object result = XmlSerilzerTool.Deserialize(type, objXML.OuterXml);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("对象转换失败，" + ex.Message);
            }

        }

        /// <summary>
        /// 将JSON字符串转换为的XML字符串。
        /// </summary>
        /// <param name="source">JSON字符串。</param>
        /// <returns>XML字符串。</returns>
        public static string ToXmlString(string source)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            try
            {
                var objXML = JsonConvert.DeserializeXmlNode(source);
                if (objXML == null)
                    throw new Exception("对象转换失败");
                return objXML.OuterXml;
            }
            catch (Exception ex)
            {
                throw new Exception("对象转换失败，" + ex.Message);
            }

        }

        public static string GetJsonNodeXml(string source, string nodeName)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            try
            {
                var xml = JsonConvert.DeserializeXmlNode(source);
                var headNode = xml.SelectNodes(string.Format("//{0}", nodeName));
                if (headNode == null || headNode.Count == 0)
                    throw new Exception("未找到回应头。");

                return headNode[0].InnerText;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static string GetJsonNode(string source, string nodeName)
        {
            try
            {
                JObject jsonObj = JObject.Parse(source);
                JToken token = null;
                if (jsonObj.TryGetValue(nodeName, out token))
                    return token.ToString();
                else
                    throw new ArgumentException("nodeName");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
