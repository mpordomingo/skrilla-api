using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NodaTime;
namespace skrilla_api.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Photoid { get; set; }

        [Required]
        public int Nonedit { get; set; }

        public Category(string name, int photoid, int nonedit)
        {
            this.Name = name;
            this.Photoid = photoid;
            this.Nonedit = nonedit;
        }
    }
}
