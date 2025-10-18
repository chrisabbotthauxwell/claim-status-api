# Implementation steps
This document details the steps and tasks required to complete the exercise, and will be used to track progress to completion.

## GenAI assisted tasks
- [ ] **Functional:** generate customer-facing summaries and a longer adjuster version; produce “next-step” recommendation text for responses.
- [ ] **DevOps:** auto-generate Azure DevOps pipeline YAML and mock claims.json/notes.json datasets.
- [ ] **Security:** summarize Defender/scan outputs into prioritized remediation actions; generate APIM policy snippets for throttling & auth.
- [ ] **Observability:** produce KQL queries for failing requests and high-latency traces; create plain-English incident digests.
- [ ] **Documentation:** auto-generate API reference from routes and draft a short ADR (“Adopt APIM as gateway for claims summarization”).

## Implement Claim Status API
- [x] Skeleton folder set up
- [x] Boiler plate .net API using WeatherForecast template
- [x] Create unit test project
- [ ] Create integration test project
- [x] Create `GET /claims/{id}` endpoint --> returns static claim JSON from `claims.json`
- [x] Implement logging
- [x] Generate `notes.json`
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

## POST /claims/{id}/summarize — Master checklist

Use this master checklist to track work required to implement the POST /claims/{id}/summarize endpoint (reads mock notes, calls Azure OpenAI, returns multi‑part ClaimSummary).

### Phase 0 — Plan & prerequisites
- [x] Define acceptance criteria (response shape, status codes, error handling, latency/SLA)
  - Response must include: summary, customerSummary, adjusterSummary, nextStep
  - Endpoint returns 200 with valid JSON, 404 when claim or notes missing, 502/500 for upstream errors
- [x] Confirm notes.json + claims.json are present and copied to publish output
- [x] Decide local secret mechanism (dotnet user-secrets or env vars) and production secret plan (Key Vault / managed identity)

### Phase 1 — Models & Contracts
- [x] Add DTOs
  - [x] ClaimNote (single note record)
  - [x] ClaimsNotes (collection wrapper with claim id)
  - [x] ClaimSummary (summary, customerSummary, adjusterSummary, nextStep)
- [x] Update IClaimsService
  - [x] Add Task<ClaimSummary?> GetSummaryByIdAsync(string claimId) signature (async)

### Phase 2 — OpenAI integration abstraction
- [x] Add IOpenAiService interface (single method: Task<string> CreateChatCompletionAsync(promptPayload,...))
- [x] Add OpenAiService implementation using OpenAI SDK
- [ ] Implement safe parsing helper to convert model output to ClaimSummary (validate JSON, strict schema)

### Phase 3 — ClaimsService changes
- [x] Implement reading notes.json into ClaimsNotes objects (copy to output, case-insensitive deserialization)
- [x] Implement ClaimsService.GetSummaryByIdAsync:
  - [x] Fetch claim by id
  - [x] Fetch corresponding notes
  - [x] Build prompt bundle (claim details + notes)
  - [x] Call IOpenAiService and parse/validate result into ClaimSummary
  - [ ] Graceful handling when model returns invalid JSON (retry/transform or return 502)

### Phase 4 — API Controller
- [x] Add POST /claims/{id}/summarize endpoint to ClaimsController
  - [x] Route: [HttpPost("{id}/summarize")]
  - [x] Signature: ssync Task<ActionResult<ClaimSummary>> SummarizeClaimAsync(string id)
  - [x] Return 200/404/502 as appropriate
- [ ] Add request/response OpenAPI documentation (Swagger annotations)

### Phase 5 — Prompt & orchestration
- [ ] Design system + user prompt template (explicitly request strict JSON with the four fields)
- [ ] Implement prompt truncation / chunking strategy for long notes (or pre-summarize long notes)
- [ ] Add deterministic instructions for token limits and model selection (configurable)

### Phase 6 — Resilience & security
- [ ] Add HTTP client timeout + retries (Polly) around OpenAI calls
- [ ] Validate and sanitize inputs (id param)
- [ ] Rate-limit or throttle summarization endpoint (APIM or in-app)
- [ ] Ensure secrets are never checked into repo; use env vars or user-secrets locally

### Phase 7 — Tests
- [ ] Unit tests
  - [ ] Mock IOpenAiService to assert ClaimsService and Controller behavior
  - [ ] Tests for success, missing claim, missing notes, invalid model response
- [ ] Integration tests (optional)
  - [ ] Local integration test that runs against a test double or recorded responses (avoid real OpenAI calls)
- [ ] End-to-end manual test plan (Swagger and container run)

### Phase 8 — Container / CI / Deploy prep
- [ ] Ensure notes.json is included in csproj publish output
- [ ] Update Dockerfile if required (publish contains mocks)
- [ ] Add CI step (pipeline) to run unit tests
- [ ] Add CI step to build image and push to registry (ACR) — include image scan step later
- [ ] Add environment variable wiring for Azure secrets in deployment scripts / Bicep

### Phase 9 — Observability & Ops
- [ ] Add structured logging for prompt, request id, model response status (avoid logging secrets)
- [ ] Add metrics: request latency, OpenAI call success/failure, token usage (if available)
- [ ] Create KQL snippets for errors and high-latency traces

### Phase 10 — Documentation & handover
- [ ] Update README with local dev steps and secret setup
- [ ] Add example curl / HTTP snippets for the new endpoint
- [ ] Document prompt template and intended model behavior (for future tuning)
- [ ] Create an ADR if using OpenAI for production decision-making (risk/cost/ops)

---

Notes / Helpful hints
- Keep OpenAI code behind an interface so unit tests never require network calls.
- Return a clear error payload on upstream failure so callers (and APIM) can act appropriately.
- For local development prefer dotnet user-secrets or env vars; migrate to Key Vault in Azure.
- Break the work into small PRs: Models+IClaimsService → ClaimsService notes load → OpenAI service → Controller + tests.

## Set up infrastricture
- [ ] Create foundation powershell script to run `az cli` tasks
- [ ] Create Resource Group
- [ ] Create OpenAI resources: AI Foundry, GPT-4o
- [ ] Create Container Registry & Container App
- [ ] Create Log Analytics workspace
- [ ] Create App Insights
- [ ] Create APIM

## Containerise the service
- [x] Containerise the service --> Dockerfile
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
│   ├── ClaimStatusAPI/
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── mocks/
│   │   │   ├── claims.json  # 5–8 claim records
│   │   │   └── notes.json   # 3–4 notes blobs
│   │   ├── Dockerfile
│   │   └── ClaimStatusAPI.csproj
│   └── ClaimStatusAPI.UnitTests/
├── apim/                    # APIM policy files or export
├── iac/                     # Bicep/Terraform/Az CLI templates
├── pipelines/
│   └── azure-pipelines.yml  # Azure DevOps pipeline
├── scans/                   # link/screenshots to Defender findings
├── observability/           # saved KQL queries and sample screenshots
└── README.md                # instructions, GenAI prompts, how to run/tests