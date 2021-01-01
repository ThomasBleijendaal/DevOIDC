using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Specifications
{
    public class GetSessionSpecification : ISpecification<SessionEntity, StoredSessionDto>
    {
        private readonly string _tenantId;
        private readonly string _sessionId;

        public GetSessionSpecification(string tenantId, string sessionId)
        {
            _tenantId = tenantId;
            _sessionId = sessionId;
        }

        public Func<SessionEntity, StoredSessionDto> Projection => session => new StoredSessionDto
        {
            ClientId = session.ClientId ?? "",
            ScopeId = session.ScopeId ?? "",
            TenantId = session.PartitionKey,
            UserId = session.UserId ?? "",
            RequestedScopes = session.RequestedScopes ?? ""
        };

        public Expression<Func<SessionEntity, bool>> Criteria => session => session.PartitionKey == _tenantId && session.RowKey == _sessionId;
    }
}
