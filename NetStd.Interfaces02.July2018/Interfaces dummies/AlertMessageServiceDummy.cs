using System;
using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.Interfaces02.July2018.Interfaces_dummies
{
    public class AlertMessageServiceDummy : IAlertMessageService
    {
        #region async version

        public void Show(string message)
        {
        }

        public Task<int> ShowOkAsync(string message)
        {
            return Task.FromResult(0);
        }

        public Task<int> ShowOkAsync(Exception ex)
        {
            return Task.FromResult(0);
        }

        public Task<int> ShowOkAsync(string message, Exception ex)
        {
            return Task.FromResult(0);
        }

        public Task<int> ShowErrorOkAsync(Exception ex)
        {
            return Task.FromResult(0);
        }

        public Task<int> ShowErrorOkAsync(string message, Exception ex)
        {
            return Task.FromResult(0);
        }

        public Task<int> ShowOkCancelAsync(string message, string messageCaption, string secondaryMessage)
        {
            return Task.FromResult(0);
        }

        public Task<int> ShowErrorOkCancelAsync(string message, string messageCaption, Exception ex)
        {
            return Task.FromResult(0);
        }

        public Task<int> ShowNotificationOrErrorMessageAsync(string failure, string locus, string locus2,
            string locus3, Exception ex)
        {
            return Task.FromResult(0);
        }

        #endregion

        #region sync version

        //public bool ShowOk(string message)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool ShowOk(Exception ex)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool ShowOk(string message, Exception ex)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool ShowErrorOk(Exception ex)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool ShowErrorOk(string message, Exception ex)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool? ShowOkCancel(string message, string messageCaption, string secondaryMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool? ShowErrorOkCancel(string message, string messageCaption, Exception ex)
        //{
        //    throw new NotImplementedException();
        //}


        //public bool ShowSimpleMessageOrFullRedactedExceptionMessage(string failure, string locus, string locus2, string locus3, Exception ex)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}