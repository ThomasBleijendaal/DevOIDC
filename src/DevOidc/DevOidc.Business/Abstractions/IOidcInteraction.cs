using System.Threading.Tasks;
using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Business.Abstractions
{
    public interface IOidcInteraction<TRequest, TResponse>
    {
        Task<TResponse> InteractionAsync(TRequest request);
    }
}
