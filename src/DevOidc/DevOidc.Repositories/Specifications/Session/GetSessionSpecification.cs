using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Specifications.Session
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
            RequestedScopes = JsonConvert.DeserializeObject<List<string>>(session.RequestedScopes ?? "[]") ?? new List<string>(),
            Audience = session.Audience
        };

        public Expression<Func<SessionEntity, bool>> Criteria => session => session.PartitionKey == _tenantId && session.RowKey == _sessionId;
    }
}
