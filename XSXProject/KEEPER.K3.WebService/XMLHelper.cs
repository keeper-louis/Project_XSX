using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace KEEPER.K3.WebService
{
    public class XMLHelper
    {
        private XMLHelper()
        {

        }

        private static XMLHelper _XMLHelper;
        private static readonly object _LockObj = new object();
        public static XMLHelper Instance
        {
            get
            {
                lock (_LockObj)
                {
                    if (_XMLHelper == null)
                    {
                        _XMLHelper = new XMLHelper();
                    }
                    return _XMLHelper;
                }
            }
        }

        /// <summary>
        /// 将XML对象保存到本地
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="fileName"></param>
        public void SaveToLocal<T>(T t, string fileName) where T : class
        {
            var filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\LocalXML\";

            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            if (System.IO.File.Exists(filePath + fileName + ".XML"))
            {
                System.IO.File.Delete(filePath + fileName + ".XML");
            }

            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(filePath + fileName + ".XML"))
                {
                    System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    ser.Serialize(sw, t);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteExceptionLog(ex, ex.Message);
            }
        }

        /// <summary>
        /// 从本地获取序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public T GetLocalXML<T>(string fileName) where T : class
        {
            var filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\LocalXML\";

            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            if (!System.IO.File.Exists(filePath + fileName + ".XML"))
            {
                return default(T);
            }
            else
            {
                var returnT = default(T);
                try
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath + fileName + ".XML"))
                    {
                        System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
                        returnT = (T)ser.Deserialize(sr);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.WriteExceptionLog(ex, ex.Message);
                }
                return returnT;
            }
        }

        public void ModifySetting(List<Tuple<string, string>> keyValue)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                string fileName = HttpContext.Current.Request.MapPath("web.config");
                LogHelper.Instance.WriteTraceLog(fileName);
                doc.Load(fileName);
                XmlNode node;

                node = doc.SelectSingleNode("//appSettings");

                foreach (var item in keyValue)
                {
                    XmlElement element;
                    element = (XmlElement)node.SelectSingleNode("//add[@key='" + item.Item1 + "']");
                    element.SetAttribute("value", item.Item2);
                }

                doc.Save(HttpContext.Current.Request.MapPath("web.config"));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteExceptionLog(ex, "修改配置文件错误");
            }
        }
    }
}