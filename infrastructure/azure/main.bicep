param location string = resourceGroup().location
param environment string = 'prod'
param sqlAdminUsername string
@secure()
param sqlAdminPassword string
param aksNodeCount int = 2
param aksNodeSize string = 'Standard_DS2_v2'

resource acr 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' = {
  name: 'expenseaiacr${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource kv 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: 'expenseaikv${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    sku: { family: 'A'; name: 'standard' }
    tenantId: subscription().tenantId
    accessPolicies: []
    enableSoftDelete: true
    enablePurgeProtection: true
  }
}

resource sql 'Microsoft.Sql/servers@2022-11-01' = {
  name: 'expenseaisql${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
  }
}

resource sqldb 'Microsoft.Sql/servers/databases@2022-11-01' = {
  parent: sql
  name: 'expenseaidb'
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    sampleName: 'AdventureWorksLT'
  }
  sku: {
    name: 'S0'
    tier: 'Standard'
    capacity: 10
  }
}

resource redis 'Microsoft.Cache/Redis@2023-08-01' = {
  name: 'expenseairedis${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Basic'
    family: 'C'
    capacity: 1
  }
  properties: {
    enableNonSslPort: false
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: 'expenseaistorage${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource aks 'Microsoft.ContainerService/managedClusters@2023-05-02-preview' = {
  name: 'expenseaiaks${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    dnsPrefix: 'expenseaiaks'
    agentPoolProfiles: [
      {
        name: 'nodepool1'
        count: aksNodeCount
        vmSize: aksNodeSize
        osType: 'Linux'
        mode: 'System'
      }
    ]
    linuxProfile: {
      adminUsername: 'azureuser'
      ssh: {
        publicKeys: [
          {
            keyData: 'ssh-rsa REPLACE_WITH_YOUR_PUBLIC_KEY'
          }
        ]
      }
    }
    enableRBAC: true
    networkProfile: {
      networkPlugin: 'azure'
      loadBalancerSku: 'standard'
    }
    addonProfiles: {}
    servicePrincipalProfile: {
      clientId: 'REPLACE_WITH_CLIENT_ID'
      secret: 'REPLACE_WITH_CLIENT_SECRET'
    }
  }
}

output acrLoginServer string = acr.properties.loginServer
output aksName string = aks.name
output sqlServerName string = sql.name
output sqlDbName string = sqldb.name
output redisHostName string = redis.properties.hostName
output storageAccountName string = storage.name
output keyVaultName string = kv.name
