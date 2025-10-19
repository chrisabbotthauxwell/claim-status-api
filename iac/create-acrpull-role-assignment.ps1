. "$PSScriptRoot\variables.ps1"

az login --tenant $TENANT_ID

# Ensure Container App managed identity has AcrPull on ACR
Write-Host "Ensuring Container App managed identity has AcrPull on ACR."

# get principalId of the container app (system-assigned identity)
$containerPrincipalId = az containerapp show --name $CLAIMSTATUSAPI_APP_NAME --resource-group $RESOURCE_GROUP --query identity.principalId -o tsv

if ([string]::IsNullOrEmpty($containerPrincipalId)) {
    Write-Host "Container App has no managed identity. Enabling system-assigned identity."
    az containerapp update --name $CLAIMSTATUSAPI_APP_NAME --resource-group $RESOURCE_GROUP --set identity.type=SystemAssigned
    # re-read principalId
    $containerPrincipalId = az containerapp show --name $CLAIMSTATUSAPI_APP_NAME --resource-group $RESOURCE_GROUP --query identity.principalId -o tsv
}

if (-not [string]::IsNullOrEmpty($containerPrincipalId)) {
    # get ACR resource id (scope)
    $acrId = az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query id -o tsv

    # check for existing AcrPull assignment for this principal at ACR scope
    $hasAcrPull = az role assignment list --assignee $containerPrincipalId --scope $acrId --query "[?roleDefinitionName=='AcrPull'] | length(@)" -o tsv

    if ($hasAcrPull -eq "0") {
        Write-Host "Assigning AcrPull to principal $containerPrincipalId on $acrId"
        az role assignment create --assignee $containerPrincipalId --role "AcrPull" --scope $acrId
        Write-Host "AcrPull assigned."
    } else {
        Write-Host "AcrPull already assigned to principal $containerPrincipalId."
    }
} else {
    Write-Host "Failed to obtain container app principalId. Cannot assign AcrPull."
}

az logout