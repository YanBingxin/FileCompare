using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FilesCompare.CompareHelper
{
    public static class XmlHelper
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadFromFile<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            using (Stream stream = File.OpenRead(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="o"></param>
        /// <param name="fileName"></param>
        public static void SaveToFile(object o, string fileName)
        {
            using (Stream stream = File.Create(fileName))
            {
                new XmlSerializer(o.GetType()).Serialize(stream, o);
            }
        }
    }
}
