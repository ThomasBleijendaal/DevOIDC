using DevOidc.Business.Abstractions;
using DevOidc.Functions.Extensions;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Resolvers
{
    internal class BaseUriResolver : IBaseUriResolver
    {
        private readonly IFunctionContextAccessor _functionContextAccessor;

        public BaseUriResolver(IFunctionContextAccessor functionContextAccessor)
        {
            _functionContextAccessor = functionContextAccessor;
        }

        public string? ResolveBaseUri(string readTo)
            => _functionContextAccessor.FunctionExecutionContext?.GetBaseUri(readTo);
    }
}
