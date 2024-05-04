namespace RezultzSvc.Clients.Mvc.Mar2023
{
    internal class StringsMar2023
    {
        public const string NoConnection = "No connection.";
        public const string HttpRequestExceptionThrown = "A HttpRequestException was thrown.";
        public const string ApiExceptionThrown = "An ApiException was thrown locally.";
        public const string ServerExceptionThrown = "An exception was thrown on the server.";

        public const string CallFailed = "Call failed.";
        public const string CallTimedOut = CallFailed + " " + "Call timed out.";
        public const string CallInvalid = CallFailed + " " + "InvalidOperationException thrown. For starters, check that the requestUri is an absolute URI and that BaseAddress has been set.";

    }
}
