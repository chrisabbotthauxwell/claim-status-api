# API Reference (auto-generated from routes)

## GET `/claims/{id}`
- **Description:** Returns claim status from `src/ClaimStatusAPI/mocks/claims.json` by `{id}`.
- **Parameters:**
	- `id` (string, required): Claim identifier.
- **Responses:**
	- `200 OK`: Claim object.
	- `404 Not Found`: Claim not found.

## POST `/claims/{id}/summarize`
- **Description:** Fetches mock notes for `{id}` and returns a multi-part summary using Azure OpenAI.
- **Parameters:**
	- `id` (string, required): Claim identifier.
- **Responses:**
	- `200 OK`:
		```json
		{
			"summary": "...",
			"customerSummary": "...",
			"adjusterSummary": "...",
			"nextStep": "..."
		}
		```
	- `404 Not Found`: Claim or notes not found.
	- `502 Bad Gateway`: Upstream/model error.

**See:** [`apim/claimstatusapi-swagger.json`](../apim/claimstatusapi-swagger.json) for full OpenAPI spec.