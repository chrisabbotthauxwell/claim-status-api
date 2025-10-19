. "$PSScriptRoot\variables.ps1"

$acrCreds = az acr credential show --name $ACR_NAME --resource-group $RESOURCE_GROUP | ConvertFrom-Json
$ACR_USERNAME = $acrCreds.username
$ACR_PASSWORD = $acrCreds.passwords[0].value

docker login $ACR_REGISTRY_SERVER --username $ACR_USERNAME --password $ACR_PASSWORD
# Build and push ClaimStatusAPI image
Write-Host "Building and pushing ProductsService image."
docker build --no-cache -t claimstatusapi:0.0.1 src/ClaimStatusAPI/.
docker tag claimstatusapi:0.0.1 $CLAIMSTATUSAPI_IMAGE_NAME
docker push $CLAIMSTATUSAPI_IMAGE_NAME

# Get App Insights connection string
$APP_INSIGHTS_CONNECTION_STRING = az monitor app-insights component show `
  --app $APP_INSIGHTS_NAME `
  --resource-group $RESOURCE_GROUP `
  --query connectionString `
  --output tsv

# Store connection string in Key Vault
# az keyvault secret set --vault-name $KEY_VAULT_NAME --name "AppInsightsConnectionString" --value $APP_INSIGHTS_CONNECTION_STRING

# Check if the Claim Status API container app exists
$existingApp = az containerapp show --name $CLAIMSTATUSAPI_APP_NAME --resource-group $RESOURCE_GROUP --query name --output tsv 2>$null

if ([string]::IsNullOrEmpty($existingApp)) {
    # Container app does not exist, create it
    Write-Host "Container app $CLAIMSTATUSAPI_APP_NAME does not exist. Creating it."
    az containerapp create `
      --name $CLAIMSTATUSAPI_APP_NAME `
      --resource-group $RESOURCE_GROUP `
      --environment $ENV_NAME `
      --image $CLAIMSTATUSAPI_IMAGE_NAME `
      --registry-server $ACR_REGISTRY_SERVER `
      --target-port 80 `
      --ingress external `
      --min-replicas 1 `
      --max-replicas 1 `
      --cpu 0.25 `
      --memory 0.5Gi `
      --registry-username $ACR_USERNAME `
      --registry-password $ACR_PASSWORD `
      --env-vars APPLICATIONINSIGHTS_CONNECTION_STRING=$APP_INSIGHTS_CONNECTION_STRING
} else {
    # Container app exists, update it
    Write-Host "Container app $CLAIMSTATUSAPI_APP_NAME already exists. Updating it."
    az containerapp update `
      --name $CLAIMSTATUSAPI_APP_NAME `
      --resource-group $RESOURCE_GROUP `
      --image $CLAIMSTATUSAPI_IMAGE_NAME
}