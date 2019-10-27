namespace Novellus.FormTests
{
    using Novellus.Core;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class UseCaseBus
    {
        private readonly Dictionary<Type, Type> interactorTypes = new Dictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, IUseCaseInvoker> invokers = new ConcurrentDictionary<Type, IUseCaseInvoker>();

        public TResponse Handle<TResponse>(IRequest<TResponse> request) where TResponse : IResponse
        {
            IUseCaseInvoker invoker = this.Invoker(request);
            return invoker.Invoke<TResponse>(request);
        }

        public async Task<TResponse> HandleAync<TResponse>(IRequest<TResponse> request) where TResponse : IResponse
        {
            IUseCaseInvoker invoker = this.Invoker(request);
            TResponse result = await Task.Run(() => invoker.Invoke<TResponse>(request));
            return result;
        }

        internal void Register<TRequest, TInteractor>() where TRequest : IRequest<IResponse> where TInteractor : IUseCase<TRequest, IResponse>
        {
            this.interactorTypes.Add(typeof(TRequest), typeof(TInteractor));
        }

        private IUseCaseInvoker Invoker<TResponse>(IRequest<TResponse> request) where TResponse : IResponse
        {
            Type requestType = request.GetType();
            if (this.invokers.TryGetValue(requestType, out IUseCaseInvoker searchedInvoker))
            {
                return searchedInvoker;
            }

            if (!this.interactorTypes.TryGetValue(requestType, out Type interactorType))
            {
                throw new InvalidOperationException($"the usecase has not been registered for {requestType.GetType().Name}");
            }

            IUseCaseInvoker invoker = invokers.GetOrAdd(requestType, _ => {
                object invokerInstance = typeof(DependencyService).GetMethod(nameof(DependencyService.Resolve), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(IUseCaseInvoker)).Invoke(null, null);
                object interactorInstance = typeof(DependencyService).GetMethod(nameof(DependencyService.Resolve), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(IUseCase<IRequest<TResponse>, TResponse>)).Invoke(null, null);
                return (IUseCaseInvoker)invokerInstance.GetType().GetConstructor(new Type[] { typeof(Type), typeof(Type) }).Invoke(new object[] { interactorType, interactorInstance.GetType() });
            });

            return invoker;
        }
    }
}
