using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevOidc.Cms.Components.Editors;
using DevOidc.Core.Models;
using GeneratedMapper.Attributes;
using RapidCMS.Core.Abstractions.Data;

namespace DevOidc.Cms.Models
{
    [MapFrom(typeof(ClientDto))]
    [MapTo(typeof(ClientDto))]
    [IgnoreInTarget(nameof(ClientDto.TenantId))]
    public class ClientCmsModel : IEntity
    {
        [MapWith(nameof(ClientDto.ClientId), IgnoreNullIncompatibility = true)]
        public string? Id { get; set; } = "";

        [Display(ShortName = "Name", Name = "Name", Order = 0)]
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public List<string> RedirectUris { get; set; } = new List<string>();

        [Required]
        public List<ScopeDto> Scopes { get; set; } = new List<ScopeDto>();

        [Display(ShortName = "Extra claims (access token)", Name = "Extra claims (access token)", ResourceType = typeof(ClaimEditor), Order = 0)]
        [Required]
        public Dictionary<string, string> AccessTokenExtraClaims { get; set; } = new Dictionary<string, string>();

        [Display(ShortName = "Extra claims (id token)", Name = "Extra claims (id token)", ResourceType = typeof(ClaimEditor), Order = 0)]
        [Required]
        public Dictionary<string, string> IdTokenExtraClaims { get; set; } = new Dictionary<string, string>();
    }
}
