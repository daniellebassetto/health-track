# Deploy com Docker

## Pré-requisitos
- Docker e Docker Compose instalados na VM Linux
- Portas 8080 e 3306 disponíveis

## Deploy Manual

1. Clone o repositório na VM:
```bash
git clone <seu-repositorio>
cd HealthTrack
```

2. Configure as variáveis de ambiente:
```bash
cp .env.example .env
nano .env  # Edite com suas credenciais
```

3. Execute o script de deploy:
```bash
chmod +x deploy.sh
./deploy.sh
```

## Deploy Automático (GitHub Actions)

Configure os seguintes secrets no GitHub:
- `VM_HOST`: IP ou hostname da VM
- `VM_USER`: Usuário SSH
- `VM_SSH_KEY`: Chave privada SSH
- `MYSQL_ROOT_PASSWORD`: Senha do MySQL
- `GEMINI_API_KEY`: Chave da API Gemini

O deploy será executado automaticamente a cada push na branch main.

## Comandos Úteis

```bash
# Ver logs
docker-compose logs -f app

# Parar aplicação
docker-compose down

# Reiniciar
docker-compose restart

# Aplicar migrations
docker-compose exec app dotnet ef database update
```
