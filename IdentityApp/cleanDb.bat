dotnet ef database drop --force --context ProductDbContext
dotnet ef database drop --force --context IdentityAppIdentityDbContext
dotnet ef database update --context ProductDbContext 
dotnet ef database update --context IdentityAppIdentityDbContext 
