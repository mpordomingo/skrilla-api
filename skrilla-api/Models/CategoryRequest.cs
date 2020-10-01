using System;
namespace skrilla_api.Models
{
    public class CategoryRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Nonedit { get; set; }

        public CategoryRequest(){ }
    }
}
