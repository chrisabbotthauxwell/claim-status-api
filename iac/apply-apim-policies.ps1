. "$PSScriptRoot\variables.ps1"

az login --tenant $TENANT_ID

# Apply APIM rate limit policy
az apim api policy update `
    --resource-group $RESOURCE_GROUP `
    --service-name $APIM_NAME `
    --api-id $APIM_CLAIMSTATUSAPI_ID `
    --policy-file "$PSScriptRoot\..\apim\rate-limit-policy.xml"

az logout