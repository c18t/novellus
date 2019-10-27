namespace Novellus.Core
{
    public interface IUseCase<in TRequest, out TResponse> where TRequest : IRequest<TResponse> where TResponse : IResponse
    {
        public TResponse Handle(TRequest response);
    }
}
