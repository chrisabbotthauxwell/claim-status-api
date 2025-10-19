. "$PSScriptRoot\variables.ps1"

az login --tenant $TENANT_ID

# Azure API Management (Development SKU)
# APIM creation is separated from the main create-infrastructure script to avoid long deployment times and unecessary costs
$apimExists = az apim show --name $APIM_NAME --resource-group $RESOURCE_GROUP --query name --output tsv 2>$null
if ([string]::IsNullOrEmpty($apimExists)) {
    Write-Host "Creating Azure API Management (Development) $APIM_NAME."
    az apim create `
        --name $APIM_NAME `
        --resource-group $RESOURCE_GROUP `
        --location $LOCATION `
        --publisher-email $APIM_PUBLISHER_EMAIL `
        --publisher-name $APIM_PUBLISHER_NAME `
        --sku-name "Developer"
    Write-Host "Azure API Management $APIM_NAME created."
} else {
    Write-Host "Azure API Management $APIM_NAME already exists."
}

az logout