using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using Xunit;

namespace RecipeBook.ServiceLibrary.Tests.Repositories
{
    public class RecipeRepositoryTests_Solution
    {
        private readonly DatabaseFixture _databaseFixture;

        public RecipeRepositoryTests_Solution(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        public RecipeRepository Setup()
        {
            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .SetupGet(m => m[It.Is<string>(s => s == "MainDatabase")])
                .Returns("Data Source=host.docker.internal,5050; Initial Catalog=RecipeBook;User Id=sa;Password=P@ssword123;MultipleActiveResultSets=true");

            var configurationMock = new Mock<IConfiguration>();
            configurationMock
                .Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings")))
                .Returns(configurationSectionMock.Object);

            var recipeRepository = new RecipeRepository(configurationMock.Object, new IngredientRepository(), new InstructionRepository());
            return recipeRepository;
        }

        public async Task GetAsyncWithId_Success()
        {
            // Arrange
            var recipeRepository = Setup();

            // Act
            var recipe = await recipeRepository.GetAsync(_databaseFixture.RecipeId1);

            // Assert
            Assert.NotNull(recipe);
            Assert.NotNull(recipe.Ingredients);
            Assert.NotNull(recipe.Instructions);
            Assert.Equal(_databaseFixture.RecipeId1, recipe.Id);
        }

        public async Task GetAsync_Success()
        {
            // Arrange
            var recipeRepository = Setup();

            // Act
            var recipes = await recipeRepository.GetAsync();

            // Assert
            Assert.NotNull(recipes);
            Assert.True(recipes.Count() >= 1);
        }

        public async Task UpdateAsync_Success()
        {
            // Arrange
            var recipeRepository = Setup();

            var recipe = new RecipeEntity()
            {
                Id = _databaseFixture.RecipeId1,
                Title = "Title Something new",
                Description = "Description New",
                Logo = "Something",
                Ingredients = new List<IngredientEntity>()
                {
                    new IngredientEntity()
                    {
                        RecipeId = _databaseFixture.RecipeId1,
                        OrdinalPosition = 0,
                        Unit = "lbs",
                        Quantity = 1,
                        Ingredient = "Chicken"
                    },
                   new IngredientEntity()
                    {
                        RecipeId = _databaseFixture.RecipeId1,
                        OrdinalPosition = 1,
                        Unit = "lbs",
                        Quantity = 1,
                        Ingredient = "Chicken"
                    }
                },
                Instructions = null
            };

            // Act
            var rowsAffected = await recipeRepository.UpdateAsync(recipe);

            // Assert
            Assert.Equal(3, rowsAffected);

            // Verify properties are updated
            var recipeFromDatabase = await recipeRepository.GetAsync(_databaseFixture.RecipeId1);

            Assert.Equal(recipe.Title, recipeFromDatabase.Title);
            Assert.Equal(recipe.Description, recipeFromDatabase.Description);
            Assert.Equal(recipe.Logo, recipeFromDatabase.Logo);
            Assert.Equal(recipe.Ingredients.Count(), recipeFromDatabase.Ingredients.Count());
            Assert.Equal(0, recipeFromDatabase.Instructions?.Count());
        }

        public async Task DeleteAsync_Success()
        {
            // Arrange
            var recipeRepository = Setup();

            // Act
            var rowsAffected = await recipeRepository.DeleteAsync(_databaseFixture.RecipeId1);

            // Assert
            Assert.Equal(3, rowsAffected);
        }
    }
}