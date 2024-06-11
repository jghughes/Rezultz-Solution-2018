using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

[DataContract(Namespace = "", Name = XeRaceSpecification)]
public class RaceSpecificationDto
{
    #region Names

    private const string XeRaceSpecification = "race-specification";
    private const string XeSeniorityRankForPointsTransferUponUpgrade = "seniority-rank-for-points-transfer";
    private const string XeTrophyPointsForTheWinner = "trophy-points";
    private const string XePointsScaleForPLacesAsCsv = "points-scale-as-csv";

    private const string XeMultiplicationFactorForNormalizationOfDifferentCourseLengths = "multiplication-factor-for-normalisation-of-average-split-duration";

    public const string XeDistanceOfCourseKm = "distance-km";
    private const string XeLabelOfItem = "label";
    private const string XeDisplayRankOfItem = "display-rank";

    // Note: the following are not used in this class, but are used in the publisher modules
    public const string XeDiscipline = "discipline";
    public const string XeAdvertisedStartDateTime = "advertised-start-date-time";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeSeniorityRankForPointsTransferUponUpgrade)]
    public int SeniorityRankForPointsTransfer { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeTrophyPointsForTheWinner)]
    public double TrophyPoints { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XePointsScaleForPLacesAsCsv)]
    public string PointsScaleAsCsv { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeMultiplicationFactorForNormalizationOfDifferentCourseLengths)]
    public double MultiplicationFactorForNormalizationOfAverageSplitDuration { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeDistanceOfCourseKm)]
    public double DistanceKm { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6, Name = XeLabelOfItem)]
    public string Label { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7, Name = XeDisplayRankOfItem)]
    public int DisplayRank { get; set; } // used only for enum items or items that don't otherwise have a self-evident ordering of display in a collection

    #endregion
}