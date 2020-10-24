using System;
using skrilla_api.Models;
using Xunit;

namespace SkrillaApi.Tests.Tests
{
    public class CategoryTest
    {
        public CategoryTest()
        {
        }

        [Fact]
        public void CategoryCreatedSuccessfully(){
            Category category = new Category("Example", false, "exampleUser","exampleIcon");

            Assert.False(category.Nonedit);
            Assert.Equal("Example", category.Name);
            Assert.Equal("exampleUser", category.PersonId);
            Assert.Equal("exampleIcon", category.IconDescriptor);
        }
    }
}
