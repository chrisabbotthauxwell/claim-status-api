# Objective
 - Deploy a Claim Status API in Azure Container Apps (ACA) fronted by API Management (APIM).
 - Implement: GET /claims/{id} (status) and POST /claims/{id}/summarize (calls Azure OpenAI to return a summary from mock notes).
 - Secure and automate via Azure DevOps CI/CD with image scanning and enable observability.

# Learning outcomes
 - Deploy a containerized API behind APIM.
 - Integrate GenAI (Azure OpenAI) into API flows for summarization and recommendations.
 - Implement CI/CD with image scanning and optional SBOMs; understand Defender vs OSS options.
 - Monitor APIs using APIM analytics & Container Insights and apply AI-assisted observability.