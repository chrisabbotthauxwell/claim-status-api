# claim-status-api
A Claim Status API for an academic exercise

## Tech stack
- Development framework: C# .net 8.0 (LTS)
- Unit test framework: xUnit
- Test mock frameowk: Moq
- Containerisation: Docker
- Infrastructure deployment: Powershell and Az CLI
- Application CI build & deploy pipelines: Azure DevOps

## Service description
### Get claim by id
- Returns a static claim json object populated from claim details in `claims.json` by id

#### Request
|HTTP verb|Path|
|--|--|
|`GET`|`claim/{id}`|

|Parameter|Mandatory|Description|
|--|--|--|
|`id`|TRUE|The id of the claim|

#### Response
```json
{
  "id": "1001",
  "claimNumber": "CLM-1001",
  "policyNumber": "POL-4321",
  "claimant": {
    "firstName": "Alice",
    "lastName": "Martin",
    "phone": "555-0101",
    "email": "alice.martin@example.com"
  },
  "dateOfLoss": "2025-08-01T14:30:00Z",
  "reportedDate": "2025-08-02T09:15:00Z",
  "status": "Open",
  "reserveAmount": 12000,
  "paidAmount": 0,
  "estimatedLoss": 15000,
  "adjusterId": "ADJ-200",
  "description": "Rear-end collision at intersection; claimant reports whiplash and rear bumper damage.",
  "notesRef": "1001"
}
```

### Request claim summary
- Reads claim details in `claims.json` by id
- Reads claim notes in `notes.json` by id
- Submits all claim information to OpenAI
- Returns a claim summary json object containing `summary`, `customerSummary`, `adjusterSummary` and `nextSteps` sections.

#### Request
|HTTP verb|Path|
|--|--|
|`POST`|`claim/{id}/summarize`|

|Parameter|Mandatory|Description|
|--|--|--|
|`id`|TRUE|The id of claim to be summarised|

#### Response
```json
{
  "summary": "Claim CLM-1001 involves a rear-end collision reported by claimant Alice Martin, resulting in neck pain and vehicle damage. The initial estimate for vehicle repair is $4,200, while the on-site inspection estimated $6,800 in repairs. The adjuster has set a reserve of $12,000 and is following up on medical outcomes and final repair invoices.",
  "customerSummary": "Alice Martin reported a rear-end collision that occurred on August 1, 2025. She is experiencing neck and lower back pain and has provided documentation including photos and a police report. The vehicle damage has been assessed at approximately $6,800. She has been advised to seek further medical attention if symptoms persist.",
  "adjusterSummary": "Adjuster ADJ-200 has opened the claim, collected necessary documentation, and increased the reserve to cover estimated vehicle repairs and medical expenses following a soft tissue injury diagnosis. The next steps include obtaining a final repair invoice and monitoring the claimant's medical progress.",
  "nextStep": "Follow up with the claimant and the repair shop in 7 days."
}
```

## Development approach
- Test Driven Development (TDD)

## Repository structure
```
claim-status-api/
├── src/                            # service source + Dockerfile
│   └── ClaimStatusAPI/
│       ├── Controllers/
│       ├── Models/
│       ├── mocks/
│       │   ├── claims.json         # 5–8 claim records
│       │   └── notes.json          # 3–4 notes blobs
│       ├── Dockerfile
│       └── ClaimStatusAPI.csproj
├── apim/                           # APIM policy files or export
├── iac/                            # Bicep/Terraform/Az CLI templates
├── pipelines/
│   └── azure-pipelines.yml         # Azure DevOps pipeline
├── scans/                          # link/screenshots to Defender findings
├── observability/                  # saved KQL queries and sample screenshots
├── README.md                       # instructions, GenAI prompts, how to run/tests
└── claim-status-api.sln
```
## Running locally
### Vanilla .net HTTP service
Build and Run the HTTP WebAPI from the terminal window.

1. Set secrets - see Secrets strategy section)
2. Run application from the ClaimStatusAPI folder:
```
dotnet build
dotnet run
```
3. Access the Swagger UI on http://localhost:5017/swagger.

### Containerised (Docker) service
Build and run the containerised HTTP service from Docker
1. Build new Docker image from repo root
```
docker build -f src/ClaimStatusAPI/Dockerfile -t claimstatusapi:0.0.1 src/ClaimStatusAPI
```
2. Run containerised app passing secrets as environment variables:
```
docker run --rm -e ASPNETCORE_ENVIRONMENT=Development -e AZURE_OPENAI_ENDPOINT="https://<your-endpoint>" -e AZURE_OPENAI_KEY="<your-key>" -e OPENAI_MODEL="gpt-4o-mini" -p 5017:80 claimstatusapi:0.0.1
```
3. Access the Swagger UI on http://localhost:5017/swagger.

## Secrets strategy (local dev, containers/CI, production)

This project uses secrets for Azure OpenAI (endpoint, key, model). Follow the guidance below — never commit secrets to source control.

### 1) Local development - dotnet user-secrets
Required when running the application as a local HTTP service.
- Initialize user-secrets (run in the API project folder):
```powershell
cd src\ClaimStatusAPI
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>"
dotnet user-secrets set "AzureOpenAI:Key" "<your-key>"
dotnet user-secrets set "AzureOpenAI:Model" "gpt-4o-mini"
```
- Confirm values
```
dotnet user-secrets list
```

### 2) Container local / CI — Environment variables (for containers / pipelines)
- Provide secrets as environment variables when running locally with Docker:
```bash
docker run --rm -e ASPNETCORE_ENVIRONMENT=Development -e AZURE_OPENAI_ENDPOINT="https://..." -e AZURE_OPENAI_KEY="..." -e OPENAI_MODEL="gpt-4o-mini" -p 5017:80 claimstatusapi:0.1.0
```
- In CI/pipeline, store values in secret variables and inject as env vars during build/release.
      - OPENAI_MODEL (pipeline variable)
      - AZURE_OPENAI_ENDPOINT (pipeline secret)
      - AZURE_OPENAI_KEY (pipeline secret)

### 3) Production — Azure Key Vault + Managed Identity (best practice)
- Store production secrets in Azure Key Vault and give the container app a managed identity.
- Use Azure Key Vault configuration provider to load secrets into IConfiguration:
```csharp
// Requires Azure.Extensions.AspNetCore.Configuration.Secrets and Azure.Identity
builder.Configuration.AddAzureKeyVault(new Uri("https://<your-vault>.vault.azure.net/"), new DefaultAzureCredential());
```
- Advantages: centralized rotation, RBAC, audit logs, no secrets in app/container.

## Running unit tests
- Restore packages
```
dotnet restore
```
- Run all tests in the solution (from repo root)
```
dotnet test claim-status-api.sln
```
- Run tests for the unit test project only
```
dotnet test src/ClaimStatusAPI.UnitTests/ClaimStatusAPI.UnitTests.csproj
```

## Infrastructure
Infrastructure provision is handled using `Powershell` scrips in the `/iac/` folder which are run from the command line in the solution root.

### Deployment variables
`.\iac\variables.ps1` defines deployment variables for the deployment.

Increment the `$ATTEMPT_NO` value to reflect the deployment version.

### Deploy infrastructure
From the solution root folder in a Powershell terminal, run:
```bash
.\iac\create-infrastrcuture.ps1
```
This will create all required Azure services ready for app deployment, except an Azure API Management service.

### Deploy API Management
APIM creation is kept as a separate script to avoid:
- Long infrastructure creation script running times when creating the rest of the infrastructure
- To avoid unecessary costs in creating APIM before it's required

From the solution root folder in a Powershell terminal, run:
```bash
.\iac\create-apim.ps1
```

## Azure DevOps

### Organisation and project
Azure DevOps is used to create an Organisation and a Project to manage CI pipelines.

### Azure Resource Manager service connection
An ARM service connection `sc-claimstatusapi-3` is created to connect the Azure DevOps Project to the Azure resource group where the infrastructure is deployed.

### Github service connection
A Github service connection 'sc-github-chrisabbotthauxwell` is created using OAuth to connect the Azure DevOps Project to the Github repo.

### Azure Container Registry service connection
An Docker Registry service connection `sc-acr-claimstatusai-3` is created to connect the Azure DevOps Project to the ACR resource.

### Pipeline variables
The following variables are defined for the Azure DevOps pipeline and applied to the Container App during the deployment of the image:
- OPENAI_MODEL (pipeline variable)
- AZURE_OPENAI_ENDPOINT (pipeline secret)
- AZURE_OPENAI_KEY (pipeline secret)