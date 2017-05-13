# Distributed logging and tracing kata


Data collection, visualization and analysis are essential to predict, detect and troubleshoot software failures.

Monitoring, alerting, [distributed tracing](http://microservices.io/patterns/observability/distributed-tracing.html) and [log aggregation](http://microservices.io/patterns/observability/application-logging.html) tools collect data from:
 - system metrics
 - application metrics
 - application tracing
 - system and application logs

Metrics, application tracing and logging are usually handled with different tools:
 - Telegraf, InfluxDB, Grafana and Kapacitor for monitoring and alerting (included anomaly detection) on metrics
 - Zipkin/Jaeger for application tracing
 - Splunk/Graylog for logs

This makes troubleshooting more difficult.


## Goal
Design a solution for log aggregation and distributed tracing to centralize the management of log and trace information in a distributed system.

**Logs** record information about application events: errors, warnings and other meaningful operational events.

**Traces** record timing data and other information about function and service calls (Service A called endpoint X in Service B successfully and it took T ms).


## Requirements
 - Be able to correlate application logs and traces
 - Be able to create a map of service calls (direct and indirect) per user request
 - Be able to add dashboards to visualize and cross information coming from logs and traces
 - Prepare your system to be able to analyse data and predict failures


## Deliverables
 - Component diagram of the overall proposed solution, describing the technologies chosen for each logical block
 - Interaction diagrams between the identified components
 - Proof of concept of the proposed solution