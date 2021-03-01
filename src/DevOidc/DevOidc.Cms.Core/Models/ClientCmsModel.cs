using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevOidc.Core.Models.Dtos;
using GeneratedMapper.Attributes;
using RapidCMS.Core.Abstractions.Data;

namespace DevOidc.Cms.Core.Models
{
    [MapFrom(typeof(ClientDto))]
    [MapTo(typeof(ClientDto))]
    [IgnoreInTarget(nameof(ClientDto.TenantId))]
    public class ClientCmsModel : IEntity
    {
        [MapWith(nameof(ClientDto.ClientId), IgnoreNullIncompatibility = true)]
        [Required]
        [RegularExpression("^[A-Za-z0-9-_\\.]*$")]
        public string? Id { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        public string? ClientSecret { get; set; } = "";

        [Required]
        public List<string> RedirectUris { get; set; } = new List<string>();

        [Required]
        public List<ScopeDto> Scopes { get; set; } = new List<ScopeDto>();

        [Required]
        public Dictionary<string, string> AccessTokenExtraClaims { get; set; } = new Dictionary<string, string>();

        [Required]
        public Dictionary<string, string> IdTokenExtraClaims { get; set; } = new Dictionary<string, string>();
    }
}
