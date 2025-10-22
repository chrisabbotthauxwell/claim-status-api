# Provisioned Services
Running the infrastructure deployment scripts deploys the required infrastructure:

|Resource|Notes|
|--|--|
|Resource Group||
|Container Registry||
|Log Analytics Workspace||
|Application Insights||
|Container Apps Environment|OpenTelemetry collection enabled<br/>Integrated with App Insights|
|Container App|Initialised with stock image<br/>Integrated with App Insights<br/>Mapped to Container Registry
|Azure OpenAI|gpt-4o-mini deployed|

## Resource Group
![Resource group](images/provisioned-resourcegroup.png)

## Container Registry
The provisioned Container Registry showing the `claimstatusapi` Repository
![Provisioned ACR](images/provisioned-acr.png)

## Log Analytics Workspace
The provisioned Log Analytics Workspace showing the Usage dashboard
![Provisioned Log Analytics Workspace](images/provisioned-log-analytics.png)

## Application Insights
The provisioned Application Insights showing some Performance metrics for dependencies (OpenAI Chat Completion is slow here!)
![Provisioned Application Insights](images/provisioned-app-insights.png)

## Container Apps Environment
The provisioned Container Apps Environment showing the claimstatusapi Container App running and OpenTelemetry endpoint configuration
![Provisioned Container Apps Environment](images/provisioned-cae.png)
![Provisioned Container Apps Environment OpenTelemetry](images/provisioned-cae-otel.png)

## Container App
The provisioned Container App showing the deployed and running claimstatusapi image:
![Provisioned Container App](images/provisioned-container-app.png)

## Azure OpenAI
The provisioned OpenAI Service showing the deployed gpt-4o-mini model deployment
![Provisioned OpenAI](images/provisioned-openai.png)

## Azure API Management
The provisioned API Management showing the configured Claims Status API endpoints and inbound processing rate limit policy:
![Provisioned APIM](images/provisioned-apim.png)