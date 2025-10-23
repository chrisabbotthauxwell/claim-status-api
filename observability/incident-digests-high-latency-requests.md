# Incident Digests for High Latency Requests
1. The KQL results shown in [high-latency-traces.md](high-latency-traces.md) were exported as CSV from Log Analytics and saved as CSV in [AppRequests-high-latency-last-2d.csv](AppRequests-high-latency-last-2d.csv)
2. A prompt was constructed and passed to GenAI to request a brief Incident Digest for each failure response code

## Incident Digest Prompt
> @workspace Consider the Log Analytics output in `AppRequests-high-latency-last-2d.csv` showing various high latency requests over the last 2 days.
>
>Identify trends or groups. Consider what each endpoint or request is doing and whether the observed latency is expected, acceptable or otherwise. Prepare a brief Incident Digest using the `### Incident digest template`

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

High latency is expected for the summarization endpoint due to the nature of AI inference, but should be monitored and bounded. 404s with high latency may indicate additional backend or data issues that warrant investigation..

### Incident Digest

**Summary:**  
Between 21/10/2025 20:11:56 and 22/10/2025 17:42:27, the Claim Status API experienced 13 high-latency requests (over 2 seconds), primarily on the POST /v1/Claims/{id}/summarize endpoint, with durations ranging from ~2.2s up to ~5s.

**Top Issues:**
- Most common high-latency endpoint: POST /v1/Claims/{id}/summarize via APIM and Container App backend
- Highest latency observed: 4975 ms on POST /v1/Claims/1002/summarize (APIM and backend)
- All high-latency requests are associated with claim summarization, which involves an OpenAI call
- Occasional 404s with high latency, indicating backend routing or data issues

**Next Steps:**
- Review OpenAI model response times and investigate if latency is due to external dependency
- Check for any backend processing bottlenecks in ClaimsService or OpenAiService
- Validate that high latency is expected for summarization (due to AI inference) and document acceptable thresholds
- Investigate 404s with high latency for possible routing or data consistency issues

**Recommended Actions:**
- [ ] Set explicit latency SLO/SLA for summarization endpoints (e.g., <5s for POST /claims/{id}/summarize)
- [ ] Monitor OpenAI dependency latency and consider retries or timeouts
- [ ] Add user-facing messaging if summarization is expected to be slow
- [ ] Review and optimize backend code for prompt construction and response parsing
- [ ] Investigate and resolve high-latency 404s to improve error handling and routing