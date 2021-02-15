namespace DevOidc.Business.Abstractions
{
    public interface IBaseUriResolver
    {
        public string? ResolveBaseUri(string readTo);
    }
}
