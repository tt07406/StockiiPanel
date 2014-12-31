using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Windows.Forms;
using System.Drawing;

//自定义类
namespace StockiiPanel
{
    /// <summary>
    /// 用于保存在本地的Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
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

    /// 说明: 使用此Button时要设置ContextMenuStrip属性值  
    ///       单击些Button的Click事件要传入所在工做区的宽高  
    ///       如果没有所需的属性值,则如平时所使用的Button一至  
    /// 使用例子:  
    ///       DropButton.WorkSizeX = this.MdiParent.ClientRectangle.Width;  
    ///       DropButton.WorkSizeY = this.MdiParent.ClientRectangle.Height;  
    /// 应用:  
    /// 创建人: lrj  
    /// 创建日期:2008-05-22  
    /// 修改人:  
    /// 修改日期:  
    /// 

    public partial class DropButton : Button
    {
        private ContextMenuStrip contextMenuStrip;
        private Point point;     //立标  
        private int x = 0;     //立标x  
        private int y = 0;     //立标y  
        private int workSize_x;//工做区x    
        private int workSize_y;//工做区y  

        public DropButton()
        {
            x = this.Size.Width;
            y = 0;
        }
        /// 

        /// 工做区的完  
        /// 

        public int WorkSizeX
        {
            get { return workSize_x; }
            set { workSize_x = value; }
        }
        /// 

        /// 工做区的高  
        /// 

        public int WorkSizeY
        {
            get { return workSize_y; }
            set { workSize_y = value - 55; }
        }
        ///


        /// ContextMenuStrip菜单  
        /// 

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return contextMenuStrip; }
            set
            {
                if (contextMenuStrip == null)
                {
                    contextMenuStrip = value;
                }
            }
        }
        //  
        //重写的单击事件  
        //  
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            //菜单在工做区离边框的宽高  
            int _x = this.Parent.Location.X + this.Location.X + contextMenuStrip.Size.Width;
            int _y = this.Parent.Location.Y + this.Location.Y + this.Size.Height + contextMenuStrip.Size.Height;
            if
            (_x < WorkSizeX - 8)
            {
                x = this.Size.Width;
            }
            else
            {
                x = 0 - contextMenuStrip.Size.Width + this.Size.Width;
            }
            if
            (_y < WorkSizeY)
            {
                y = 0;
            }
            else
            {
                y = 0 - contextMenuStrip.Size.Height;
            }
            point = new Point(x, y);
            contextMenuStrip.Show(this, point);
        }
        //  
        //使鼠标右键失效  
        //  
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button.ToString() != "Right")
            {
            }
        }

    } 
}