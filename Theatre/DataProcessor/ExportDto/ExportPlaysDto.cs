using System;
using System.Xml.Serialization;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ExportDto
{
    [XmlType("Play")]
    public class ExportPlaysDto
    {
        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("Duration")]
        public string Duration { get; set; }

        [XmlAttribute("Rating")]
        public string Rating { get; set; }

        [XmlAttribute("Genre")]
        public string Genre { get; set; }

        [XmlArray("Actors")]
        public ExportPlaysActorsDto[] Actors { get; set; }
    }
}
