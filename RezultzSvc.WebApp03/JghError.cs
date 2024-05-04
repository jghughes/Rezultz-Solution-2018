

// NB. never ever refactor the class name or member names because they are cast in concrete in all MVC and WCF service signatures
// do not rename these faults it will mess everything up unless you recompile your WCF services, republish them and recompile your client side classes

using System.Runtime.Serialization;

namespace RezultzSvc.WebApp03
{

	[DataContract]
	public class JghError
	{
		#region ctor

		public JghError(Exception ex)
		{
			Type = ex.GetType().ToString();
			Status = 0;
			Code = string.Empty;
			Detail = ex.Message;
			Source = ex.Source ?? string.Empty;
		}

		public JghError(int httpStatus, string code, string detail)
		{
			Type = string.Empty;
			Status = httpStatus;
			Code = code;
			Detail = detail;
			Source = string.Empty;
		}

		public JghError(string type, int httpStatus, string code, string detail, string source)
		{
			Type = type;
			Status = httpStatus;
			Code = code;
			Detail = detail;
			Source = source;

		}

		#endregion

		#region props

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Type { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public int Status { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Code { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Detail { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Source { get; set; }

		#endregion
	}
}