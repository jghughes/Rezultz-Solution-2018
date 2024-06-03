using System.Runtime.Serialization;
using NetStd.Goodies.Mar2022;

// NB. never ever refactor the class name or member names because they are cast in concrete in all MVC and WCF service signatures
// do not rename these faults it will mess everything up unless you recompile your WCF services, republish them and recompile your client side classes

namespace RezultzSvc.WebApp03
{
    [DataContract]
    public class JghFault
    {
        #region ctor

        public JghFault(string narrative)
    {
        Narrative = narrative;
    }

        public JghFault(JghError error)
        {
            var theList = new List<string>();

            if (!string.IsNullOrWhiteSpace(error.Detail))
                theList.Add($"[Detail: {error.Detail}]");

            if (!string.IsNullOrWhiteSpace(error.Source))
                theList.Add($"[Source: {error.Source}]");

            if (error.Status != 0)
                theList.Add($"[Status: {error.Status}]");

            if (!string.IsNullOrWhiteSpace(error.Code))
                theList.Add($"[Code: {error.Code}]");

#if DEBUG
            if (!string.IsNullOrWhiteSpace(error.Type))
                theList.Add($"[Type: {error.Type}]");
#endif



            Narrative = JghString.ConcatAsParagraphs(theList.ToArray());

        }

        public JghFault(string narrative, JghError error)
        {
            var theList = new List<string>();

            if (!string.IsNullOrWhiteSpace(error.Detail))
                theList.Add($"[Detail: {error.Detail}]");

            if (!string.IsNullOrWhiteSpace(error.Source))
                theList.Add($"[Source: {error.Source}]");

            if (error.Status != 0)
                theList.Add($"[Status: {error.Status}]");

            if (!string.IsNullOrWhiteSpace(error.Code))
                theList.Add($"[Code: {error.Code}]");

#if DEBUG
            if (!string.IsNullOrWhiteSpace(error.Type))
                theList.Add($"[Type: {error.Type}]");
#endif

            Narrative =JghString.ConcatAsParagraphs(narrative, JghString.ConcatAsParagraphs(theList.ToArray()));

        }

        #endregion

        #region props

        [DataMember(EmitDefaultValue = false, IsRequired = false)] public string Narrative { get; set; }


        #endregion
    }
}