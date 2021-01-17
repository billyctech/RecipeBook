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
    public class RecipeRepository_Solution
    {
        private readonly string _connectionString;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IInstructionRepository _instructionRepository;

        public RecipeRepository_Solution(
            string connectionString,
            IIngredientRepository ingredientRepository,
            IInstructionRepository instructionRepository)
        {
            _connectionString = connectionString;

            _ingredientRepository = ingredientRepository;
            _instructionRepository = instructionRepository;
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var rowsAffected = 0;

                    rowsAffected += await _ingredientRepository.DeleteAsync
                    (
                        connection,
                        transaction,
                        id
                    );

                    rowsAffected += await _instructionRepository.DeleteAsync
                    (
                        connection,
                        transaction,
                        id
                    );

                    rowsAffected += await connection.ExecuteAsync(@"
				    DELETE FROM [dbo].[Recipes]
				    WHERE [Id] = @Id",
                    new
                    {
                        Id = id,
                    }, transaction: transaction);
                    return rowsAffected;
                }
            }
        }

        public async Task<IEnumerable<RecipeEntity>> GetAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<RecipeEntity>(@"
				SELECT *
				FROM [Recipes]");
                return result;
            }
        }

        public async Task<RecipeEntity> GetAsync(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var reader = await connection.QueryMultipleAsync(@"
				    SELECT *
				    FROM [Recipes]
				    WHERE [Id] = @RecipeId

				    SELECT *
				    FROM [Ingredients]
				    WHERE [RecipeId] = @RecipeId

				    SELECT *
				    FROM [Instructions]
				    WHERE [RecipeId] = @RecipeId",
                    new
                    {
                        RecipeId = id
                    }))
                {
                    var recipe = await reader.ReadSingleAsync<RecipeEntity>();
                    recipe.Ingredients = (await reader.ReadAsync<IngredientEntity>())?.ToList();
                    recipe.Instructions = (await reader.ReadAsync<InstructionEntity>())?.ToList();
                    return recipe;
                }
            }
        }

        public async Task<int> UpdateAsync(RecipeEntity entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var rowsAffected = await connection.ExecuteAsync(@"
					UPDATE [dbo].[Recipes]
					   SET [Title] = @Title
						  ,[Description] = @Description
						  ,[Logo] = @Logo
					 WHERE [Id] = @Id",
                    new
                    {
                        entity.Id,
                        entity.Title,
                        entity.Description,
                        entity.Logo
                    }, transaction: transaction);

                    await _ingredientRepository.DeleteAsync(connection, transaction, entity.Id);
                    rowsAffected += await _ingredientRepository.InsertAsync(connection, transaction, entity.Ingredients);

                    await _instructionRepository.DeleteAsync(connection, transaction, entity.Id);
                    rowsAffected += await _instructionRepository.InsertAsync(connection, transaction, entity.Instructions);

                    transaction.Commit();

                    return rowsAffected;
                }
            }
        }
    }
}