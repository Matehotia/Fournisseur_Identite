# Vérifier d'abord la version de docker et docker-compose
docker --version
docker-compose --version

# Les images utilisées: mcr.microsoft.com/dotnet/sdk:8.0 et postgres:14.10

# La structure de l' application:
#   -DB/ : contient les scripts pour la base et la conception MCD
#   -api/ : contient l' API et le Dockerfile
#   -todo.xlsx : contient la distribution des tâches par l'équipe
#   -docker-compose.yml : contient les instructions pour le démarrage

# Lien vers le swaggerUI: http://localhost:5000/swagger

# Lancement et déploiment de l'application dans Docker
# Ceci va prendre un peu quelques temps
docker-compose up --build
