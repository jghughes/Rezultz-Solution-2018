Race particulars are loaded from profiles and settings .xml file into:

_parentEvent.ArrayOfRaceSpecificationItem and
_parentSeries.ArrayOfRaceSpecificationItem

Unfortunately there are unavoidable hard-coded 
comparators in a myriad of methods. The comparators 
must exact match the CodeName properties here. Case sensitive.
The Label and Title properties are also used variously
for displaying text, and Label is also used for comparisons. Label and CodeName must be identical. The Title
property as the race header in the HTML printouts and the Label
property in 
NetStd.Rezultz02.July2018.MultiFactoralFilterViewModel.CboListOfRacesPresenter.Label

If a comparison fails,the results for the race matching that CodeName 
will not be printed in the HTML versions of Results.

InformationPrinter.GetLeaderboardStyleResultsAsPrettyPrintedWebPageAsync()
InformationPrinter.GenerateLeaderboardStyleResultsAsPreformattedTextAsync()
InformationPrinter.GenerateLeaderboardStyleResultsAsWebPageAsync()

AlgorithmsForAverageLapTimes.PopulateAverageLapTimesForThisEventAsync()
AlgorithmsForPoints.CalculateMeasureOfKelsoSeniority2021KelsoMtb()

ParticipantHubItemEditTemplate.MergeEditsWithItem()
ParticipantHubItemEditTemplate.IsInvalidParticulars()

The CodeName property value must flow faultlessly
through :

NetStd.Rezultz.Types.July2018.ParticipantHubItemEditTemplate.RaceDivisionIdentifier
to
NetStd.Rezultz.Types.July2018.ParticipantHubItem.RaceDivision
to
NetStd.Rezultz.Types.July2018.SplitIntervalItem.RaceDivision
to
NetStd.RezultzDataAdapters.July2018.TimingTentDataConverterModuleForKelso2021Mtb.Result.Race
to
NetStd.Rezultz.Types.July2018.RezultzTypeXmlParsers.ParseToRaceSpecificationItemEntity()
to
NetStd.Rezultz.Types.July2018.ParticularsItemXmlParsers.ParseToParticularsItemEntity()
to
NetStd.Rezultz.Types.July2018.Result.Race


To see, if/what has reached a final destination, go to:

NetStd.Rezultz01.July2018.InformationPrinter.GenerateLeaderboardStyleResultsAsPreformattedTextAsync()

the block begining with: if (resultsMustBeGroupedByRace)


		