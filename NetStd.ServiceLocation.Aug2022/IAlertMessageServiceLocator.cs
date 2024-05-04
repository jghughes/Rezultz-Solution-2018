using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.ServiceLocation.Aug2022
{
    public interface IAlertMessageServiceLocator
    {
        void DeRegisterIAlertMessageServiceProvider();
        void RegisterIAlertMessageServiceProvider(IAlertMessageService implementingObject);
    }
}