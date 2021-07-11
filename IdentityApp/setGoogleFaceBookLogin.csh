#!/bin/bash
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "950792777676-h30pdra7fil29jtsl5l9n0sjvrnhkgdb.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Facebook:AppSecret" "6b8a19035e47978765f71657015840b6"
dotnet user-secrets set "Authentication:Facebook:AppId" "2979913789001540"
dotnet user-secrets set "Authentication:Facebook:AppSecret" "6b8a19035e47978765f71657015840b6"