using System;
using System.Collections.Generic;
using System.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.PortalSplitIntervalItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repository_interfaces;

namespace Rezultz.Library01.Mar2024.Repositories
{
    public class RepositoryOfSplitDurationsPerParticipant
    {
        #region load database - HEAP POWERFUL

        public bool LoadRepository(EventProfileItem thisEventProfile, IRepositoryOfHubStyleEntries<TimeStampHubItem> repositoryOfTimeStampHubItems, IRepositoryOfHubStyleEntries<ParticipantHubItem> repositoryOfParticipantHubItems)
        {
            if (thisEventProfile is null)
                return true;

            if (repositoryOfTimeStampHubItems is null)
                return true;

            #region load database of all participantProfiles

            _thisEventProfile = thisEventProfile;


            //if (repositoryOfParticipantHubItems is null)
            //    _participantDatabase.LoadDatabaseV2(Array.Empty<ParticipantHubItem>(), dateOfEvent);
            //else
            _participantDatabase.LoadDatabaseV2(repositoryOfParticipantHubItems);

            // N.B. quite normal and commonplace for repositoryOfParticipantHubItems to be null or empty
            // simply because the timekeeper hasn't initialised the Participant Admin role and hasn't downloaded all the participants
            // this means _participantDatabase will have no profiles in it 

            #endregion

            #region get all the GunStart timestamps by type (mass and group). and contestant (individual)

            _gunStartForEverybodyTimestamp = repositoryOfTimeStampHubItems.GetSingleMostRecentItemOfThisKindOfRecordingModeFromMasterList(
                EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody); // will be null if not found in dataset

            _dictionaryOfRacesThatHadAGunStart = repositoryOfTimeStampHubItems.GetDictionaryOfIdentifiersWithTheirMostRecentItemForThisRecordingModeFromMasterList(
                EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup); // will be empty if none found

            _dictionaryOfContestantsWhoHadAnIndividualGunStart = repositoryOfTimeStampHubItems.GetDictionaryOfIdentifiersWithTheirMostRecentItemForThisRecordingModeFromMasterList(
                EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual); // will be empty if none found

            #endregion

            #region get all the timing mat timestamps for everyone

            _listDictionaryOfContestantsWhoActivatedATimingMat = repositoryOfTimeStampHubItems.GetDictionaryOfIdentifiersWithTheirMultipleItemsForThisRecordingModeFromMasterList(
                EnumStrings.KindOfEntryIsTimeStampForTimingMatSignal);

            #endregion

            #region make a list of the IDs of all contestants who competed - people who either started or triggered a timing-mat 

            var listOfAllContestantIdentifiers = new List<string>();

            foreach (var timeStampHubItemKvp in _dictionaryOfContestantsWhoHadAnIndividualGunStart)
                listOfAllContestantIdentifiers.Add(timeStampHubItemKvp.Key);

            foreach (var timeStampHubItemKvp in _listDictionaryOfContestantsWhoActivatedATimingMat)
                listOfAllContestantIdentifiers.Add(timeStampHubItemKvp.Key);

            #endregion

            #region for everybody who competed, create a ConsolidatedSplitIntervalsItem, and populate the RaceGroup for each constestant 

            _dictionaryOfEveryBodyWhoCompeted = new Dictionary<string, SplitDurationConsolidationForParticipantItem>();

            foreach (var identifier in listOfAllContestantIdentifiers.Where(z => !string.IsNullOrWhiteSpace(z)).Distinct())
            {

                // new up

                var consolidatedSplitIntervalsItem = new SplitDurationConsolidationForParticipantItem
                {
                    Bib = identifier,
                    Participant = _participantDatabase.GetParticipantFromMasterList(identifier) // totally normal for this to be null
                };

                consolidatedSplitIntervalsItem.Rfid = consolidatedSplitIntervalsItem.Participant?.Rfid; // Note: todo. it would be more logical for this to come from a timestamp, not the participant master list.

                // figure out the governing RaceGroup of the individual when the event took place (could conceivably be an upgraded/downgraded RaceGroup if the individual changes categories)

                if (consolidatedSplitIntervalsItem.Participant is not null)
                {
                    var dateOfEvent = _thisEventProfile.AdvertisedDate.Date;

                    if (consolidatedSplitIntervalsItem.Participant.RaceGroupBeforeTransition == consolidatedSplitIntervalsItem.Participant.RaceGroupAfterTransition)
                    {
                        // simple case. both the same
                        consolidatedSplitIntervalsItem.RaceGroupDeducedFromParticipant = consolidatedSplitIntervalsItem.Participant.RaceGroupBeforeTransition; // default
                    }
                    else
                    {
                        // deduce the governing group

                        var transitionDate = consolidatedSplitIntervalsItem.Participant.DateOfRaceGroupTransition.Date;

                        consolidatedSplitIntervalsItem.RaceGroupDeducedFromParticipant = dateOfEvent < transitionDate ? consolidatedSplitIntervalsItem.Participant.RaceGroupBeforeTransition : consolidatedSplitIntervalsItem.Participant.RaceGroupAfterTransition;
                    }
                }

                _dictionaryOfEveryBodyWhoCompeted.Add(identifier, consolidatedSplitIntervalsItem);
            }

            #endregion

            #region for everybody who competed, populate their ConsolidatedSplitIntervalsItem the list of all their timing-mat timestamps and their gun start (if known)

            foreach (var personKvp in _dictionaryOfEveryBodyWhoCompeted)
            {
                #region populate the list of timing-mat timestamps for the person

                var collectionOfTimeStampsForThePerson = JghDictionaryHelpers.LookUpValueSafely(personKvp.Key, _listDictionaryOfContestantsWhoActivatedATimingMat);

                if (collectionOfTimeStampsForThePerson is not null) personKvp.Value.ListOfTimingMatTimeStamps = collectionOfTimeStampsForThePerson.ToList();

                #endregion

                #region populate the gun start for the person if it can be deduced

                #region step 1: iff the person has an individual gun start, use it and look no further

                personKvp.Value.GunStartTimeStamp = JghDictionaryHelpers.LookUpValueSafely(personKvp.Key, _dictionaryOfContestantsWhoHadAnIndividualGunStart);

                if (personKvp.Value.GunStartTimeStamp is not null)
                {
                    // success
                    personKvp.Value.KindOfGunStart = EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual; // for display in the datagrid only
                    continue; 
                }

                #endregion

                #region step 2: fallback, if the person is recognised and a member of a RaceGroup for which there was a GroupStart, use that and look no further

                if (personKvp.Value.Participant is null)
                {
                    personKvp.Value.GunStartTimeStamp = null;
                    personKvp.Value.KindOfGunStart = string.Empty; // for display in the datagrid only
                }
                else
                {
                    bool eventIsBeforeTransitionToADifferentRaceGroup = _thisEventProfile.AdvertisedDate.Date < personKvp.Value.Participant.DateOfRaceGroupTransition.Date;
             
                    string governingRaceGroup = eventIsBeforeTransitionToADifferentRaceGroup ? personKvp.Value.Participant.RaceGroupBeforeTransition : personKvp.Value.Participant.RaceGroupAfterTransition;

                    personKvp.Value.GunStartTimeStamp = JghDictionaryHelpers.LookUpValueSafely(governingRaceGroup, _dictionaryOfRacesThatHadAGunStart);

                    if (personKvp.Value.GunStartTimeStamp is null)
                    {
                        // failure
                        personKvp.Value.KindOfGunStart = string.Empty; // for display in the datagrid only
                    }
                    else 
                    {
                        // success

                        personKvp.Value.KindOfGunStart = JghString.ConcatAsSentences(EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup, governingRaceGroup); // for display in the datagrid only

                        continue; 
                    }
                }

                #endregion

                #region step 3: penultimate fallback. If we reach here, the gunstart for this person did not have an individual or a group gun start. If there is a mass start, use that for him

                if (_gunStartForEverybodyTimestamp is not null)
                {
                    personKvp.Value.GunStartTimeStamp = _gunStartForEverybodyTimestamp;

                    personKvp.Value.KindOfGunStart = EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody;

                }
                #endregion

                #region step 4: final fallback. There was no starter wielding a gun in any way, shape or form. The person started automatically when they crossed a timing mat at the start line.

                // Do nothing for now. We deal with the scenario for this person later

                #endregion


                #endregion
            }

            #endregion

            #region populate consolidated list of all timestamps

            foreach (var contestantScratchPadKvp in _dictionaryOfEveryBodyWhoCompeted)
            {
                if (contestantScratchPadKvp.Value.GunStartTimeStamp is not null)
                    contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps.Add(contestantScratchPadKvp.Value.GunStartTimeStamp);

                contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps.AddRange(contestantScratchPadKvp.Value.ListOfTimingMatTimeStamps);

                contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps = contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps.OrderBy(z => z.TimeStampBinaryFormat).ToList();
            }

            #endregion

            #region populate DnxRecordedByTimekeeper (if any)

            foreach (var contestantScratchPadKvp in _dictionaryOfEveryBodyWhoCompeted)
            {
                var dnxTimeStamps = contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps.Where(xx => !string.IsNullOrWhiteSpace(xx.DnxSymbol)).ToArray();

                if (dnxTimeStamps.Any())
                {
                    var xx = dnxTimeStamps.Last();

                    if (!string.IsNullOrWhiteSpace(xx.DnxSymbol))
                        contestantScratchPadKvp.Value.DnxRecordedByTimekeeper = xx.DnxSymbol;
                }
            }

            #endregion

            #region populate split intervals foreach contestant (regardless of whether Dnx or not because even Dnx's like to be able to see their intervals)

            foreach (var contestantScratchPadKvp in _dictionaryOfEveryBodyWhoCompeted)
            {
                if (!contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps.Any())
                    continue;

                if (contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps.Count() == 1)
                {
                    // aha. there is only one timestamp and therefore not even one interval

                    // and yet we have a timestamp. if the timestamp is a gun start, we can surmise that contestant didn't finish

                    if (contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps[0].RecordingModeEnum is EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody or EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup
                        or EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual)
                    {
                        contestantScratchPadKvp.Value.DnxSurmisedByThisAlgorithm = Symbols.SymbolDnf;
                        continue;
                    }

                    // or else the timestamp is a timing mat timestamp and we have no idea whatsoever if he failed to start or he failed to finish. default to DNS

                    contestantScratchPadKvp.Value.DnxSurmisedByThisAlgorithm = Symbols.SymbolIncomplete;

                    continue;
                }

                // success we have two or more timestamps and thus one or more PairedTimestampIntervals. populate them

                for (var i = 1; i < contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps.Count; i++)
                {
                    var thisInterval = new SplitIntervalAsPairOfTimeStampsItem
                    {
                        BeginningTimeStamp = contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps[i - 1],
                        EndingTimeStamp = contestantScratchPadKvp.Value.ConsolidatedListOfAllTimeStamps[i]
                    };

                    TimeSpan intervalDuration;

                    intervalDuration = DateTime.FromBinary(thisInterval.EndingTimeStamp.TimeStampBinaryFormat) -
                                       DateTime.FromBinary(thisInterval.BeginningTimeStamp.TimeStampBinaryFormat);

                    thisInterval.CalculatedIntervalDurationTicks = intervalDuration.Ticks;

                    contestantScratchPadKvp.Value.ListOfCalculatedPairedTimeStampIntervals.Add(thisInterval);
                }
            }

            #endregion

            #region deduce age group specification

            foreach (var contestantScratchPadKvp in _dictionaryOfEveryBodyWhoCompeted.Where(z => z.Value.Participant is not null))
            {
                var ageGroup = ParticipantDatabase.ToAgeCategoryDescriptionFromBirthYear(contestantScratchPadKvp.Value.Participant.BirthYear, _thisEventProfile?.EventSettingsItem?.AgeGroupSpecificationItems);

                contestantScratchPadKvp.Value.AgeGroupDeducedFromAgeGroupSpecificationsForEvent = ageGroup;
            }

            #endregion

            #region calculate tallies and totals

            foreach (var contestantScratchPadKvp in _dictionaryOfEveryBodyWhoCompeted)
            {
                contestantScratchPadKvp.Value.TallyOfTimingMatTimeStamps = contestantScratchPadKvp.Value.ListOfTimingMatTimeStamps.Count;
                contestantScratchPadKvp.Value.TallyOfSplitIntervals = contestantScratchPadKvp.Value.ListOfCalculatedPairedTimeStampIntervals.Count;
                contestantScratchPadKvp.Value.CalculatedCumulativeTotalDurationTicks = contestantScratchPadKvp.Value.ListOfCalculatedPairedTimeStampIntervals.Sum(z => z.CalculatedIntervalDurationTicks);
            }

            #endregion

            _repositoryIsBootstrapped = true;

            return true;
        }

        #endregion

        #region helpers

        private static SplitDurationConsolidationForParticipantItem[] PopulatePseudoRanksForSubsetsOfRaceWithinEventAsync(IEnumerable<SplitDurationConsolidationForParticipantItem> consolidatedSplitIntervalItems)
        {
            if (consolidatedSplitIntervalItems is null)
                return [];

            var dictionaryOfRaces = new JghListDictionary<string, SplitDurationConsolidationForParticipantItem>();

            foreach (var intervalsItem in consolidatedSplitIntervalItems.Where(z => z is not null))
            {
                dictionaryOfRaces.Add(intervalsItem.RaceGroupDeducedFromParticipant ?? string.Empty, intervalsItem);
            }

            foreach (var raceKvp in dictionaryOfRaces)
            {
                //if (string.IsNullOrWhiteSpace(raceKvp.Key))
                //    continue;

                var splitIntervals = raceKvp.Value
                    .OrderByDescending(z => z.TallyOfSplitIntervals)
                    .ThenBy(z => z.CalculatedCumulativeTotalDurationTicks);

                var i = 1;

                foreach (var intervalsItem in splitIntervals)
                {
                    var durationIsAnError = TimeSpan.FromTicks(intervalsItem.CalculatedCumulativeTotalDurationTicks).TotalSeconds < 1;

                    var isDnx = !string.IsNullOrWhiteSpace(intervalsItem.DnxRecordedByTimekeeper) || !string.IsNullOrWhiteSpace(intervalsItem.DnxSurmisedByThisAlgorithm);

                    if (durationIsAnError || isDnx)
                        continue;

                    intervalsItem.CalculatedRankOverall = i;

                    i += 1;
                }
            }

            var answer = new List<SplitDurationConsolidationForParticipantItem>();


            foreach (var raceKvp in dictionaryOfRaces.OrderByDescending(race => race.Value.Max(interval => interval.TallyOfSplitIntervals)))
            {
                var placedSplitIntervals = raceKvp.Value
                    .Where(z => z.CalculatedRankOverall != 0)
                    .OrderBy(intervalsItem => intervalsItem.CalculatedRankOverall)
                    .ThenBy(intervalsItem => intervalsItem.DnxRecordedByTimekeeper)
                    .ThenBy(intervalsItem => intervalsItem.DnxSurmisedByThisAlgorithm);

                answer.AddRange(placedSplitIntervals);

                var unPlacedSplitIntervals = raceKvp.Value
                    .Where(z => z.CalculatedRankOverall == 0)
                    .OrderBy(intervalsItem => intervalsItem.Bib);

                answer.AddRange(unPlacedSplitIntervals);
            }

            return answer.ToArray();
        }

        #endregion

        #region fields

        private bool _repositoryIsBootstrapped;

        private EventProfileItem _thisEventProfile;

        private readonly ParticipantDatabase _participantDatabase = new();

        private TimeStampHubItem _gunStartForEverybodyTimestamp;

        private Dictionary<string, TimeStampHubItem> _dictionaryOfContestantsWhoHadAnIndividualGunStart = new();

        private JghListDictionary<string, TimeStampHubItem> _listDictionaryOfContestantsWhoActivatedATimingMat = new();

        private Dictionary<string, TimeStampHubItem> _dictionaryOfRacesThatHadAGunStart = new();

        private Dictionary<string, SplitDurationConsolidationForParticipantItem> _dictionaryOfEveryBodyWhoCompeted = new(); // end of the line

        #endregion

        #region public methods - access data

        public SplitDurationConsolidationForParticipantItem[] GetTimeStampsAsSplitDurationsPerPersonInRankOrder(int anomalousThresholdForTooManySplits, int anomalousThresholdForTooFewSplits,
            double anomalousThresholdForTooBriefSplitMinutes)
        {
            if (!_repositoryIsBootstrapped)
                return [];

            if (_dictionaryOfEveryBodyWhoCompeted is null)
                return [];

            var rows = _dictionaryOfEveryBodyWhoCompeted.Select(z => z.Value).ToArray();

            if (!rows.Any())
                return [];

            #region riffle through row collection to identify anomalies add comments to them

            List<SplitDurationConsolidationForParticipantItem> answer = [];

            foreach (var row in rows.Where(z => z is not null))
            {
                row.Comment = string.Empty; // clear out prior work

                #region features of an item that are manifestly anomalous

                if (string.IsNullOrWhiteSpace(row.Bib))
                    row.Comment = JghString.ConcatAsSentences(row.Comment, "Missing ID.");
                else if (JghString.JghStartsWith(Symbols.SymbolUnspecified, row.Bib)) row.Comment = JghString.ConcatAsSentences(row.Comment, "Unassigned ID.");

                #endregion

                #region we choose to include Dnx in the identified anomalies

                if (!string.IsNullOrWhiteSpace(row.DnxRecordedByTimekeeper))
                    row.Comment = JghString.ConcatAsSentences(row.Comment, "Recorded as a Dnx by timekeeper.");
                else if (!string.IsNullOrWhiteSpace(row.DnxSurmisedByThisAlgorithm)) row.Comment = JghString.ConcatAsSentences(row.Comment, "Surmised as a Dnx by this algorithm.");

                #endregion

                #region features of the sequence of interval durations that are suspicious

                if (anomalousThresholdForTooManySplits > 0)
                    if (row.TallyOfSplitIntervals >= anomalousThresholdForTooManySplits)
                        row.Comment = JghString.ConcatAsSentences(row.Comment, "Too many splits.");

                if (anomalousThresholdForTooFewSplits > 0)
                    if (row.TallyOfSplitIntervals <= anomalousThresholdForTooFewSplits)
                        row.Comment = JghString.ConcatAsSentences(row.Comment, "Too few splits.");

                if (anomalousThresholdForTooBriefSplitMinutes > 0)
                {
                    var xx = row.ListOfCalculatedPairedTimeStampIntervals
                        .Any(z => TimeSpan.FromTicks(z.CalculatedIntervalDurationTicks).TotalSeconds < anomalousThresholdForTooBriefSplitMinutes * 60);

                    if (xx) row.Comment = JghString.ConcatAsSentences(row.Comment, "Too fast split.");
                }

                #endregion

                #region details of the attached participant profile that are potential show stoppers

                if (row.Participant is null)
                {
                    row.Comment = JghString.ConcatAsSentences(row.Comment, "Unknown person. No visible participant has this ID.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(row.Participant.RaceGroupBeforeTransition)) row.Comment = JghString.ConcatAsSentences(row.Comment, "RaceGroup not specified."); // RaceGroupAfterTransition is not necessary in this algorithm

                    if (string.IsNullOrWhiteSpace(row.Participant.Gender)) row.Comment = JghString.ConcatAsSentences(row.Comment, "Gender not specified.");

                    if (string.IsNullOrWhiteSpace(row.Participant?.FirstName))
                        row.Comment = JghString.ConcatAsSentences(row.Comment, "FirstName not specified.");

                    if (string.IsNullOrWhiteSpace(row.Participant?.LastName))
                        row.Comment = JghString.ConcatAsSentences(row.Comment, "LastName not specified.");

                    if (string.IsNullOrWhiteSpace(row.AgeGroupDeducedFromAgeGroupSpecificationsForEvent)) row.Comment = JghString.ConcatAsSentences(row.Comment, "Age group indeterminate.");
                }

                #endregion

                answer.Add(row);
            }

            #endregion

            answer = PopulatePseudoRanksForSubsetsOfRaceWithinEventAsync(answer).ToList();

            return answer.ToArray();
        }

        public ResultDto[] GetDraftResultItemDataTransferObjectForAllContestantsInRankOrder()
        {
            if (!_repositoryIsBootstrapped)
                return [];

            var temp = _dictionaryOfEveryBodyWhoCompeted.Select(z => z.Value).ToArray();

            temp = PopulatePseudoRanksForSubsetsOfRaceWithinEventAsync(temp);

            var answer = new List<ResultDto>();

            foreach (var consolidatedSplitIntervalsItem in temp)
            {
                var resultItemDataTransferObject = SplitDurationConsolidationForParticipantItem.ToResultItemDataTransferObject(consolidatedSplitIntervalsItem);

                if (consolidatedSplitIntervalsItem.Participant is not null)
                    resultItemDataTransferObject.AgeGroup = ParticipantDatabase.ToAgeCategoryDescriptionFromBirthYear(consolidatedSplitIntervalsItem.Participant.BirthYear, _thisEventProfile?.EventSettingsItem?.AgeGroupSpecificationItems);

                answer.Add(resultItemDataTransferObject);
            }

            return answer.ToArray();
        }

        #endregion
    }
}