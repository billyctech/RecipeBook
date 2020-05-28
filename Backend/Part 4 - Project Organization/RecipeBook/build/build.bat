cd ../src/RecipeBook.Api

dotnet restore
dotnet build --no-restore
dotnet publish -o ../../deploy