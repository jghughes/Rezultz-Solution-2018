using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using NetStd.ServiceLocation.Aug2022;

/*
 * This class is a wrapper for the open source DryIoc container. The beauty is that NuGet dropped the source code dropped into this project!
 * For everything about DryIoc including user documentation see https://github.com/dadhi/DryIoc
 * You can clone this class to use any container you like, such as the the GalaSoft MvvmLight container
 * or the Unity container in Microsoft.Extensions.DependencyInjection for example, since deprecated.
 * Prism uses the DriIoc container. If the source code ever stops working, go find out what PRISM has switched to.
 * For info on using DI in general, see https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/february/mvvm-ioc-containers-and-mvvm
 * For a more modern take on DI see https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/ioc
 */

namespace NetStd.DependencyInjection.Mar2024
{
    public class DependencyInjectionContainer : IServiceLocator
    {
        private readonly Container _container;

        public DependencyInjectionContainer()
        {
            _container = new Container();
        }

        #region registration methods

        public void Register<T>(T myClass, string key) where T : class, new()
        {
            try
            {
                UnRegister<T>();

                if (key is null)
                    _container.Register<T>(Reuse.Singleton);
                else
                    _container.Register<T>(serviceKey: key);
                //_container.Register(() => new T(), key);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {nameof(T)} with Key={key} failed to be successfully registered in container.");
            }
        }

        public void Register<T>() where T : class
        {
            try
            {
                UnRegister<T>();

                _container.Register<T>(Reuse.Singleton);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {nameof(T)} failed to be successfully registered in container.");
            }
        }

        public void Register<TInterface>(Func<TInterface> instanceReturningExpression) where TInterface : class
        {
            try
            {
                UnRegister<TInterface>();

                TInterface xx = instanceReturningExpression();

                if (xx is not null)
                    //_container.Use(xx);
                    _container.RegisterInstance(xx);
                //_container.UseInstance(xx);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {typeof(TInterface)} failed to be successfully registered in container using the factory method argument named {nameof(instanceReturningExpression)}.");
            }
        }

        public void Register<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
        {
            try
            {
                UnRegister<TInterface>();

                _container.Register<TInterface, TImplementation>(Reuse.Singleton);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {typeof(TInterface)} {typeof(TImplementation)} failed to be successfully registered in container.");
            }
        }

        public bool IsRegistered<TInterface>() where TInterface : class
        {
            try
            {
                return _container.IsRegistered<TInterface>();
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} Registration of {typeof(TInterface)} in container is indeterminate.");
            }
        }

        public void UnRegister<TInterface>() where TInterface : class
        {
            try
            {
                if (_container.IsRegistered<TInterface>())
                    _container.Unregister<TInterface>();
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} Failed to un-register {typeof(TInterface)} in container.");
            }
        }

        #endregion

        #region retrieval methods

        public object GetService(Type typeOfService)
        {
            try
            {
                return _container.Resolve(typeOfService);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {nameof(typeOfService)} not registered in container or failed to resolve successfully.");
            }
        }

        public object GetInstance(Type typeOfInstance)
        {
            try
            {
                return _container.Resolve(typeOfInstance);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {nameof(typeOfInstance)} not registered in container or failed to resolve successfully.");
            }
        }

        public object GetInstance(Type typeOfInstance, string key)
        {
            try
            {
                if (key is null)
                    _container.Resolve(typeOfInstance);

                return _container.Resolve(typeOfInstance, key);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {nameof(typeOfInstance)} with Key={key} not registered in container or failed to resolve successfully.");
            }
        }

        public IEnumerable<object> GetAllInstances(Type typeOfInstance)
        {
            try
            {
                return _container.ResolveMany(typeOfInstance);
            }
            catch (Exception e)
            {
                throw new ActivationException(
                    $"{e.Message} {nameof(typeOfInstance)} not registered in container or failed to resolve successfully.");
            }
        }

        public TInstance GetInstance<TInstance>()
        {
            try
            {
                return (TInstance)GetInstance(typeof(TInstance), null);
            }
            catch (Exception e)
            {
                throw new ActivationException(e.Message);
            }
        }

        public TInstance GetInstance<TInstance>(string key)
        {
            try
            {
                return (TInstance)GetInstance(typeof(TInstance), key);
            }
            catch (Exception e)
            {
                throw new ActivationException(e.Message);
            }
        }

        public IEnumerable<TInstance> GetAllInstances<TInstance>()
        {
            try
            {
                var answers = GetAllInstances(typeof(TInstance));

                return answers.Cast<TInstance>().ToArray();
            }
            catch (Exception e)
            {
                throw new ActivationException(e.Message);
            }
        }

        #endregion
    }
}