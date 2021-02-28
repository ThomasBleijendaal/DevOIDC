using System.Threading.Tasks;

namespace DevOidc.Business.Abstractions
{
    public interface IOidcHandler<TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}
