using DevOidc2.Domain;

namespace DevOidc2.Services;

public interface IMetadataService
{
    Task<Result<Metadata>> GetMetadataAsync(Guid tenantId);
    Task<Result<KeyMetadata>> GetPublicKey(Guid tenantId);
}

public class MetadataService : IMetadataService
{
    public Task<Result<Metadata>> GetMetadataAsync(Guid tenantId)
    {
        return Task.FromResult(Result.Success(
            new Metadata(["sub", "iss", "aud", "exp", "email"])));
    }

    public Task<Result<KeyMetadata>> GetPublicKey(Guid tenantId)
    {
        return Task.FromResult(Result.Success(
            new KeyMetadata("RSA", "RS256", "sig", "kUdxqabateU0zbuWliSslNH6xR4", "2tX4Mppg71cgxSLoNbHfdbtQpFwrGiADZLogDlu4RzDkr8+avDnUHQ6q/S+8/ORr4qd7ImDZZi8cnwlomQS94PO/DqQ2ib9lt1XHXT6o+/fgvaOvFr7slOZzHHq7F1qhCKa8Lr2xmjUVuyqnxUYsBIZtevgjhMVPeY0WYX+wznhAnQWCmgK0eqytlITkCyJSrEt/parpoN+OnZbr6/hNJXeGDvL66upvDKdt8D7Yvop2pYYnSf6fYjrShjqwp/wbAcwtXZZUee6jHETcArd7PxQBIPAkeQB+BTBrgOqihVI8wy2DwYiMbQftMo6KlKCcsnusTDODgimOTkF6O88Z6Q==", "AQAB")));
    }
}
