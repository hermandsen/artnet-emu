using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace ArtnetEmu.Model
{
    [XmlRoot(ElementName = "leaf")]
    public class Leaf
    {
        [XmlAttribute(AttributeName = "ro")]
        public string Ro { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "duration")]
        public int Duration { get; set; }
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }
        [XmlAttribute(AttributeName = "current")]
        public string Current { get; set; }
    }

    [XmlRoot(ElementName = "node")]
    public class Node
    {
        [XmlElement(ElementName = "leaf")]
        public List<Leaf> Files { get; set; }
        [XmlAttribute(AttributeName = "ro")]
        public string Ro { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "node")]
    public class VLCPlaylistModel
    {
        [XmlElement(ElementName = "node")]
        public List<Node> Lists { get; set; }
        [XmlAttribute(AttributeName = "ro")]
        public string Ro { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }
}
