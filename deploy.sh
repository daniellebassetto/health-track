#!/bin/bash
set -e

echo "🚀 Iniciando deploy do HealthTrack..."

if [ ! -f .env ]; then
    echo "⚠️  Arquivo .env não encontrado. Copiando .env.example..."
    cp .env.example .env
    echo "⚠️  Configure o arquivo .env antes de continuar!"
    exit 1
fi

source .env

echo "🐳 Parando containers existentes..."
docker-compose down

echo "🔨 Construindo imagens..."
docker-compose build --no-cache

echo "▶️  Iniciando containers..."
docker-compose up -d

echo "⏳ Aguardando MySQL inicializar..."
sleep 15

echo "📊 Aplicando migrations..."
docker-compose exec -T app dotnet ef database update || echo "⚠️  Migrations já aplicadas ou erro"

echo "✅ Deploy concluído!"
echo "🌐 Aplicação disponível em: http://localhost:8080"
