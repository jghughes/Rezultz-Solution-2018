Tool01:	This program is intended to be a dirty little throw-away scratch pad. Do your work in Main01(). Erase it when you are finished.

Tool02:	This program (Tool02) de-serialises .JSON files, then exports tidied up XML and JSON files.

Tool03:	This program (Tool03 ) reads one or more files of MyLaps data (in problematic CSV format). It tediously converts the data into ResultDto items, relying on the participant master list from RezultzHub and parameters for the specified event from the SeriesProfile to plug gaps in the (incomplete) MyLaps data. Finaly, it exports the manicured file/s of publishable XML results for the event for upload to Azure.

Tool04:	This program (Tool04) reads one or more XML files of published results data from the Portal. For every ResultItemDto in each of those files, it overwrites the T01 field with the corresponding field obtained from the three associated MyLaps data files (in CSV format). Then it exports tidied up file/s of XML.

Tool05:	"This program (Tool 05) is used to do four things." +
        " 1. be a test bed for new publisher-profile files authored in XML for future publisher modules." +
        " 2. ensure that the authored profile-files deserialise correctly using the system serialiser at the client end of the remote svc wire." +
        " 3. maintain a handwritten free-form deserialiser that can deserialise profile-files (used for exception fallback at the remote svc end). " +
        " 4. be a test bed for a handwritten XML fragment reader to extract custom information out of the profile-files for sole use at the remote svc end." +
        " A minimally valid profile-file includes a <ThisFileNameFragment> element and <CSharpModuleCodeName> element." +
        " These paired elements tie the specifications in a publisher profile file to the functions of a paired C# code module." +
        " The program is used to debug new xml profiles used for serialisation as a serialised [PublisherModuleProfileItemDto]." +
        " The beauty is that the XML is designed to be extensible, it can be free-form to an extent. Each C# module can have its" +
        " own customised profile that contains settings, parameters, and data that is specific to the code module." +
        " At minimum, the profile must populate all the members of a valid [PublisherModuleProfileItemDto]." +
        " Beyond that, additional elements are optional and customised. The problem we are circumventing with a " +
        " handwritten deserialiser is that it caters for the extensibilty. We deserialise the profile file" +
        " in two steps: first we use the system deserialiser to create a [PublisherModuleProfileItemDto] object." +
        " This gets us the essential elements of the file. The we use a totally simple little custom manual de-serialiser" +
        " to fish out custom XElements manually. Once debugged, copy and paste the perfected methods into your module." +
        " For 3. cut and paste the method perfected here into private [IPublisher.PublisherBase.ManuallyDeserialisePublisherProfileDtoFromXml()]" +
        " For 4. cut and paste the method perfected here into abstract [IPublisher.PublisherXXXXX.ExtractCustomElementsFromAssociatedProfile()]";

Tool06:	This program (Tool06) is a scratchpad for testing RaceResultsPublishingSvcAgent and the C# publisher module [PublisherForMyLapsElectronicTimingSystem2023].

Tool07:	This program (Tool07) reads a folder of files of published results and replaces all the XElement names to lower case.

Tool08:	This program (Tool08) is a scratchpad for testing RaceResultsPublishingSvcAgent, their injected Wcf or Mvc clients and services, and the C# publisher profile and module [PublisherForRezultzPortalTimingSystem2021].

Tool09:	"This program (Tool09) reads files of MyLaps timing data and compares the electronic " +
"data from the timing mat to a (potentially totally different) set of timing data recorded in " +
"the timing tent using the Portal timing system. The purpose of doing this is to search for gaps " +
"in the data by means of comparison. Based on empirical experience, there are typically about ten " +
"anomalies each week because the electronic mat misses people. With a double-mat for redundancy, the anomalies are reduced " +
"The Portal timing team also misses people, but generally fewer. The pipeline for this tool is that data is exported from MyLaps in Excel " +
"format, then exported from Excel in .csv format, and then finally imported into this tool as .csv and " +
"deserialized and analysed. Portal data is exported effortlessly by clicking the export button on the " +
"Portal Timing Tools screen and exporting the data as timestamps consolidated into provisional results.";

Tool10:	This program (Tool10) reads the Kelso Master List of people registered before the start of the season for the series. It generates equivalent ParticipantHubItems for upload to the hub so that the Portal is ready to go from Day one.

Tool11:	This mickey-mouse program (Tool11) reads the Kelso participant master list/s and determines which participants have birthdays mid-season that would trigger a change in age-group if age-group was variable and based on actual age on the day of an event.

Tool12: "This program (Tool12) is intended to indentify discrepencies between the Kelso SeriesPoints-participant lists in Andrew's four-worksheet " +
"Excel spreadsheet and the ParticipantHubItems in the portal. To obtain the Kelso list, it reads XML files exported from Andrew's SERIES-POINTS " +
"spreadsheet (exported by JGH into four worksheets in Access named Expert, Intermediate, Novice, and Sport and then exported as XML). To obtain an " +
"up-to-date master list of the participants registered in the Portal, JGH exports the master list from the Particpant registration module as a JSON file."

Tool13:	"This program (Tool13) reads files of the published results for all events in the series to date " +
"and then determines which participants have raced on more than one category because they upgraded or " +
"downgraded, or were mis-categorised, or just rode as a bandit in an illegitimate category. If asked, it can " +
"also inspect an arbitrary list of bibs."

Tool14:

Tool15:







