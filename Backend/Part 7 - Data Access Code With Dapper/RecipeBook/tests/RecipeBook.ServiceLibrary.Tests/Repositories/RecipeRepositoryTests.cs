using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using Xunit;

namespace RecipeBook.ServiceLibrary.Tests.Repositories
{
    public class RecipeRepositoryTests: IClassFixture<DatabaseFixture>
    {
        private bool _commitToDatabase = false;
        private readonly DatabaseFixture _databaseFixture;

        public RecipeRepositoryTests(DatabaseFixture databaseFixture)
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

        [Fact]
        [Trait("Category", "Database")]
        public async Task GetAsyncWithId_Success()
        {
            var recipeRepository = Setup();

            var recipe = await recipeRepository.GetAsync(_databaseFixture.RecipeId1);

            Assert.NotNull(recipe);
            Assert.NotNull(recipe.Ingredients);
            Assert.NotNull(recipe.Instructions);
            Assert.Equal(_databaseFixture.RecipeId1, recipe.Id);
        }

        [Fact]
        [Trait("Category", "Database")]
        public async Task GetAsync_Success()
        {
            var recipeRepository = Setup();

            var recipes = await recipeRepository.GetAsync();

            Assert.NotNull(recipes);
            Assert.True(recipes.Count() >= 1);
        }

        [Fact]
        [Trait("Category", "Database")]
        public async Task InsertAsync_Success()
        {
            var recipeRepository = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var recipeId = Guid.NewGuid();
                var rowsAffected = await recipeRepository.InsertAsync(new RecipeEntity()
                {
                    Id = recipeId,
                    Title = "Fried Chicken Unit Test",
                    Description = "Fried Chicken Description",
                    Logo = null,
                    CreatedDate = DateTimeOffset.UtcNow,
                    Ingredients = new List<IngredientEntity>()
                    {
                        new IngredientEntity()
                        {
                            RecipeId = recipeId,
                            OrdinalPosition = 0,
                            Unit = "lbs",
                            Quantity = 1,
                            Ingredient = "Chicken"
                        }
                    },
                    Instructions = new List<InstructionEntity>()
                    {
                        new InstructionEntity()
                        {
                            RecipeId = recipeId,
                            OrdinalPosition = 0,
                            Instruction = "Cook it"
                        }
                    }
                });

                if (_commitToDatabase)
                {
                    scope.Complete();
                }

                Assert.Equal(3, rowsAffected);
            }
        }

        [Fact]
        [Trait("Category", "Database")]
        public async Task UpdateAsync_Success()
        {
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

            var rowsAffected = await recipeRepository.UpdateAsync(recipe);

            Assert.Equal(3, rowsAffected);

            var recipeFromDatabase = await recipeRepository.GetAsync(_databaseFixture.RecipeId1);

            Assert.Equal(recipe.Title, recipeFromDatabase.Title);
            Assert.Equal(recipe.Description, recipeFromDatabase.Description);
            Assert.Equal(recipe.Logo, recipeFromDatabase.Logo);
            Assert.Equal(recipe.Ingredients.Count(), recipeFromDatabase.Ingredients.Count());
            Assert.Equal(0, recipeFromDatabase.Instructions?.Count());
        }

        [Fact]
        [Trait("Category", "Database")]
        public async Task DeleteAsync_Success()
        {
            var recipeRepository = Setup();

            var rowsAffected = await recipeRepository.DeleteAsync(_databaseFixture.RecipeId1);

            Assert.Equal(3, rowsAffected);
        }
    }
}
