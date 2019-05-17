using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PokerCardPlay.Core
{
    /// <summary>
    /// XML序列化工具。
    /// </summary>
    public class XmlSerilzerTool
    {
        /// <summary>
        /// 将对象序列化成 Xml 字符串。
        /// </summary>
        /// <param name="source">要序列化成 Xml 字符串的对象实例。</param>
        /// <param name="containsDeclaration">指定是否包含文档声明及命名空间声明。</param>
        /// <returns>表示对象信息的 Xml 字符串。</returns>
        public static string Serializer(Object source, bool containsDeclaration = false)
        {

            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = !containsDeclaration
            };
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                try
                {
                    new XmlSerializer(source.GetType()).Serialize(writer, source);
                }
                catch (Exception ex )
                {       
                   System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                
            }
            return output.ToString();
        }



        /// <summary>
        /// 反序列化指定的 XML 字符串。
        /// </summary>
        /// <typeparam name="T">反序列化后的对象的类型。</typeparam>
        /// <param name="xml">包含要反序列化的 XML 文档。</param>
        /// <returns>正被反序列化的 <typeparamref name="T"/> 的对象实例。</returns>
        /// <exception cref="InvalidOperationException">反序列化期间发生错误。使用 <see cref="System.Exception.InnerException"/> 属性时可使用原始异常。</exception>
        public static T Deserialize<T>(string xml)
        {
            return (T)Deserialize(typeof(T), xml);
        }

        /// <summary>
        /// 反序列化指定的 XML 字符串。
        /// </summary>
        /// <param name="type">反序列化后的对象的类型。</param>
        /// <param name="xml">包含要反序列化的 XML 文档。</param>
        /// <returns>正被反序列化的 <see cref="Object">System.Object</see>。</returns>
        /// <exception cref="InvalidOperationException">反序列化期间发生错误。使用 <see cref="System.Exception.InnerException"/> 属性时可使用原始异常。</exception>
        public static object Deserialize(Type type, string xml)
        {
            if (null == type)
                throw new ArgumentNullException("xml");

            if (null == xml)
                return null;

            using (StringReader reader = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(reader);
            }
        }
    }
}
