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
        [MapWith(nameof(ClientDto.ClientId))]
        public string Id { get; set; } = "";

        [Display(ShortName = "Name", Name = "Name", Order = 0)]
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public List<string> RedirectUris { get; set; } = new List<string>();

        [Required]
        public List<ScopeDto> Scopes { get; set; } = new List<ScopeDto>();

        [Display(ShortName = "Extra claims", Name = "Extra claims", ResourceType = typeof(ClaimEditor), Order = 0)]
        [Required]
        public Dictionary<string, string> ExtraClaims { get; set; } = new Dictionary<string, string>();
    }
}
