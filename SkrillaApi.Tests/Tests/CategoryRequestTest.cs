using System;
using skrilla_api.Models;
using Xunit;

namespace SkrillaApi.Tests.Tests
{
    public class CategoryRequestTest
    {
        public CategoryRequestTest()
        {
        }

        [Fact]
        public void CategoryRequestCreatedSuccesfully()
        {
            CategoryRequest request = new CategoryRequest();
            request.Name = null;
            request.Nonedit = true;
            request.Icon = "exampleIcon";
            Assert.Equal("exampleIcon", request.Icon);
            Assert.True(request.Nonedit);
            Assert.Null(request.Name);
            request.Name = "Example";
            Assert.Equal("Example", request.Name);
        }

    }
}
