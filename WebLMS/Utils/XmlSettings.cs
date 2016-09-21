using System;
using System.Xml.Serialization;
using System.IO;

namespace VClass
{
    class XmlSettings
    {
        public static XmlModel LoadXml(string path){
            /*XDocument xdoc = XDocument.Load(path);

            List<Frame> frames = xdoc.Descendants("frame").Where(f => f.Attribute("type").Value == "image").Select(f => new Frame
            {
                Height = Convert.ToDouble(f.Attribute("height").Value.Replace('.', ',')),
                Width = Convert.ToDouble(f.Attribute("width").Value.Replace('.', ',')),
                VisibleHeight = Convert.ToDouble(f.Attribute("visibleHeight").Value.Replace('.', ',')),
                VisibleWidth = Convert.ToDouble(f.Attribute("visibleWidth").Value.Replace('.', ',')),
                Time = Convert.ToInt32(f.Attribute("time").Value),
                Type = f.Attribute("type").Value,
                Zindex = Convert.ToInt32(f.Attribute("zindex").Value),
                Visible = Convert.ToBoolean(f.Attribute("visible").Value),
                Y = Convert.ToDouble(f.Attribute("y").Value.Replace('.', ',')),
                X = Convert.ToDouble(f.Attribute("x").Value.Replace('.', ',')),
                Name = f.Attribute("name").Value,
                Id = f.Attribute("id").Value,
                Parent = f.Attribute("id").Value
            }).ToList<Frame>();
            XmlModel model = new XmlModel
            {
                Name = xdoc.Root.Element("name").Value,
                TimeStamp = Convert.ToUInt32(xdoc.Root.Attribute("timestamp").Value),
                Version = xdoc.Root.Attribute("version").Value,
                Height = Convert.ToInt32(xdoc.Root.Attribute("height").Value),
                Width = Convert.ToInt32(xdoc.Root.Attribute("width").Value),
                Frames = frames
            };
            return model;*/

            XmlSerializer reader = new XmlSerializer(typeof(XmlModel));
            StreamReader file = new StreamReader(path);
            XmlModel overview = (XmlModel)reader.Deserialize(file);
            file.Close();
            return overview;
        }
    }
}
