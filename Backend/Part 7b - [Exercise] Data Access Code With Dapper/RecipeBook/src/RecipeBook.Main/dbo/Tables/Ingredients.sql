CREATE TABLE [dbo].[Ingredients]
(
	[RecipeId] UNIQUEIDENTIFIER NOT NULL,
	[OrdinalPosition] INT NOT NULL,
	[Unit] NVARCHAR(25) NOT NULL,
	[Quantity] float NOT NULL,
	[Ingredient] NVARCHAR(50) NOT NULL,
	CONSTRAINT [FK_Ingredients_Recipes_Id] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [PK_Ingredients] PRIMARY KEY ([RecipeId], [OrdinalPosition]),
)