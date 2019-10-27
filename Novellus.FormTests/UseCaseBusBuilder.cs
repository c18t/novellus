namespace Novellus.FormTests
{
    using Novellus.Core;
    using Xamarin.Forms;

    public class UseCaseBusBuilder
    {
        private bool isBuilt = false;
        private readonly UseCaseBus bus = new UseCaseBus();

        public UseCaseBusBuilder()
        {
            Novellus.App.RegisterType<IUseCaseInvoker, UseCaseInvoker>();
        }

        public UseCaseBus Build() {
            Novellus.App.BuildSeriveProvider();
            this.isBuilt = true;
            return this.bus;
        }

        public void RegisterUseCase<TRequest, TInteractor>() where TRequest : IRequest<IResponse> where TInteractor : class, IUseCase<TRequest, IResponse>
        {
            if (isBuilt)
            {
                throw new InvalidNavigationException($"{nameof(RegisterUseCase)} called after building {bus.GetType()}");
            }

            Novellus.App.RegisterType<IUseCase<TRequest, IResponse>, TInteractor>();
            bus.Register<TRequest, TInteractor>();
        }
    }
}
