using System;
using DevOidc.Core.Models;
using GeneratedMapper.Attributes;
using RapidCMS.Core.Abstractions.Data;

namespace DevOidc.Cms.Models
{
    [MapFrom(typeof(TenantDto))]
    public class TenantCmsModel : IEntity
    {
        [MapWith(nameof(TenantDto.TenantId))]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? OwnerName { get; set; }
        public string? Description { get; set; }
        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromMinutes(5);
    }
}
