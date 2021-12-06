namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPlayDto[]), new XmlRootAttribute("Plays"));

            using StringReader stringReader = new StringReader(xmlString);

            ImportPlayDto[] playDtos = (ImportPlayDto[])xmlSerializer.Deserialize(stringReader);

            List<Play> plays = new List<Play>();

            foreach (ImportPlayDto playDto in playDtos)
            {
                if (!IsValid(playDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                TimeSpan duration = TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture);

                if (!IsValid(duration) || duration.TotalHours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isGenreTypeValid = Enum.TryParse(typeof(Genre), playDto.Genre, out object genreTypes);

                if (!isGenreTypeValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Genre genreType = (Genre)genreTypes;

                Play p = new Play()
                {
                    Title = playDto.Title,
                    Duration = duration,
                    Rating = playDto.Rating,
                    Genre = genreType,
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter
                };

                plays.Add(p);
                sb.AppendLine(String.Format(SuccessfulImportPlay, p.Title, p.Genre, p.Rating));
            }

            context.Plays.AddRange(plays);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCastDto[]), new XmlRootAttribute("Casts"));

            using StringReader stringReader = new StringReader(xmlString);

            ImportCastDto[] castDtos = (ImportCastDto[])xmlSerializer.Deserialize(stringReader);

            List<Cast> casts = new List<Cast>();

            foreach (ImportCastDto castDto in castDtos)
            {
                if (!IsValid(castDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Cast c = new Cast()
                {
                    FullName = castDto.FullName,
                    IsMainCharacter = castDto.IsMainCharacter,
                    PhoneNumber = castDto.PhoneNumber,
                    PlayId = castDto.PlayId
                };

                casts.Add(c);
                sb.AppendLine(string.Format(SuccessfulImportActor, c.FullName,
                     c.IsMainCharacter ? "main" : "lesser"));
            }

            context.Casts.AddRange(casts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportTheatreDto[] theatreDtos = JsonConvert.DeserializeObject<ImportTheatreDto[]>(jsonString);

            List<Theatre> theatres = new List<Theatre>();
            List<Ticket> tickets = new List<Ticket>();

            foreach (ImportTheatreDto theatreDto in theatreDtos)
            {
                if (!IsValid(theatreDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre t = new Theatre()
                {
                    Name = theatreDto.Name,
                    NumberOfHalls = (sbyte)theatreDto.NumberOfHalls,
                    Director = theatreDto.Director
                };

                foreach (var ticketDto in theatreDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ticket = new Ticket()
                    {
                        Price = ticketDto.Price,
                        RowNumber = ticketDto.RowNumber,
                        PlayId = ticketDto.PlayId
                    };

                    tickets.Add(ticket);
                    t.Tickets.Add(ticket);
                }

                theatres.Add(t);
                sb.AppendLine(String.Format(SuccessfulImportTheatre, t.Name, t.Tickets.Count));
            }

            context.Theatres.AddRange(theatres);
            context.Tickets.AddRange(tickets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
