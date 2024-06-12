using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;

// ReSharper disable InconsistentNaming

namespace Tool12
{
    [Serializable]
    public class ParticipantWithSeriesPointsTally
    {
        #region properties

        public int Position { get; set; }

        public string FullName { get; set; } = string.Empty;

        public bool IsSeries { get; set; }

        public string Bib { get; set; } = string.Empty;

        public string Rfid { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public int Age { get; set; }

        public string Sex { get; set; } = string.Empty;

        public string RaceGroup { get; set; } = string.Empty;

        public int SeriesPointTopNine { get; set; }

        public int SeriesPointsOverall { get; set; }

        public int T01 { get; set; }

        public int T02 { get; set; }

        public int T03 { get; set; }

        public int T04 { get; set; }

        public int T05 { get; set; }

        public int T06 { get; set; }

        public int T07 { get; set; }

        public int T08 { get; set; }

        public int T09 { get; set; }

        public int T10 { get; set; }

        public int T11 { get; set; }

        public int T12 { get; set; }

        public bool IsDefective => string.IsNullOrWhiteSpace(Bib) || string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(RaceGroup) || (RaceGroup != "expert" && RaceGroup != "intermediate" && RaceGroup != "novice" && RaceGroup != "sport");

        public string Comment { get; set; } = string.Empty;

        public string Guid { get; } = System.Guid.NewGuid().ToString();

        #endregion

        #region methods

        public static ParticipantWithSeriesPointsTally FromDataTransferObject(ParticipantWithSeriesPointsTallyDto? dto)
        {
            const string failure = "Populating InfantParticipant.";
            const string locus = "[FromDataTransferObject]";


            try
            {
                var x = dto ?? new ParticipantWithSeriesPointsTallyDto();

                var answer = new ParticipantWithSeriesPointsTally()
                {
                    Position = JghConvert.ToInt32(x.Position),
                    FullName = JghString.TmLr(x.FullName),
                    IsSeries = true,
                    Bib = JghString.TmLr(x.Bib),
                    Rfid = JghString.TmLr(x.Rfid),
                    Age = JghConvert.ToInt32(x.Age),
                    Sex = JghString.TmLr(x.Sex),
                    RaceGroup = JghString.TmLr(x.RaceGroup),
                    SeriesPointTopNine = JghConvert.ToInt32(x.PointsTotalTopNine),
                    SeriesPointsOverall = JghConvert.ToInt32(x.PointsTotalOverall),
                    T01 = JghConvert.ToInt32(x.T01),
                    T02 = JghConvert.ToInt32(x.T02),
                    T03 = JghConvert.ToInt32(x.T03),
                    T04 = JghConvert.ToInt32(x.T04),
                    T05 = JghConvert.ToInt32(x.T05),
                    T06 = JghConvert.ToInt32(x.T06),
                    T07 = JghConvert.ToInt32(x.T07),
                    T08 = JghConvert.ToInt32(x.T08),
                    T09 = JghConvert.ToInt32(x.T09),
                    T10 = JghConvert.ToInt32(x.T10),
                    T11 = JghConvert.ToInt32(x.T11),
                    T12 = JghConvert.ToInt32(x.T12),
                    Comment = JghString.TmLr(x.Comment),
                };

                if (DateTime.TryParse(x.DateOfBirthAsString, out var dateOfBirth))
                {
                    answer.DateOfBirth = DateOnly.FromDateTime(dateOfBirth);
                }

                answer.IsSeries = JghString.JghContains("Series-Full Series", x.Product, StringComparison.OrdinalIgnoreCase) || JghString.JghContains("Kids Series", x.Product, StringComparison.OrdinalIgnoreCase);

                if(answer.IsDefective)
                {
                    var sb = new JghStringBuilder();

                    if (string.IsNullOrWhiteSpace(answer.Bib))
                    {
                        sb.Append("Bib is blank.");
                    }

                    if (string.IsNullOrWhiteSpace(answer.FullName))
                    {
                        sb.AppendAsNewSentence("Full name is blank.");
                    }

                    if (string.IsNullOrWhiteSpace(answer.Sex))
                    {
                        sb.AppendAsNewSentence("Gender is blank.");
                    }

                    if (string.IsNullOrWhiteSpace(answer.RaceGroup))
                    {
                        sb.AppendAsNewSentence("Category is blank.");
                    }

                    if (answer.RaceGroup != "expert" && answer.RaceGroup != "intermediate" && answer.RaceGroup != "novice" && answer.RaceGroup != "sport" )
                    {
                        sb.AppendAsNewSentence("Category is not valid.");
                    }

                    answer.Comment = sb.ToString();

                }
                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }

        public static ParticipantWithSeriesPointsTally[] FromDataTransferObject(ParticipantWithSeriesPointsTallyDto[]? dataTransferObject)
        {
            const string failure = "Populating SeriesParticipantWithPointsTally.";
            const string locus = "[FromDataTransferObject]";


            try
            {
                if (dataTransferObject == null)
                    return [];

                var answer = dataTransferObject.Select(FromDataTransferObject).ToArray();

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }

        public static ParticipantWithSeriesPointsTallyDto ToDataTransferObject(ParticipantWithSeriesPointsTally? item)
        {
            const string failure = "Populating SeriesParticipantWithPointsTallyDto.";
            const string locus = "[ToDataTransferObject]";

            try
            {

                var x = item ?? new ParticipantWithSeriesPointsTally();

                var answer = new ParticipantWithSeriesPointsTallyDto()
                {
                    Position = x.Position.ToString(),
                    FullName = JghString.TmLr(x.FullName),
                    Bib = JghString.TmLr(x.Bib),
                    Rfid = JghString.TmLr(x.Rfid),
                    Age = x.Age.ToString(),
                    Sex = JghString.TmLr(x.Sex),
                    RaceGroup = JghString.TmLr(x.RaceGroup),
                    PointsTotalTopNine = x.SeriesPointTopNine.ToString(),
                    PointsTotalOverall = x.SeriesPointsOverall.ToString(),
                    T01 = x.T01.ToString(),
                    T02 = x.T02.ToString(),
                    T03 = x.T03.ToString(),
                    T04 = x.T04.ToString(),
                    T05 = x.T05.ToString(),
                    T06 = x.T06.ToString(),
                    T07 = x.T07.ToString(),
                    T08 = x.T08.ToString(),
                    T09 = x.T09.ToString(),
                    T10 = x.T10.ToString(),
                    T11 = x.T11.ToString(),
                    T12 = x.T12.ToString(),
                    Comment = JghString.TmLr(x.Comment)
                };

                answer.Product = x.IsSeries ? "Series-Full Series" : "One-off";
                answer.DateOfBirthAsString = x.DateOfBirth.ToString(JghDateTime.Iso8601Pattern);

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static ParticipantWithSeriesPointsTallyDto[] ToDataTransferObject(ParticipantWithSeriesPointsTally[]? item)
        {

            const string failure = "Populating SeriesParticipantWithPointsTallyDto[].";
            const string locus = "[ToDataTransferObject]";

            try
            {
                if (item == null)
                    return [];

                var answer = item.Select(ToDataTransferObject).ToArray();

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }

        public override string ToString()
        {
            return JghString.ConcatAsSentences(Bib, FullName, Age.ToString(), RaceGroup);
        }


        #endregion

    }
}