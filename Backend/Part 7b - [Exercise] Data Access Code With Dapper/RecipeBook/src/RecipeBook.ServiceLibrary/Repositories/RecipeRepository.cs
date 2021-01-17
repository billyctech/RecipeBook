using Dapper;
using Microsoft.Extensions.Configuration;
using RecipeBook.ServiceLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.ServiceLibrary.Repositories
{
    public interface IRecipeRepository
    {
        Task<RecipeEntity> GetAsync(Guid id);

        Task<IEnumerable<RecipeEntity>> GetAsync();

        Task<int> InsertAsync(RecipeEntity entity);

        Task<int> DeleteAsync(Guid id);

        Task<int> UpdateAsync(RecipeEntity entity);
    }

    public class RecipeRepository : IRecipeRepository
    {
        private readonly string _connectionString;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IInstructionRepository _instructionRepository;

        public RecipeRepository(
            IConfiguration configuration,
            IIngredientRepository ingredientRepository,
            IInstructionRepository instructionRepository)
        {
            _connectionString = configuration.GetConnectionString("MainDatabase");

            _ingredientRepository = ingredientRepository;
            _instructionRepository = instructionRepository;
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            // Open Connection
            // Start Transaction
            // Delete ingredients by recipeId
            // Delete instructions by recipeId
            // Delete Recipes by recipeId
            // Commit transaction
            // Close Connections
            return await new RecipeRepository_Solution(_connectionString, _ingredientRepository, _instructionRepository).DeleteAsync(id);
        }

        public async Task<IEnumerable<RecipeEntity>> GetAsync()
        {
            // Open Connection
            // Declare database reader and Query database
            // Serialize RecipeEntity Object
            // Return
            // Close Connections
            return await new RecipeRepository_Solution(_connectionString, _ingredientRepository, _instructionRepository).GetAsync();
        }

        public async Task<RecipeEntity> GetAsync(Guid id)
        {
            // Open Connection
            // Declare database reader and QueryMutiple database
            // Serialize Objects (RecipeEntity, IngredientEntity, InstructionEntity)
            // Return serialized object
            // Close Connections
            return await new RecipeRepository_Solution(_connectionString, _ingredientRepository, _instructionRepository).GetAsync(id);
        }

        public async Task<int> InsertAsync(RecipeEntity entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var rowsAffected = await connection.ExecuteAsync(@"
                    INSERT INTO [dbo].[Recipes]
                                ([Id]
                                ,[Title]
                                ,[Description]
                                ,[Logo]
                                ,[CreatedDate])
                            VALUES
                                (@Id
                                ,@Title
                                ,@Description
                                ,@Logo
                                ,@CreatedDate)",
                                new
                                {
                                    entity.Id,
                                    entity.Title,
                                    entity.Description,
                                    entity.Logo,
                                    entity.CreatedDate
                                }, transaction: transaction);

                    rowsAffected += await _ingredientRepository.InsertAsync(connection, transaction, entity.Ingredients);

                    rowsAffected += await _instructionRepository.InsertAsync(connection, transaction, entity.Instructions);

                    transaction.Commit();

                    return rowsAffected;
                }
            }
        }

        public async Task<int> UpdateAsync(RecipeEntity entity)
        {
            // Open Connection
            // Start Transaction
            // Update Recipes
            // Delete ingredients by recipeId
            // Insert new Ingredents
            // Delete instructions by recipeId
            // Insert new instructions
            // Commit transaction
            // Close Connections
            return await new RecipeRepository_Solution(_connectionString, _ingredientRepository, _instructionRepository).UpdateAsync(entity);
        }
    }
}