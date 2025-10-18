# claim-status-api
A Claim Status API for an academic exercise

## Tech stack
- Development framework: C# .net 8.0 (LTS)
- Unit test framework: xUnit
- Test mock frameowk: Moq
- Containerisation: Docker

## Development approach
- Test Driven Development (TDD)

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

## Running locally
### Vanilla .net HTTP service
Build and Run the HTTP WebAPI from the terminal window.

1. Browse to the ClaimStatusAPI folder and run `dotnet build` followed by `dotnet run`:

```
C:\claim-status-api\src\ClaimStatusAPI> dotnet build
Restore complete (0.4s)
  ClaimStatusAPI succeeded (4.7s) → bin\Debug\net8.0\ClaimStatusAPI.dll

C:\claim-status-api\src\ClaimStatusAPI> dotnet run
Using launch settings from C:\claim-status-api\ClaimStatusAPI\Properties\launchSettings.json...
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5017
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\claim-status-api\src\ClaimStatusAPI
```

2. Access the Swagger UI on http://localhost:5017/swagger.

### Containerised (Docker) service
Build and run the containerised HTTP service from Docker
1. Build new Docker image from repo root
```
docker build -f src/ClaimStatusAPI/Dockerfile -t claimstatusapi:latest src/ClaimStatusAPI
```
2. Run containerised app
```
docker run --rm -e ASPNETCORE_ENVIRONMENT=Development -p 5017:80 claimstatusapi:latest
```
3. Access the Swagger UI on http://localhost:5017/swagger.
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

## Secrets strategy (local dev, containers/CI, production)

This project uses secrets for Azure OpenAI (endpoint, key, model). Follow the guidance below — never commit secrets to source control.

### 1) Local development — dotnet user-secrets
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
- Access in code (Program.cs):
```csharp
// sample read (user-secrets are integrated into IConfiguration during development)
var endpoint = builder.Configuration["AzureOpenAI:Endpoint"];
var key = builder.Configuration["AzureOpenAI:Key"];
var model = builder.Configuration["AzureOpenAI:Model"];
```
- Advantages: per-project, stored outside repo, easy to change. Good for TDD and local runs.

### 2) Container local / CI — Environment variables (for containers / pipelines)
- Provide secrets as environment variables when running locally with Docker:
```bash
docker run --rm -e ASPNETCORE_ENVIRONMENT=Development -e AZURE_OPENAI_ENDPOINT="https://..." -e AZURE_OPENAI_KEY="..." -e OPENAI_MODEL="gpt-4o-mini" -p 5017:80 claimstatusapi:0.1.0
```
- In CI/pipeline, store values in secret variables and inject as env vars during build/release.
- Read in code (prefer a fallback to allow either config style):
```csharp
var endpoint = builder.Configuration["AzureOpenAI:Endpoint"] 
               ?? builder.Configuration["AZURE_OPENAI_ENDPOINT"];
var key = builder.Configuration["AzureOpenAI:Key"] 
          ?? builder.Configuration["AZURE_OPENAI_KEY"];
var model = builder.Configuration["AzureOpenAI:Model"] 
            ?? builder.Configuration["OPENAI_MODEL"];
```

### 3) Production — Azure Key Vault + Managed Identity (best practice)
- Store production secrets in Azure Key Vault and give the container app a managed identity.
- Use Azure Key Vault configuration provider to load secrets into IConfiguration:
```csharp
// Requires Azure.Extensions.AspNetCore.Configuration.Secrets and Azure.Identity
builder.Configuration.AddAzureKeyVault(new Uri("https://<your-vault>.vault.azure.net/"), new DefaultAzureCredential());
```
- Advantages: centralized rotation, RBAC, audit logs, no secrets in app/container.