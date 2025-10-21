. "$PSScriptRoot\variables.ps1"

az login --tenant $TENANT_ID

# Resource Group (idempotent)
Write-Host "Creating Resource Group $RESOURCE_GROUP."
az group create --name $RESOURCE_GROUP --location $LOCATION

# Azure Container Registry
$acrExists = az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query name --output tsv 2>$null
if ([string]::IsNullOrEmpty($acrExists)) {
    Write-Host "Creating ACR $ACR_NAME."
    az acr create --name $ACR_NAME --resource-group $RESOURCE_GROUP --location $LOCATION --sku Basic --admin-enabled true
    Write-Host "ACR $ACR_NAME created."

} else {
    Write-Host "ACR $ACR_NAME already exists."
}

# Log Analytics Workspace
$LOG_ANALYTICS_WORKSPACE_ID = az monitor log-analytics workspace show --resource-group $RESOURCE_GROUP --workspace-name $LOG_ANALYTICS_NAME --query customerId -o tsv 2>$null

if (-not $LOG_ANALYTICS_WORKSPACE_ID) {
    Write-Host "Creating Log Analytics Workspace $LOG_ANALYTICS_NAME."
    az monitor log-analytics workspace create --resource-group $RESOURCE_GROUP --workspace-name $LOG_ANALYTICS_NAME --location $LOCATION
    $LOG_ANALYTICS_WORKSPACE_ID = az monitor log-analytics workspace show --resource-group $RESOURCE_GROUP --workspace-name $LOG_ANALYTICS_NAME --query customerId -o tsv
} else {
    Write-Host "Log Analytics Workspace $LOG_ANALYTICS_NAME already exists."
}

# App Insights
$APP_INSIGHTS_EXISTS = az monitor app-insights component show --app $APP_INSIGHTS_NAME --resource-group $RESOURCE_GROUP --query name --output tsv 2>$null

if (-not $APP_INSIGHTS_EXISTS) {
    Write-Host "Creating Application Insights $APP_INSIGHTS_NAME."
    az monitor app-insights component create `
        --app $APP_INSIGHTS_NAME `
        --location $LOCATION `
        --resource-group $RESOURCE_GROUP `
        --workspace $LOG_ANALYTICS_NAME
} else {
    Write-Host "Application Insights $APP_INSIGHTS_NAME already exists."
}

# Get App Insights connection string
$APP_INSIGHTS_CONNECTION_STRING = az monitor app-insights component show `
  --app $APP_INSIGHTS_NAME `
  --resource-group $RESOURCE_GROUP `
  --query connectionString `
  --output tsv

# Container Apps Environment
$envExists = az containerapp env show --name $ENV_NAME --resource-group $RESOURCE_GROUP --query name --output tsv 2>$null
if ([string]::IsNullOrEmpty($envExists)) {
    Write-Host "Creating Container Apps Environment $ENV_NAME."
    az containerapp env create --name $ENV_NAME --resource-group $RESOURCE_GROUP --location $LOCATION --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_ID
    Write-Host "Container Apps Environment $ENV_NAME created."
} else {
    Write-Host "Container Apps Environment $ENV_NAME already exists. Updating existing environment."
    az containerapp env update --name $ENV_NAME --resource-group $RESOURCE_GROUP --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_ID
    Write-Host "Container Apps Environment $ENV_NAME updated."
}

# Set Container Apps Environment OpenTelemetry collector
Write-Host "Setting OpenTelemetry collector for Container Apps Environment $ENV_NAME."
az containerapp env telemetry app-insights set --name $ENV_NAME --resource-group $RESOURCE_GROUP --connection-string $APP_INSIGHTS_CONNECTION_STRING --enable-open-telemetry-traces true --enable-open-telemetry-logs true
Write-Host "OpenTelemetry collector set for Container Apps Environment $ENV_NAME."

# Check if the Claim Status API container app exists
$existingApp = az containerapp show --name $CLAIMSTATUSAPI_APP_NAME --resource-group $RESOURCE_GROUP --query name --output tsv 2>$null
if ([string]::IsNullOrEmpty($existingApp)) {
    # Container app does not exist, create it
    Write-Host "Container app $CLAIMSTATUSAPI_APP_NAME does not exist. Creating it."
    az containerapp create `
      --name $CLAIMSTATUSAPI_APP_NAME `
      --resource-group $RESOURCE_GROUP `
      --environment $ENV_NAME `
      --image mcr.microsoft.com/azuredocs/aci-helloworld `
      --registry-server $ACR_REGISTRY_SERVER `
      --target-port 8080 `
      --ingress external `
      --min-replicas 1 `
      --max-replicas 1 `
      --cpu 0.25 `
      --memory 0.5Gi `
      --env-vars APPLICATIONINSIGHTS_CONNECTION_STRING=$APP_INSIGHTS_CONNECTION_STRING
}

# Azure OpenAI Resource
$openAiExists = az cognitiveservices account show --name $OPENAI_RESOURCE_NAME --resource-group $RESOURCE_GROUP --query name --output tsv 2>$null
if ([string]::IsNullOrEmpty($openAiExists)) {
    Write-Host "Creating Azure OpenAI Resource $OPENAI_RESOURCE_NAME."
    az cognitiveservices account create --name $OPENAI_RESOURCE_NAME --resource-group $RESOURCE_GROUP --location $LOCATION --kind OpenAI --sku S0 --yes
    Write-Host "Azure OpenAI Resource $OPENAI_RESOURCE_NAME created."
} else {
    Write-Host "Azure OpenAI Resource $OPENAI_RESOURCE_NAME already exists."
}

# Deploy GPT-4o-mini model
Write-Host "Deploying GPT-4o-mini model to Azure OpenAI resource $OPENAI_RESOURCE_NAME."
az cognitiveservices account deployment create --resource-group $RESOURCE_GROUP --name $OPENAI_RESOURCE_NAME --deployment-name $GPT4O_DEPLOYMENT_NAME --model-name $GPT4O_DEPLOYMENT_NAME --model-version "2024-07-18" --model-format OpenAI --sku-capacity 4 --sku-name "GlobalStandard"
Write-Host "GPT-4o-mini model deployed."

az logout