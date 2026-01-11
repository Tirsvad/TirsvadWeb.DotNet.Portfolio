# KPI
The following table consolidates all requirements.
Each requirement ID starts with `KPI` and includes the original cross reference where present.

## Consolidated Requirements
| ID          | Cross Reference | Type                        | Requirement |
| ----------- | --------------- | --------------------------- | --- |
| KPI-FR-001  |                 | Functional requirement      | The system must be able serve 10 users simultaneously. |
| KPI-FR-002  |                 | Functional requirement      | Comprehensive test coverage for all features. |
| KPI-NFR-001 | [QC-001]        | Non-functional requirement  | Well-structured, modular, and maintainable code. |
| KPI-NFR-002 |                 | Non-functional requirement  | Regular performance and load testing. |
| KPI-NFR-003 | [OR-001]        | Non-functional requirement  | Implementation of monitoring for all services. |
| KPI-NFR-004 |                 | Non-functional requirement  | Alerts and notifications for critical issues. |
| KPI-NFR-005 |                 | Non-functional requirement  | Regular review of logs and performance metrics. |
| KPI-BR-001  |                 | Business requirement        | Comprehensive documentation for components and services. |
| KPI-BR-002  |                 | Business requirement        | Clear API documentation for external integrations. |
| KPI-BR-003  |                 | Business requirement        | User documentation and help resources. |
| KPI-BR-004  |                 | Business requirement        | Adherence to industry standards and regulations. |
| KPI-BR-005  |                 | Business requirement        | Regular security audits and assessments. |
| KPI-BR-006  | [LCR-001]       | Business requirement        | Compliance with data protection and privacy laws. |

---

## Functional KPI measurements

| ID         | KPI                                    | Measurement method                                               | Target             | Frequency            |
|------------|----------------------------------------|------------------------------------------------------------------|--------------------|----------------------|
| KPI-FR-001 | Stability (concurrent users)           | Load testing reports                                             | Passed             | Per Release           |
| KPI-FR-002 | Automated test coverage %              | Code coverage reports from CI (coverage tool)                    | ≥ 80%              | Per Release           |

---

## Non-functional KPI measurements

| ID         | KPI                                    | Measurement method                                               | Target             | Frequency            |
|------------|----------------------------------------|------------------------------------------------------------------|--------------------|----------------------|
| KPI-NFR-001 | Code quality (static analysis score)  | Static code analysis reports                                     | Score ≥ 90         | Per Release           |
| KPI-NFR-002 | Performance metrics (response time, throughput) | Performance testing reports                            | Meet defined response time and throughput targets | Per Release / On Load Test |
| KPI-NFR-003 | Monitoring coverage (%)               | Monitoring system reports                                        | 100% services monitored | Continuous / Monthly Review |
| KPI-NFR-004 | Alert response time                   | Incident management system reports                               | Critical alerts responded to within 15 minutes | Monthly / As Needed  |
| KPI-NFR-005 | Incident resolution time              | Incident management system reports                               | Incidents resolved within defined SLAs | Monthly / As Needed  |

---

## Business KPI measurements

| ID         | KPI                                    | Measurement method                                               | Target             | Frequency            |
|------------|----------------------------------------|------------------------------------------------------------------|--------------------|----------------------|
| KPI-BR-001 | Documentation coverage (% modules documented, doc age) | Documentation registry and last-updated checks | 100% modules documented; docs up-to-date | Per Release |
| KPI-BR-002 | API documentation coverage and examples | API docs coverage reports and example availability | 100% public endpoints documented with examples | On Change |
| KPI-BR-003 | User documentation satisfaction and discoverability | User satisfaction surveys and help search analytics | Satisfaction ≥ 80%; searchable help available | Per Release |
| KPI-BR-004 | Compliance with industry standards (audit pass rate) | Audit reports and compliance checklists | Audit pass with no critical non-conformances | Annual / Audit |
| KPI-BR-005 | Security audit remediation SLA adherence | Security audit reports and remediation tracker | Findings closed within SLA (≤ 7 days for critical) | Quarterly / As Needed |
| KPI-BR-006 | Data protection compliance (breach count, process documentation) | Incident tracker and process documentation coverage | 0 data breaches; 100% critical processes documented | Continuous / Annual Review |

---

## Index IDs
[KPI-FR-001](KPI.md)
[KPI-FR-002](KPI.md)

[KPI-NFR-001](KPI.md)
[KPI-NFR-002](KPI.md)
[KPI-NFR-003](KPI.md)
[KPI-NFR-004](KPI.md)
[KPI-NFR-005](KPI.md)

[KPI-BR-001](KPI.md)
[KPI-BR-002](KPI.md)
[KPI-BR-003](KPI.md)
[KPI-BR-004](KPI.md)
[KPI-BR-005](KPI.md)
[KPI-BR-006](KPI.md)

<!-- LINKS -->
[QC-001]: QualityCriteriaCode.md "Quality Criteria for Source Code (OOP) and MSSQL"
[OR-001]: RiskAnalysis.md "Risk analysis"
[LCR-001]: RiskAnalysis.md "Risk analysis"