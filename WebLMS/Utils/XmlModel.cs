using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VClass
{
    [XmlRoot(ElementName = "conference")]
    public class XmlModel
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "timestamp")]
        public uint TimeStamp { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlArray("frames")]
        [XmlArrayItem("frame", Type = typeof(Frame))]
        public List<Frame> Frames { get; set; }
    }


    public class Frame
    {
        [XmlAttribute(AttributeName = "height")]
        public double Height { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public double Width { get; set; }

        [XmlAttribute(AttributeName = "visibleHeight")]
        public double VisibleHeight { get; set; }

        [XmlAttribute(AttributeName = "visibleWidth")]
        public double VisibleWidth { get; set; }

        [XmlAttribute(AttributeName = "time")]
        public int Time { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "zindex")]
        public int Zindex { get; set; }

        [XmlAttribute(AttributeName = "visible")]
        public bool Visible { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public double Y { get; set; }

        [XmlAttribute(AttributeName = "x")]
        public double X { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }

        [XmlAttribute(AttributeName = "stype")]
        public string Stype { get; set; }

        [XmlAttribute(AttributeName = "action")]
        public string Action { get; set; }
    }
}
