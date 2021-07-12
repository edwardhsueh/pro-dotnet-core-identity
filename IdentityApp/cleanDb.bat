Write-Host '#### 1'
dotnet ef database drop --force --context IdentityAppIdentityDbContext
Write-Host '#### 2'
dotnet ef database update 0 --context IdentityAppIdentityDbContext  
Write-Host '#### 3'
dotnet ef migrations remove  --context IdentityAppIdentityDbContext  
Write-Host '#### 4'
dotnet ef migrations add CreateIdentitySchema --context IdentityAppIdentityDbContext  
Write-Host '#### 5'
dotnet ef database update --context IdentityAppIdentityDbContext 
Write-Host '#######################'
Write-Host '#### 6'
Write-Host '#######################'
dotnet ef database drop --force --context ProductDbContext
Write-Host '#### 7'
dotnet ef database update 0 --context ProductDbContext 
Write-Host '#### 8'
dotnet ef migrations remove  --context ProductDbContext  
Write-Host '#### 9'
dotnet ef migrations add CreateProductionSchema --context ProductDbContext 
Write-Host '#### 10'
dotnet ef database update --context ProductDbContext 
