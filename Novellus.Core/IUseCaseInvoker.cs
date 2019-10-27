namespace Novellus.Core
{
    public interface IUseCaseInvoker
    {
        public TResponse Invoke<TResponse>(object request) where TResponse : IResponse;
    }
}
