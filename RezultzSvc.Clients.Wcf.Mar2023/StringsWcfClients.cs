namespace RezultzSvc.Clients.Wcf.Mar2023
{
    internal class StringsWcfClients
    {
        public const string NoConnection = "No connection.";
        public const string OperationAborted = "Operation aborted.";
        public const string ProblemEncountered = "The remote service faulted.";

        public const string CallFailed = "Call failed.";
        public const string CallBounced = CallFailed + " " + "There was a communication problem with the server. Call bounced back from the receiving end.";
        public const string CallTimedOut = CallFailed + " " + "Call timed out.";

    }
}
