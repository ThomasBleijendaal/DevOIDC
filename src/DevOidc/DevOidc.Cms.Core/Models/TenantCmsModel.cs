using System;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [RegularExpression("^[A-Za-z0-9-_\\.]*$")]
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string OwnerName { get; set; } = default!;

        [Required]
        public string Description { get; set; } = string.Empty;

        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromMinutes(5);

        [Ignore]
        [JsonIgnore]
        [Required]
        [Range(5, int.MaxValue)]
        public int TokenLifetimeSeconds {
            get => (int)TokenLifetime.TotalSeconds;
            set => TokenLifetime = TimeSpan.FromSeconds(value);
        }
    }
}
