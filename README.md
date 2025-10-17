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

2. Access the Swagger UI for the Claims Status API on `/swagger`

### Containerised (Docker) service

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