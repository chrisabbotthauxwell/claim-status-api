# Incident Digests
1. The KQL results shown in [failure-responses.md](failure-responses.md) were exported as CSV from Log Analytics and saved as CSV in [AppRequests-failure-responses-last-24h.csv](AppRequests-failure-responses-last-24h.csv)
2. A prompt was constructed and passed to GenAI to request a brief Incident Digest for each failure response code

## Incident Digest Prompt
> Consider the Log Analytics output in `AppRequests-failure-responses-last-24h.csv` showing various failed app requests over the last 24 hours.
>
> For each failure type respose code, prepare a brief Incident Digest using the `## Incident digest template`

### Incident digest template
```
# Incident Digest

**Summary:**  
Between {start_time} and {end_time}, the Claim Status API experienced {failure_count} failed requests and {high_latency_count} high-latency requests.

**Top Issues:**
- Most common error: {top_error_code} on endpoint {top_error_endpoint}
- Highest latency observed: {max_latency} ms on endpoint {max_latency_endpoint}

**Next Steps:**
- Investigate root cause for {top_error_code} errors.
- Review performance for endpoints exceeding 2s latency.
- Check recent deployments or configuration changes.

**Recommended Actions:**
- [ ] Assign investigation to on-call engineer.
- [ ] Notify stakeholders if SLA is breached.
- [ ] Document findings and mitigation steps.
```

## Incident Digest prompt results
The following results were prepared by GenAI through inspecting the KQL results as CSV.

These incident digests show that most failures are actually security controls working correctly (401, 429) or expected behavior (404 for non-existent resources), with only the POST summarize 404 error potentially indicating a real issue requiring investigation.

### Incident Digest - HTTP 401 Unauthorized

**Summary:**  
Between 21/10/2025 17:39:59 and 22/10/2025 17:56:33, the Claim Status API experienced 4 failed requests due to missing or invalid subscription keys.

**Top Issues:**
- Most common error: HTTP 401 on endpoint GET /v1/Claims/1002 and POST /v1/Claims/1002/summarize
- Authentication failures affecting both claim retrieval and summarization endpoints
- All 401 errors had very low latency (<1 second), indicating fast rejection at APIM gateway

**Next Steps:**
- Verify APIM subscription key policy configuration is working correctly
- Check if legitimate clients are missing subscription keys in headers
- Review subscription key distribution to authorized consumers

**Recommended Actions:**
- [x] Confirm APIM policy is correctly rejecting unauthorized requests (working as intended)
- [ ] Provide clear API documentation showing required subscription key header
- [ ] Monitor for patterns indicating credential sharing or exposure

### Incident Digest - HTTP 404 Not Found

**Summary:**  
Between 21/10/2025 18:39:35 and 22/10/2025 16:51:16, the Claim Status API experienced 25+ failed requests for non-existent endpoints and resources.

**Top Issues:**
- Most common error: HTTP 404 on endpoints GET /, GET /favicon.ico, GET /swagger
- Security scanning attempts: GET /jsonrpc, GET /v1/mcp, GET /remote/logincheck
- Legitimate API endpoint: POST /Claims/1002/summarize (routing issue)

**Next Steps:**
- Investigate why POST /Claims/1002/summarize returned 404 (potential backend routing issue)
- Consider implementing basic security headers to deter scanning attempts
- Review APIM routing configuration for legitimate endpoints

**Recommended Actions:**
- [ ] Fix routing issue for POST /Claims/{id}/summarize endpoint
- [ ] Implement rate limiting for common scan paths to reduce noise
- [ ] Add security monitoring for suspicious endpoint probing patterns
- [ ] Document expected vs. unexpected 404 patterns for monitoring

### Incident Digest - HTTP 405 Method Not Allowed

**Summary:**  
Between 22/10/2025 17:36:29 and 22/10/2025 17:36:29, the Claim Status API experienced 2 failed requests due to incorrect HTTP method usage.

**Top Issues:**
- Most common error: HTTP 405 on endpoint GET /claims/1001/summarize
- Client attempted GET request on summarize endpoint which requires POST method
- Low latency (~1ms) indicating quick method validation

**Next Steps:**
- Review API documentation to ensure HTTP methods are clearly specified
- Check if client applications are using correct HTTP verbs
- Consider implementing better error messages for method mismatches

**Recommended Actions:**
- [ ] Update API documentation to clearly specify POST method for /claims/{id}/summarize
- [ ] Implement custom error response for 405 errors with correct method guidance
- [ ] Review client integration patterns to prevent method confusion

### Incident Digest - HTTP 429 Rate Limiting

**Summary:**  
Between 21/10/2025 18:58:58 and 22/10/2025 17:56:06, the Claim Status API experienced 7 failed requests due to rate limit enforcement.

**Top Issues:**
- Most common error: HTTP 429 on endpoint GET /v1/Claims/1004 and GET /v1/Claims/1002
- Rate limiting policy successfully enforcing 10 requests per 60-second window per IP
- All rate limited requests had very low latency (<3ms), showing efficient policy enforcement

**Next Steps:**
- Verify rate limiting thresholds are appropriate for legitimate use cases
- Check if rate limited requests represent normal usage patterns or potential abuse
- Consider implementing different rate limits for different subscription tiers

**Recommended Actions:**
- [x] Confirm APIM rate limiting policy is working as designed (functioning correctly)
- [ ] Monitor for legitimate clients hitting rate limits during normal operations
- [ ] Consider implementing usage analytics to optimize rate limit thresholds
- [ ] Document rate limits in API documentation for client developers