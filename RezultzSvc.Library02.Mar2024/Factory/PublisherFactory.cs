using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Xml.July2018;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using RezultzSvc.Library02.Mar2024.PublisherModules;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace RezultzSvc.Library02.Mar2024.Factory;

/// <summary>
///     Factory for instantiating publisher modules for timing-tent data.
/// </summary>
public class PublisherFactory
{
    private const string Locus2 = nameof(PublisherFactory);
    private const string Locus3 = "[RezultzSvc.Library02.Mar2024]]";

    #region important consts - CSharpModuleCodeName is defined in the publisher profile XML files. Keep these in sync with those 

    public const string ModuleCodeNameForKelso2013to2016Cx = "FilesFromCrossMgrEditedManuallyByJGH2013";
    public const string ModuleCodeNameForKelso2015to2019Mtb = "KelsoFilesFromAJLeemingViaAccess2015";
    public const string ModuleCodeNameForRezultzPortalTimingSystem2021 = "RezultzPortalTimingSystem2021";
    public const string ModuleCodeNameForMyLaps2023csv = "MyLapsElectronicTimingSystem2023csv";
    public const string ModuleCodeNameForMyLaps2024csv = "MyLapsElectronicTimingSystem2024csv";
    public const string ModuleCodeNameForMyLaps2024xml = "MyLapsElectronicTimingSystem2024xml";

    private readonly Dictionary<string, ModuleParticulars> DictionaryOfCSharpModuleParticulars = new()
    {
        {ModuleCodeNameForKelso2013to2016Cx,new ModuleParticulars(ModuleCodeNameForKelso2013to2016Cx, @"2016.08.01") },
        {ModuleCodeNameForKelso2015to2019Mtb,new ModuleParticulars(ModuleCodeNameForKelso2015to2019Mtb,  @"2016.05.01") },
        {ModuleCodeNameForRezultzPortalTimingSystem2021,new ModuleParticulars(ModuleCodeNameForRezultzPortalTimingSystem2021, @"2021.06.24") },
        {ModuleCodeNameForMyLaps2023csv,new ModuleParticulars(ModuleCodeNameForMyLaps2023csv, @"2023.05.30") },
        {ModuleCodeNameForMyLaps2024csv,new ModuleParticulars(ModuleCodeNameForMyLaps2024csv, @"2024.05.30") },
        {ModuleCodeNameForMyLaps2024xml,new ModuleParticulars(ModuleCodeNameForMyLaps2024xml, @"2024.05.23") },
    };

    #endregion

    #region methods

    internal IPublisher ManufacturePublisher(XElement profileXe)
    {
        const string failure = "Unable to create publishing module coded in C-Sharp.";
        const string locus = "[ManufacturePublisher]";


        string requiredCSharpModuleCodeName = JghXElementHelpers.ChildElementValueAsString(profileXe, PublisherModuleProfileItemDto.XeCSharpModuleId);

        try
        {
            #region ensure that we have a CSharpModuleCodeName that complies with the codename in the publisher profile file i.e. that a code module exists. If not, bale.

            if (profileXe is null)
                throw new JghPublisherServiceFaultException("Unable to proceed. Publisher profile xml file is empty. Unable to build publisher.");


            if (string.IsNullOrWhiteSpace(requiredCSharpModuleCodeName))
                throw new JghPublisherServiceFaultException("Unable to proceed. The publisher profile xml file is missing the name of the publisher module. Unable to build publisher.");

            if (!DictionaryOfCSharpModuleParticulars.ContainsKey(requiredCSharpModuleCodeName))
            {
                var provisionalErrorMsgSb = new StringBuilder();

                provisionalErrorMsgSb.AppendLine(
                    "Unable to proceed. Unable to build publisher. There is a disconnect between the name of the publisher module in the publisher profile xml file and the available modules listed in PublisherFactory.DictionaryOfCSharpModuleParticulars. Sorry. This is a system error. For information, the listed names are the following:");

                provisionalErrorMsgSb.AppendLine();
                foreach (var kvp in DictionaryOfCSharpModuleParticulars)
                    provisionalErrorMsgSb.AppendLine($"CSharpModule :  <{kvp.Key, -40}>    Version : {kvp.Value.CSharpModuleVersionNumber}");

                throw new JghPublisherServiceFaultException(provisionalErrorMsgSb.ToString());
            }

            #endregion

            #region Dowork

            IPublisher publisher = requiredCSharpModuleCodeName switch
            {
                ModuleCodeNameForKelso2013to2016Cx => new PublisherForKelsoCrossMgr2013To2016(),
                ModuleCodeNameForKelso2015to2019Mtb => new PublisherForKelsoMtb2015To2019(),
                ModuleCodeNameForRezultzPortalTimingSystem2021 => new PublisherForRezultzPortalTimingSystem2021(),
                ModuleCodeNameForMyLaps2023csv => new PublisherForMyLapsElectronicTimingSystem2023(),
                ModuleCodeNameForMyLaps2024csv => new PublisherForMyLapsElectronicTimingSystem2024Csv(),
                ModuleCodeNameForMyLaps2024xml => new PublisherForMyLapsElectronicTimingSystem2024Xml(),
                _ => throw new JghPublisherServiceFaultException(
                    $"Unable to proceed. Unable to instantiate C# module. Codename of failing module is <{requiredCSharpModuleCodeName}>. Sorry. This is a coding error in the publishing service.[PublisherFactory.ManufacturePublisher()]")
            };

            publisher.AssociatedProfileFile = profileXe;

            return publisher;

            #endregion
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region classes

    public class ModuleParticulars
    {
        public string CSharpModuleCodeName { get; set; }

        public string CSharpModuleVersionNumber { get; set; }

        public ModuleParticulars(string cSharpModuleCodeName, string cSharpModuleVersionNumber)
        {
            CSharpModuleCodeName = cSharpModuleCodeName ?? string.Empty;
            CSharpModuleVersionNumber = cSharpModuleVersionNumber ?? string.Empty;
        }
    }

    #endregion

}