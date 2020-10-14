using System;
using System.Collections.Generic;
using skrilla_api.Models;

namespace skrilla_api.Services
{
    public interface ICategoriesService
    {

        public bool ModifyCategory(CategoryRequest categoryRequest, int id);
        public bool DeleteCategory(int id);
        public Category CreateCategory(CategoryRequest request);
        public List<Category> GetCategories();

    }
}
