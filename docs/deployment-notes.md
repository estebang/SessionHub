# Deployment notes

## Prerequisites

- Azure subscription with permission to create resource groups and resources
- GitHub repository with Actions enabled
- Azure Entra ID app registration configured for workload identity / OIDC
- A resource group created for the target environment

## Required Azure configuration

1. Create an Entra app registration for GitHub Actions.
2. Add the federated credential for GitHub OIDC.
3. Grant Contributor access to the target resource group or subscription.
4. Set the repository secrets and variables listed in docs/azure-deployment.md.
5. Replace the placeholder values in infra/parameters/dev.bicepparam before deployment.

## Recommended release flow

- Publish to the main branch for automated deployment.
- Use a separate environment in GitHub for production gating if needed.
- Require successful CodeQL and test jobs before infrastructure deployment.

## Rollback guidance

- Keep the previous build artifact available in the workflow run artifacts or a release store.
- Roll back the app package first for runtime issues.
- Roll back the infrastructure only when a deployment introduced resource drift or configuration regressions.
- Validate the database and app service health endpoints before reopening traffic.
