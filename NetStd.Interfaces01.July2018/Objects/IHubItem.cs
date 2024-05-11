namespace NetStd.Interfaces01.July2018.Objects
{
    public interface IHubItem
	{
		string RecordingModeEnum { get; set; }

        string Bib { get; set; }

        string Rfid { get; set; }

        long TimeStampBinaryFormat { get; set; }

		bool MustDitchOriginatingItem { get; set; }

		bool IsStillToBeBackedUp { get; set; }

		bool IsStillToBePushed { get; set; }

		long WhenPushedBinaryFormat { get; set; }

		long WhenTouchedBinaryFormat { get; set; }

		string OriginatingItemGuid { get; set; }

        string GetBothGuids();

    }
}