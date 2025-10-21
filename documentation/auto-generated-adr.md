# ADR: Adopt APIM as Gateway for Claims Summarization

**Context:**  
The Claim Status API exposes endpoints for claim retrieval and summarization, including integration with Azure OpenAI. Direct exposure of these endpoints would limit control over security, rate limiting, and observability.

**Decision:**  
Adopt Azure API Management (APIM) as the public gateway for all Claim Status API endpoints.

**Consequences:**
- Centralized authentication, rate limiting, and monitoring.
- Enables policy-based controls (e.g., subscription keys, throttling).
- Simplifies future integration with other APIs and external consumers.
- Adds a small latency and cost overhead, but improves security and manageability.

**Status:**  
Accepted and implemented. All external traffic is routed via APIM.