namespace Novellus.Core
{
    public interface IRequest<out TResponse> where TResponse : IResponse
    {
    }
}
