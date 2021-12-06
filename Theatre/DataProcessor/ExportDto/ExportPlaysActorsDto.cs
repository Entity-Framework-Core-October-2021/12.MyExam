using System.Xml.Serialization;

namespace Theatre.DataProcessor.ExportDto
{
    [XmlType("Actor")]
    public class ExportPlaysActorsDto
    {
        [XmlAttribute("FullName")]
        public string FullName { get; set; }

        [XmlAttribute("MainCharacter")]
        public string MainCharacter { get; set; }
    }
}
