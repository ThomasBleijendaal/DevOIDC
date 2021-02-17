using System;
using DevOidc.Core.Models.Dtos;
using GeneratedMapper.Attributes;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;

namespace DevOidc.Cms.Models
{
    [MapFrom(typeof(TenantDto))]
    [MapTo(typeof(TenantDto))]
    public class TenantCmsModel : IEntity
    {
        [MapWith(nameof(TenantDto.TenantId), IgnoreNullIncompatibility = true)]
        public string? Id { get; set; }
        public string Name { get; set; } = default!;
        public string OwnerName { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromMinutes(5);

        [Ignore]
        [JsonIgnore]
        public int TokenLifetimeSeconds {
            get => (int)TokenLifetime.TotalSeconds;
            set => TokenLifetime = TimeSpan.FromSeconds(value);
        }
    }
}
