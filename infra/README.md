# Infrastructure

This folder contains a minimal Azure deployment for SessionHub using:

- Azure App Service for the API hosting
- Azure Application Insights for telemetry
- Azure SQL for persistent relational storage
- Bicep for infrastructure as code

## Files

- main.bicep — core infrastructure definition
- parameters/dev.bicepparam — sample deployment parameters

## Deployment

```bash
az group create --name rg-sessionhub-dev --location eastus
az deployment group create \
  --resource-group rg-sessionhub-dev \
  --template-file infra/main.bicep \
  --parameters infra/parameters/dev.bicepparam
```

## Notes

- Replace placeholder values before provisioning.
- Use managed identity or OIDC-based deployment for production automation.
- Store secrets in GitHub environments or Azure Key Vault, not in source control.
