using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ArtnetEmu.Model
{
    [XmlRoot(ElementName = "element")]
    public class BrowseElement
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "access_time")]
        public int AccessTime { get; set; }
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }
        [XmlAttribute(AttributeName = "creation_time")]
        public int CreationTime { get; set; }
        [XmlAttribute(AttributeName = "gid")]
        public string Gid { get; set; }
        [XmlAttribute(AttributeName = "modification_time")]
        public int ModificationTime { get; set; }
        [XmlAttribute(AttributeName = "mode")]
        public int Mode { get; set; }
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }
        [XmlAttribute(AttributeName = "size")]
        public int Size { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class VLCBrowseModel
    {
        [XmlElement(ElementName = "element")]
        public List<BrowseElement> Elements { get; set; }
    }
}
