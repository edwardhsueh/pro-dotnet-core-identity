#!/bin/bash
echo '#### 1'
dotnet ef database drop --force --context IdentityAppIdentityDbContext
echo '#### 2'
dotnet ef database update 0 --context IdentityAppIdentityDbContext  
echo '#### 3'
dotnet ef migrations remove  --context IdentityAppIdentityDbContext  
echo '#### 4'
dotnet ef migrations add CreateIdentitySchema --context IdentityAppIdentityDbContext  
echo '#### 5'
dotnet ef database update --context IdentityAppIdentityDbContext 
echo '#######################'
echo '#### 6'
echo '#######################'
dotnet ef database drop --force --context ProductDbContext
echo '#### 7'
dotnet ef database update 0 --context ProductDbContext 
echo '#### 8'
dotnet ef migrations remove  --context ProductDbContext  
echo '#### 9'
dotnet ef migrations add CreateProductionSchema --context ProductDbContext 
echo '#### 10'
dotnet ef database update --context ProductDbContext 
