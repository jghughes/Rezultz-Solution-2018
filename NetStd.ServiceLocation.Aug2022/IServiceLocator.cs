using System;
using System.Collections.Generic;

//using CommonServiceLocator;

namespace NetStd.ServiceLocation.Aug2022
{
    /// <summary>
    ///     For more info on satisfying the requirements of IServiceLocator, read https://github.com/unitycontainer/commonservicelocator
    ///     This class is a cut and paste of that class.
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        object GetInstance(Type typeOfInstance);

        object GetInstance(Type typeOfInstance, string key);

        IEnumerable<object> GetAllInstances(Type typeOfInstance);

        TInstance GetInstance<TInstance>();

        TInstance GetInstance<TInstance>(string key);

        IEnumerable<TInstance> GetAllInstances<TInstance>();
    }
}