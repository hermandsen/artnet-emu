using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArtnetEmu.Model
{
    [XmlRoot(ElementName = "info")]
    public class Info
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "category")]
    public class Category
    {
        [XmlElement(ElementName = "info")]
        public List<Info> Info { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "information")]
    public class Information
    {
        [XmlElement(ElementName = "category")]
        public List<Category> Category { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class VLCStatusModel
    {
        [XmlElement(ElementName = "fullscreen")]
        public bool Fullscreen { get; set; }
        [XmlElement(ElementName = "audiodelay")]
        public float Audiodelay { get; set; }
        [XmlElement(ElementName = "apiversion")]
        public int Apiversion { get; set; }
        [XmlElement(ElementName = "currentplid")]
        public int Currentplid { get; set; }
        [XmlElement(ElementName = "time")]
        public int Time { get; set; }
        [XmlElement(ElementName = "volume")]
        public int Volume { get; set; }
        [XmlElement(ElementName = "length")]
        public int Length { get; set; }
        [XmlElement(ElementName = "random")]
        public bool Random { get; set; }
        [XmlElement(ElementName = "rate")]
        public float Rate { get; set; }
        [XmlElement(ElementName = "state")]
        public string State { get; set; }
        [XmlElement(ElementName = "loop")]
        public bool Loop { get; set; }
        [XmlElement(ElementName = "version")]
        public string Version { get; set; }
        [XmlElement(ElementName = "position")]
        public float Position { get; set; }
        [XmlElement(ElementName = "repeat")]
        public bool Repeat { get; set; }
        [XmlElement(ElementName = "subtitledelay")]
        public float SubtitleDelay { get; set; }
    }

}
