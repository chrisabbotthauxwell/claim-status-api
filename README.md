# claim-status-api
A Claim Status API for an academic exercise

## Local execution
### Vanilla dotnet service
1. Build and Run
In the terminal window, browse to the ClaimStatusAPI folder and run `dotnet build` followed by `dotnet run`:

```
C:\claim-status-api\src\ClaimStatusAPI>dotnet build
Restore complete (0.4s)
  ClaimStatusAPI succeeded (4.7s) → bin\Debug\net8.0\ClaimStatusAPI.dll

C:\claim-status-api\src\ClaimStatusAPI>dotnet run
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

### Containerised (Docker) service

## Repository structure
claim-status-api/
├── src/                     # service source + Dockerfile
│   └── ClaimStatusAPI/
│       ├── Controllers/
│       ├── Models/
│       ├── mocks/
│       │   ├── claims.json  # 5–8 claim records
│       │   └── notes.json   # 3–4 notes blobs
│       ├── Dockerfile
│       └── ClaimStatusAPI.csproj
├── apim/                    # APIM policy files or export
├── iac/                     # Bicep/Terraform/Az CLI templates
├── pipelines/
│   └── azure-pipelines.yml  # Azure DevOps pipeline
├── scans/                   # link/screenshots to Defender findings
├── observability/           # saved KQL queries and sample screenshots
└── README.md                # instructions, GenAI prompts, how to run/tests