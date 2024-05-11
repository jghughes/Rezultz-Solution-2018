using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using Rezultz.DataTransferObjects.Nov2023.Results;

// ReSharper disable ValueParameterNotUsed

namespace Rezultz.DataTypes.Nov2023.PortalSplitIntervalItems
{

    // No serialisation attributes are required on this Item. It never gets exported/printed or sent over the wire or serialised.
    // It is a working scratchpad for storing the computed split intervals for each person. Each person will have one or more intervals
    // depending on the format of the race.  The contents of all the split intervals for a person are consolidated into 
    // a single ResultItemDataTransferObject for that person and  pushed to the location on the server for pre-processed results.

    public class SplitIntervalItem : IHasTimeStampBinaryFormat, IHasGuid, IHasIdentifier
    {
		#region constants

		public const string Spacer = "   "; // size governed by width of screen limitation and size of font

		#region constants

		// be sure that this item always matches the equivalent in ParticipantHubItemEditTemplate otherwise the Results for
		// participants in an unknown race division will never print out in the HTML versions of the results

		#endregion

		#endregion

		#region properties

		public int Place { get; set; }

        public string Bib { get; set; } = string.Empty;

        public string Rfid { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

		public string MiddleInitial { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Gender { get; set; } = string.Empty;

		public int BirthYear { get; set; }

		public string AgeGroup { get; set; } = string.Empty;

		public string City { get; set; } = string.Empty;

		public string Team { get; set; } = string.Empty;

		public string RaceGroup { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

		public bool IsSeries { get; set; }

		public string DnxSymbol { get; set; } = string.Empty;

		public string Comment { get; set; } = string.Empty;

		public long DurationSincePrecedingTimeStampTicks { get; set; }

		public long CumulativeTotalDurationTicks { get; set; }

		public long TimeStampBinaryFormat { get; set; } // unfortunately can't change name to EndingTimeStampBinaryLocal because this TimeStampBinaryFormat prop implements required interface

		public string Guid { get; set; } // not used for anything. merely to satisfy the required interface

		#endregion

		#region methods


		public static ResultDto ToResultItemDataTransferObject(SplitIntervalItem splitIntervalItem)
		{
			if (splitIntervalItem == null) return new ResultDto();
			// nb. don't return null. logic downstream requires to know that IsAuthorisedToOperate == false

			string ToSplitIntervalDuration(SplitIntervalItem interval)
			{
				var answer = interval.DurationSincePrecedingTimeStampTicks == 0
					? string.Empty
					: TimeSpan.FromTicks(interval.DurationSincePrecedingTimeStampTicks)
						.ToString(JghTimeSpan.GeneralLongPattern);

				return answer;
			}

			var answer = new ResultDto()
			{
				Bib = JghString.TmLr(splitIntervalItem.Bib),
                Rfid = JghString.TmLr(splitIntervalItem.Rfid),
				First = JghString.TmLr(splitIntervalItem.FirstName),
				Last = JghString.TmLr(splitIntervalItem.LastName),
				MiddleInitial = JghString.TmLr(splitIntervalItem.MiddleInitial),
				Sex = JghString.TmLr(splitIntervalItem.Gender),
				Age = JghConvert.ToAgeFromYearOfBirth(splitIntervalItem.BirthYear),
				AgeGroup = JghString.TmLr(splitIntervalItem.AgeGroup),
				City = JghString.TmLr(splitIntervalItem.City),
				Team = JghString.TmLr(splitIntervalItem.Team),
				RaceGroup = JghString.TmLr(splitIntervalItem.RaceGroup),
				IsSeries = splitIntervalItem.IsSeries,
				DnxString = splitIntervalItem.DnxSymbol,
			};

			answer.T01 = string.IsNullOrWhiteSpace(answer.DnxString) ? ToSplitIntervalDuration(splitIntervalItem) : string.Empty; // important

			return answer;
		}

		public static ResultDto[] ToResultItemDataTransferObject(SplitIntervalItem[] splitInterval)
		{
			const string failure = "Populating ResultItemDataTransferObject.";
			const string locus = "[ToResultItemDataTransferObject]";

			try
			{
				if (splitInterval == null)
					return Array.Empty<ResultDto>();

				var answer = splitInterval.Select(ToResultItemDataTransferObject).Where(z => z != null).ToArray();

				return answer;
			}

			#region trycatch

			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
			}

			#endregion
		}


		#endregion
	}
}
