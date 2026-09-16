# Azure deployment guide

## Architecture

SessionHub is deployed as a lightweight Azure-hosted web application with a split between frontend and API hosting:

- Azure App Service hosts the ASP.NET Core API.
- Azure Application Insights captures telemetry, requests, failures, and dependency traces.
- Azure SQL Database replaces the local SQLite database for persistent data storage in Azure.
- GitHub Actions orchestrates build, test, security scanning, infrastructure provisioning, and deployment using OIDC.

This architecture keeps deployment simple while still reflecting a realistic production setup for a small business application.

## Deployment flow

1. A GitHub push to the main branch triggers the deployment workflow.
2. The workflow restores the .NET solution, builds the app, runs tests, and builds the frontend bundle.
3. CodeQL scans the repository for security issues.
4. The workflow authenticates to Azure using OIDC federation instead of a stored password.
5. Bicep deploys the Azure App Service, Application Insights, and Azure SQL resources.
6. The API is published and deployed to the App Service.
7. The frontend static bundle is uploaded to the configured hosting target or served through the app platform as needed.

## Environment variables

The following environment settings are expected for the Azure deployment:

### GitHub repository variables
- AZURE_RESOURCE_GROUP
- AZURE_LOCATION
- ENVIRONMENT_NAME
- APP_SERVICE_NAME
- WEB_STORAGE_CONTAINER
- WEB_STORAGE_ACCOUNT

### GitHub repository secrets
- AZURE_CLIENT_ID
- AZURE_TENANT_ID
- AZURE_SUBSCRIPTION_ID
- SQL_ADMIN_LOGIN
- SQL_ADMIN_PASSWORD

### App Service configuration
The infrastructure template sets the following runtime settings:

- APPLICATIONINSIGHTS_CONNECTION_STRING
- ASPNETCORE_ENVIRONMENT
- ConnectionStrings__AzureSql

The AzureSql connection string should be available as an application setting on the App Service (using the `ConnectionStrings__AzureSql` key so ASP.NET Core configuration binds it to `ConnectionStrings:AzureSql`) and should point to the Azure SQL database created in the Bicep deployment.

## Rollback process

Rollback is intentionally simple for this small deployment model:

1. Identify the last successful release or deployment artifact.
2. Redeploy the prior app package to the App Service using the same workflow or Azure CLI.
3. If the infrastructure changed and is causing the issue, redeploy the previous Bicep template revision.
4. Validate the health endpoint after rollback.
5. If database structure changes were introduced, restore from the most recent backup before reapplying the previous app version.

Recommended operational practice:

- keep deployment artifacts versioned,
- keep a recent Azure SQL backup,
- verify /healthz after every deployment,
- avoid destructive schema changes without a backup plan.

## Notes

- The app is intentionally scoped for a small demo production deployment, not a large enterprise environment.
- OIDC is used for all Azure auth so no stored Azure password is required in GitHub.
- Terraform is not used here; Bicep is the infrastructure-as-code choice for Azure deployment.
