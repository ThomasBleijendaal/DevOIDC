using DevOidc.Business.Abstractions;
using DevOidc.Functions.Extensions;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Resolvers
{
    internal class BaseUriResolver : IBaseUriResolver
    {
        private readonly IFunctionExecutionContextAccessor _functionExecutionContextAccessor;

        public BaseUriResolver(IFunctionExecutionContextAccessor functionExecutionContextAccessor)
        {
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
        }

        public string? ResolveBaseUri(string readTo)
            => _functionExecutionContextAccessor.FunctionExecutionContext?.GetBaseUri(readTo);
    }
}
