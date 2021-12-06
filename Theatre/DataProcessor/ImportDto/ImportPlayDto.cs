using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPlayDto
    {
        [Required]
        [StringLength(50,MinimumLength =4)]
        [XmlElement("Title")]
        public string Title { get; set; }

        [Required]
        [XmlElement("Duration")]
        public string Duration { get; set; }

        [Range(typeof(float),"0.00","10.00")]
        public float Rating { get; set; }
        
        [EnumDataType(typeof(Genre))]
        [XmlElement("Genre")]
        public string Genre { get; set; }

        [Required]
        [StringLength(700)]
        [XmlElement("Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4)]
        [XmlElement("Screenwriter")]
        public string Screenwriter { get; set; }
    }
}