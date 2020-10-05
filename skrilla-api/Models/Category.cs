using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace skrilla_api.Models
{
    [Serializable]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Nonedit { get; set; }

        [Required]
        public string PersonId { get; set; }

        public Category(string name,  bool nonedit, string personId)
        {
            this.Name = name;
            this.Nonedit = nonedit;
            this.PersonId = personId;
        }

        public Category()
        {
        }
    }
}
