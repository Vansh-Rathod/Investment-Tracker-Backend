using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public class UserSettingDto
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
