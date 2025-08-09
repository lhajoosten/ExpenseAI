# Deployment Guide

ExpenseAI supports local, Docker, and Azure cloud deployments.

## Local Development

1. **Backend**
   ```bash
   cd src/backend
   dotnet restore
   dotnet build
   dotnet ef database update --project ExpenseAI.Infrastructure
   dotnet run --project ExpenseAI.Api
   ```

2. **Frontend**
   ```bash
   cd src/frontend
   npm install
   ng serve
   ```

## Docker

- **Development**
  ```bash
  docker-compose -f docker/docker-compose.dev.yml up
  ```

- **Production**
  ```bash
  docker-compose -f docker/docker-compose.prod.yml up -d
  ```

## Azure Cloud

1. **Provision Resources**
   ```bash
   az deployment group create \
     --resource-group expense-ai-rg \
     --template-file azure/main.bicep
   ```

2. **Build & Push Containers**
   ```bash
   docker build -t expenseai-api:latest -f docker/Dockerfile.api .
   docker build -t expenseai-web:latest -f docker/Dockerfile.web .
   ```

3. **Deploy Containers**
   ```bash
   az container create --resource-group expense-ai-rg \
     --name expenseai-api --image expenseai-api:latest
   ```

4. **Configure Environment**
   - Set connection strings, API keys, and secrets via Azure Key Vault or environment variables.

## CI/CD

- **GitHub Actions**: Automated build, test, and deployment pipelines.
- **Azure DevOps**: Optional for advanced workflows.
- **Health Checks**: Enabled for monitoring.

## Configuration

- Use `appsettings.{Environment}.json` for environment-specific settings.
- Secrets should never be committed; use environment variables or Azure Key Vault.

---

For troubleshooting and advanced deployment, see the [Wiki](https://github.com/lhajoosten/expense-ai-platform/wiki).
