# Implementation steps
This document details the steps and tasks required to complete the exercise, and will be used to track progress to completion.

## GenAI assisted tasks
- [ ] **Functional:** generate customer-facing summaries and a longer adjuster version; produce “next-step” recommendation text for responses.
- [ ] **DevOps:** auto-generate Azure DevOps pipeline YAML and mock claims.json/notes.json datasets.
- [ ] **Security:** summarize Defender/scan outputs into prioritized remediation actions; generate APIM policy snippets for throttling & auth.
- [ ] **Observability:** produce KQL queries for failing requests and high-latency traces; create plain-English incident digests.
- [ ] **Documentation:** auto-generate API reference from routes and draft a short ADR (“Adopt APIM as gateway for claims summarization”).

## Implement Claim Status API
- [ ] Skeleton folder set up
- [ ] Boiler plate .net API using WeatherForecast template
- [ ] Create unit test project
- [ ] Create integration test project
- [ ] Create `GET /claims/{id}` endpoint --> returns static claim JSON from `claims.json`
- [ ] Generate `notes.json`
- [ ] Create `POST /claims/{id}/summarize` endpoint --> reads mock notes for the id from `notes.json`, calls Azure OpenAI to generate multi-part summary for the claim using claim notes and claim details
    - Requires:
        - OpenAI deployment + chat completion endpoint
        - `notes.json` populated with mock claim notes
    - Returns:
        - Multi-part summary response JSON payload, including `summary`, `customerSummary`, `adjusterSummary` and `nextStep` sections:
```json 
{
    "summary": "...",
    "customerSummary": "...",
    "adjusterSummary": "...",
    "nextStep": "..."
}
```

## Set up infrastricture
- [ ] Create foundation powershell script to run `az cli` tasks
- [ ] Create Resource Group
- [ ] Create OpenAI resources: AI Foundry, GPT-4o
- [ ] Create Container Registry
- [ ] Create Log Analytics workspace
- [ ] Create App Insights
- [ ] Create APIM

## Containerise the service
- [ ] Containerise the service --> Dockerfile
- [ ] Push the image to ACR

## CI/CD pipeline in Azure DevOps -- build & deploy flow:
- [ ] Build image
- [ ] Security/scan step: EITHER Push to ACR & run Defender for Containers to auto-scan image, OR use Defender for DevOps for IaC posture
- [ ] Gate: fail pipeline if critical/high vulnerability found (policy)
- [ ] Deploy ACA app and APIM configuration (Bicep/az cli)

## Configure APIM routes and policies
- [ ] `GET /claims/{id}` --> returns claim status from Claim Status API endpoint, which returns claim status JSON from `claims.json` by `{id}`
- [ ] `POST /claims/{id}/summarize` --> fetches mock notes for `{id}` from the Claim Status API endpoint, which  returns the multi-part summary by `{id}`
- [ ] Apply APIM policy for rate limiting
- [ ] Apply APIM policy for subscription key auth

## Enable observability
- [ ] APIM analytics + container insights into Log Analytics
- [ ] Save a couple of KQL queries for errors/latency

## Test endpoints via APIM
- [ ] Call `GET /claims/{id}` to verify status response
- [ ] Call `POST /claims/{id}/summarise` and confirm the response contains complete multi-part summary
- [ ] Inspect logs/latency in APIM analytics and Container Insights 

## Repository Structure
claim-status-api/
├── src/                     # service source + Dockerfile
├── mocks/
│   ├── claims.json          # 5–8 claim records
│   └── notes.json           # 3–4 notes blobs
├── apim/                    # APIM policy files or export
├── iac/                     # Bicep/Terraform/Az CLI templates
├── pipelines/
│   └── azure-pipelines.yml  # Azure DevOps pipeline
├── scans/                   # link/screenshots to Defender findings
├── observability/           # saved KQL queries and sample screenshots
└── README.md                # instructions, GenAI prompts, how to run/tests