using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace RecipeBook.ServiceLibrary.Repositories
{
    public class IngredientRepository_Solution
    {
        public static async Task<int> DeleteAsync(
            SqlConnection connection,
            DbTransaction transaction,
            Guid recipeId)
        {
            var rowsAffected = await connection.ExecuteAsync(@"
				DELETE FROM [dbo].[Ingredients]
				WHERE [RecipeId] = @RecipeId",
            new
            {
                RecipeId = recipeId,
            }, transaction: transaction);
            return rowsAffected;
        }
    }
}