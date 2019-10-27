namespace Novellus.FormTests
{
    using Novellus.Core;
    using System;
    using System.Reflection;
    using Xamarin.Forms;

    public class UseCaseInvoker : IUseCaseInvoker
    {
        private readonly MethodInfo resolverMethod;
        private readonly MethodInfo handleMethod;

        public UseCaseInvoker()
        {

        }

        public UseCaseInvoker(Type usecaseType, Type implementsType)
        {
            this.resolverMethod = typeof(DependencyService).GetMethod(nameof(DependencyService.Resolve), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(usecaseType);
            this.handleMethod = implementsType.GetMethod(nameof(IUseCase<IRequest<IResponse>, IResponse>.Handle));
        }

        public TResponse Invoke<TResponse>(object request) where TResponse : IResponse
        {
            object instance = resolverMethod.Invoke(null, null);
            object response;
            try
            {
                response = handleMethod.Invoke(instance, new[] { request });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (TResponse)response;
        }
    }
}
