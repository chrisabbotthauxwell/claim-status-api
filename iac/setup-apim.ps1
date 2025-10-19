. "$PSScriptRoot\variables.ps1"

az login --tenant $TENANT_ID

# Update APIM with swagger file
Write-Host "Updating APIM with swagger file..."
az apim api import `
    --resource-group $RESOURCE_GROUP `
    --service-name $APIM_NAME `
    --service-url $CONTAINER_APP_URL `
    --path $APIM_CLAIMS_PATH `
    --api-id $APIM_CLAIMSTATUSAPI_ID `
    --specification-format OpenApi `
    --specification-path "$PSScriptRoot\..\apim\claimstatusapi-swagger.json" `
    --subscription-required true
Write-Host "APIM updated with swagger file."