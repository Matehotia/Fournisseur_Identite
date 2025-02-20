FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . ./
# Restaurer les dépendances
RUN dotnet restore
# Compiler l'application en mode Release
RUN dotnet publish -c Release -o out

# Utiliser une image runtime pour exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
# Copier les fichiers compilés de l'étape précédente
COPY --from=build /app/out .

# Exposer le port utilisé par votre application
EXPOSE 5000

# Démarrer l'application
ENTRYPOINT ["dotnet", "api.dll"]
