using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevOidc.Cms.Components.Editors;
using DevOidc.Core.Models;
using GeneratedMapper.Attributes;
using RapidCMS.Core.Abstractions.Data;

namespace DevOidc.Cms.Models
{
    [MapFrom(typeof(UserDto))]
    [MapTo(typeof(UserDto))]
    public class UserCmsModel : IEntity
    {
        [MapWith(nameof(UserDto.UserId))]
        public string Id { get; set; } = "";

        [Required]
        [EmailAddress]
        public string UserName { get; set; } = "";

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public Dictionary<string, string> ExtraClaims { get; set; } = new Dictionary<string, string>();
    }
}
