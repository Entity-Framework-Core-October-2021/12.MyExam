namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatres = context.Theatres
                                        .ToArray()
                                        .Where(h => h.NumberOfHalls >= numbersOfHalls && h.Tickets.Count >= 20)
                                        .Select(t => new
                                        {
                                            Name = t.Name,
                                            Halls = t.NumberOfHalls,
                                            TotalIncome = t.Tickets
                                                                .Where(x => x.RowNumber >= 1 && x.RowNumber <= 5)
                                                                .Sum(p => p.Price),
                                            Tickets = t.Tickets
                                                         .Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5)
                                                         .Select(ti => new
                                                         {
                                                             Price = ti.Price,
                                                             RowNumber = ti.RowNumber
                                                         })
                                                         .OrderByDescending(ti => ti.Price)
                                        })
                                        .OrderByDescending(t => t.Halls)
                                        .ThenBy(t => t.Name);

            return JsonConvert.SerializeObject(theatres, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportPlaysDto[]), new XmlRootAttribute("Plays"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter sw = new StringWriter(sb);

            var playDtos = context.Plays
                                    .Where(p => p.Rating <= rating)
                                    .OrderBy(o=>o.Title)
                                    .ThenByDescending(o => o.Genre)
                                    .Select(p => new ExportPlaysDto
                                    {
                                        Title = p.Title,
                                        Duration = p.Duration.ToString("c"),
                                        Rating = p.Rating == 0? "Premier" : p.Rating.ToString(),
                                        Genre = p.Genre.ToString(),
                                        Actors = p.Casts
                                                     .Where(c => c.IsMainCharacter)
                                                     .Select(c => new ExportPlaysActorsDto
                                                     {
                                                         FullName = c.FullName,
                                                         MainCharacter = $"Plays main character in '{c.Play.Title}'."
                                                     })
                                                     .OrderByDescending(a => a.FullName)
                                                     .ToArray()
                                    })
                                    .ToArray();

            xmlSerializer.Serialize(sw, playDtos, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
