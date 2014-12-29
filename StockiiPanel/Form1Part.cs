using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace StockiiPanel
{
    public partial class Form1 : Form
    {
        MyProgressBar myBar = new MyProgressBar();

        // 代理定义，可以在Invoke时传入相应的参数
        private delegate void funHandle(int nValue);
        private funHandle myHandle = null;

        /// <summary>
        /// 线程函数中调用的函数
        /// </summary>
        private void ShowProgressBar()
        {
            myHandle = new funHandle(myBar.SetProgressValue);
          //  myBar.MdiParent = this;
            myBar.StartPosition = FormStartPosition.CenterScreen;
            myBar.ShowDialog();
        }

        /// <summary>
        /// 线程函数，用于处理调用
        /// </summary>
        private void ThreadFun()
        {
            MethodInvoker mi = new MethodInvoker(ShowProgressBar);
            this.BeginInvoke(mi);

            System.Threading.Thread.Sleep(500); // sleep to show window

            int i = 0;
            while(!stop)
            {
                ++i;
                System.Threading.Thread.Sleep(50);
                // 这里直接调用代理
                this.Invoke(this.myHandle, new object[] { (i % 101) });
                if (i > 100)
                    i = 0; 
            }
        }

        private bool stop = false;
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public SerializableDictionary() { }
        public void WriteXml(XmlWriter write)       // Serializer
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                write.WriteStartElement("SerializableDictionary");
                write.WriteStartElement("key");
                KeySerializer.Serialize(write, kv.Key);
                write.WriteEndElement();
                write.WriteStartElement("value");
                ValueSerializer.Serialize(write, kv.Value);
                write.WriteEndElement();
                write.WriteEndElement();
            }
        }
        public void ReadXml(XmlReader reader)       // Deserializer
        {
            reader.Read();
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("SerializableDictionary");
                reader.ReadStartElement("key");
                TKey tk = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue vl = (TValue)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                this.Add(tk, vl);
                reader.MoveToContent();
            }
            reader.ReadEndElement();

        }
        public XmlSchema GetSchema()
        {
            return null;
        }
    }
}
