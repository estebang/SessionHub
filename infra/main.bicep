@description('The location for all Azure resources.')
param location string = resourceGroup().location

@description('The environment name, such as dev or prod.')
param environmentName string

@description('The name of the application service plan.')
param appServicePlanName string = 'asp-${environmentName}-${uniqueString(resourceGroup().id)}'

@description('The name of the App Service app.')
param appServiceName string = 'app-sessionhub-prod'

@description('The application insights name.')
param applicationInsightsName string = 'appi-${environmentName}-${uniqueString(resourceGroup().id)}'

@description('The Azure SQL logical server name.')
param sqlServerName string = 'sqlsrv-${environmentName}-${uniqueString(resourceGroup().id)}'

@description('The Azure SQL database name.')
param sqlDatabaseName string = 'sessionhub'

@description('The SQL administrator login name.')
param sqlAdministratorLogin string

@secure()
@description('The SQL administrator password.')
param sqlAdministratorPassword string

@description('The Azure AD tenant ID used for OIDC deployment authentication.')
param tenantId string

var appServiceSkuName = 'B1'
var appServiceSkuTier = 'Basic'

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'law-${environmentName}-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServiceSkuName
    tier: appServiceSkuTier
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: appServiceName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|9.0'
      minTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environmentName
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'AzureSql'
          value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${sqlDatabase.name};User ID=${sqlAdministratorLogin};Password=${sqlAdministratorPassword};Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;'
        }
      ]
    }
  }
}

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorPassword
    version: '12.0'
    publicNetworkAccess: 'Enabled'
    minimalTlsVersion: '1.2'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  name: sqlDatabaseName
  parent: sqlServer
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
  }
}

resource firewallRules 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'AllowAzureServices'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output appServiceName string = appService.name
output appServiceUrl string = 'https://${appService.properties.defaultHostName}'
output applicationInsightsConnectionString string = appInsights.properties.ConnectionString
output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name
output tenantId string = tenantId
