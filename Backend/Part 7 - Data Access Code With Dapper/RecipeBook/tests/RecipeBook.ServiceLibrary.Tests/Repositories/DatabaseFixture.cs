using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace RecipeBook.ServiceLibrary.Tests.Repositories
{
    public class DatabaseFixture : IDisposable
    {
        public SqlConnection Connection { get; private set; }
        public Guid RecipeId1 = new Guid("1752073E-909A-4A68-9399-5C97E1129E6E");

        public DatabaseFixture()
        {
            Connection = new SqlConnection("Data Source=host.docker.internal,5050; Initial Catalog=RecipeBook;User Id=sa;Password=P@ssword123;MultipleActiveResultSets=true");
            SeedData();
        }

        public void Dispose()
        {
            DeleteData();
            Connection.Close();
        }


        private void SeedData()
        {
            DeleteData();

            Connection.Execute(@"
BEGIN TRANSACTION
	INSERT INTO [dbo].[Recipes]
          ([Id]
          ,[Title]
          ,[Description]
          ,[Logo]
          ,[CreatedDate])
    VALUES
          (@RecipeId
          ,'Fried Chicken Seeded'
          ,'Fried Chicken Seeded'
          ,null
          ,GETUTCDATE())

  INSERT INTO [dbo].[Ingredients]
              ([RecipeId]
              ,[OrdinalPosition]
              ,[Unit]
              ,[Quantity]
              ,[Ingredient])
        VALUES
              (@RecipeId
              ,0
              ,'lbs seeded'
              ,1
              ,'Chicken seeded')

  INSERT INTO [dbo].[Instructions]
              ([RecipeId]
              ,[OrdinalPosition]
              ,[Instruction])
        VALUES
              (@RecipeId
              ,0
              ,'Cook it Seeded')
COMMIT TRANSACTION
      ", new { RecipeId = RecipeId1 });
        }

        private void DeleteData()
        {
            Connection.Execute(@"
BEGIN TRANSACTION
  DELETE FROM [dbo].[Ingredients] WHERE [RecipeId] = @RecipeId
  DELETE FROM [dbo].[Instructions] WHERE [RecipeId] = @RecipeId
  DELETE FROM [dbo].[Recipes] WHERE [Id] = @RecipeId
COMMIT TRANSACTION", new { RecipeId = RecipeId1 });
        }
    }
}
