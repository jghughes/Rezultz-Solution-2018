using CoreWCF;
using CoreWCF.Channels;

namespace RezultzSvc.WebApp01;

public static class BindingConfigs
{
    // Note: the endpoint names are discretionary, but they must be unique. The Svc client wizard uses them to generate titles for the EndpointConfiguration enums in Reference.cs in the client project.

    #region bindings

    public static BasicHttpBinding GetHttpTextBinding()
    {

        var answer = new BasicHttpBinding(BasicHttpSecurityMode.None)
        {
            Name = "MyHttpTextBinding",
            CloseTimeout = TimeSpan.FromSeconds(20),
            OpenTimeout = TimeSpan.FromSeconds(20),
            ReceiveTimeout = TimeSpan.FromSeconds(20),
            SendTimeout = TimeSpan.FromSeconds(20),
            MaxBufferSize = int.MaxValue,
            MaxReceivedMessageSize = long.MaxValue,
        };

        return answer;
    }

    public static BasicHttpBinding GetHttpsTextBinding()
    {

        var answer = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
        {
            Name = "MyHttpsTextBinding",
            CloseTimeout = TimeSpan.FromSeconds(20),
            OpenTimeout = TimeSpan.FromSeconds(20),
            ReceiveTimeout = TimeSpan.FromSeconds(20),
            SendTimeout = TimeSpan.FromSeconds(20),
            MaxBufferSize = int.MaxValue,
            MaxReceivedMessageSize = long.MaxValue,
        };

        return answer;
    }

    public static CustomBinding GetCustomBindingWithHttpsBinaryEncoding()
    {
        var answer = new CustomBinding
        {
            Name = "MyHttpsCustomBinaryBinding",
            CloseTimeout = TimeSpan.FromSeconds(20),
            OpenTimeout = TimeSpan.FromSeconds(20),
            ReceiveTimeout = TimeSpan.FromSeconds(20),
            SendTimeout = TimeSpan.FromSeconds(20)
        };

        answer.Elements.Add(new BinaryMessageEncodingBindingElement());

        answer.Elements.Add(new HttpsTransportBindingElement
        {
            MaxBufferSize = int.MaxValue,
            MaxReceivedMessageSize = int.MaxValue
        });

        return answer;
    }


    #endregion

}