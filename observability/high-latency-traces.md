# High Latency Traces

## KQL query
Find requests that took longer than 2000ms to return:
```
AppRequests
| where DurationMs > 2000
| order by DurationMs desc
```

### KQL query results
![high-latency-traces](high-latency-traces.png)