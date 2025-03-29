#!/bin/bash

# Configurations
CONTAINER_NAME="myapp-mysql"
MYSQL_ROOT_PASSWORD="rootpass"
MYSQL_DATABASE="MyAppDb"
MYSQL_USER="myuser"
MYSQL_PASSWORD="mypass"
MYSQL_PORT="3306"

MIGRATION_NAME="InitialCreate"

# 1. Check if the container already exists
if [ "$(docker ps -aq -f name=$CONTAINER_NAME)" ]; then
    echo "🚢 Container $CONTAINER_NAME already exists."
    docker start $CONTAINER_NAME
else
    echo "🚀 Starting a new MySQL container: $CONTAINER_NAME"
    docker run -d \
        --name $CONTAINER_NAME \
        -e MYSQL_ROOT_PASSWORD=$MYSQL_ROOT_PASSWORD \
        -e MYSQL_DATABASE=$MYSQL_DATABASE \
        -e MYSQL_USER=$MYSQL_USER \
        -e MYSQL_PASSWORD=$MYSQL_PASSWORD \
        -p $MYSQL_PORT:3306 \
        mysql:8.0
fi

# 2. Wait for MySQL to become available
echo "⏳ Waiting for MySQL to be ready..."
until docker exec $CONTAINER_NAME mysqladmin --user=$MYSQL_USER --password=$MYSQL_PASSWORD ping --silent &> /dev/null ; do
    sleep 2
done
echo "✅ MySQL is ready!"

# 3. Set environment for .NET
export DOTNET_ENVIRONMENT=Development

# 4. Create migration if it doesn't already exist
if [ ! -f "./Migrations/${MIGRATION_NAME}.cs" ] && [ ! -d "./Migrations" ]; then
  echo "📦 Creating migration: $MIGRATION_NAME"
  dotnet ef migrations add $MIGRATION_NAME
else
  echo "📁 Migration '$MIGRATION_NAME' already exists. Skipping..."
fi

# 5. Apply migration
echo "⚙️ Applying migration to MySQL..."
dotnet ef database update

echo "🎉 Database created and migrations applied successfully!"
