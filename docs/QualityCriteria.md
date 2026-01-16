
# Quality Criteria 📋
This document outlines the quality criteria that should be met for our projects and deliverables.
Adhering to these criteria ensures that we maintain high standards and deliver value to our stakeholders.


## Table of Contents 🗂️

- [File Naming Conventions](#file-naming-conventions)
  - [Folder Structure](#folder-structure)
- [Foranalytics](#foranalytics)
  - [Business Model Canvas (BMC)](#business-model-canvas-bmc)
  - [Business Process Model and Notation (BPMN)](#business-process-model-and-notation-bpmn)
- [Artifact](#artifact)
  - [Usecase](#usecase)
  - [High Level Design (HLD)](#high-level-design-hld)
    - [Domain Models](#domain-models)
    - [Entity Relationship Diagram (ERD)](#entity-relationship-diagram-erd)
    - [Use Case Brief](#use-case-brief)
    - [Use Case Casual](#use-case-casual)
    - [System Sequence Diagram (SSD)](#system-sequence-diagram-ssd)
    - [Operation Contract (OC)](#operation-contract-oc)
  - [Low Level Design (LLD)](#low-level-design-lld)
    - [Sequence Diagram (SD)](#sequence-diagram-sd)
    - [Design Class Diagram (DCD)](#design-class-diagram-dcd)
- [Compliance](#compliance)

---

## File Naming Conventions 🏷️
To ensure consistency and easy identification of files, we follow these naming conventions:
- For markdown files, use the `.md` extension.
  - Use lowercase letters and hyphens to separate words (e.g., `project-plan.md`).
  - Avoid using spaces or special characters in file names.
- Mermaid diagram files should be written in markdown files.
- C# files should follow the class name with a `.cs` extension.

### Folder Structure 📁
Follow this folder structure for organizing files:
```plaintext
project-root/
│
├── docs/                       # Project documentation, quality criteria, use cases, diagrams, and guides
│   └── UseCases/               # Use case documentation and related artifacts
│       ├── UC-001/             # Use case specific folder
│       └── ...                 # Other use case folders
├── src/                        # Source code for all projects
│   ├── Portfolio.Domain/       # Domain models and business logic
│   ├── Portfolio.Web/          # Blazor WebAssembly client application
│   └── ...                     # Other projects (e.g., API, infrastructure)
│
├── tests/                      # Automated tests for all projects
│
├── .github/                    # GitHub workflows and configuration files
│
├── README.md                   # Project overview and getting started guide
├── LICENSE                     # License information
└── ...                         # Other root-level files (e.g., .gitignore, solution files)
```

---

## Foranalytics 📊

### Business Model Canvas (BMC) 🧩
Our BMC diagrams must adhere to the following criteria:
- Each BMC must have a unique version identifier and a documented change log.
- All BMC diagrams should be stored in the centralized repository in markdown format.
- BMCs must be reviewed and approved by relevant stakeholders before acceptance.
- The BMC should clearly define all nine building blocks:
  - Key Partners
  - Key Activities
  - Key Resources
  - Value Propositions
  - Customer Relationships
  - Channels
  - Customer Segments
  - Cost Structure
  - Revenue Streams
- Language should be clear, concise, and business-oriented.
- All assumptions and dependencies must be documented.
- Visuals and layout should be consistent and easy to understand.

### Business Process Model and Notation (BPMN) 🔄
Our BPMN diagrams must adhere to the following criteria:
- Each BPMN diagram must have a unique version identifier and a documented change log.
- All BPMN diagrams should be stored in the centralized repository in markdown format.
- BPMN diagrams must be reviewed and approved by relevant stakeholders before acceptance.
- Diagrams should accurately represent business processes, including all relevant actors, events, and flows.
- Notation and symbols must conform to the BPMN standard.
- Language should be clear, concise, and business-oriented.
- All assumptions and dependencies must be documented.
- Visuals and layout should be consistent and easy to understand.

---

## Artifact 📦
Our artifacts must adhere to the following criteria:
- Artifacts should be versioned and stored in a centralized repository.
- Maintain clear documentation for all artifacts, including usage instructions and dependencies.
- All artifacts should have a unique identifier for easy tracking and reference.

### Usecase 📝

Our usecase models must adhere to the following criteria:
- Each usecase must have a unique metadata identifier following the format UC-XXX (e.g., UC-001).
- Usecase documentation should include a clear title, description, and scope.
- All actors and their roles must be clearly defined.
- Usecases should be cross-referenced with related artifacts.
- Language should be clear, concise, and consistent.
- Usecases must be reviewed and approved by relevant stakeholders.
- All assumptions and dependencies must be documented.

### High Level Design (HLD) 🏗️

#### Domain Models 🏛️

Our domain models must adhere to the following criteria:
- Each domain model must have a unique metadata identifier following the specified format (e.g., UC-001-DM, DM-001).
- Domain models should accurately represent the business concepts and rules of the domain.
- All attributes and relationships must be clearly defined and named according to the domain language.
- Multiplicity and cardinality of relationships must be specified.
- Domain models should be easily understandable by all stakeholders.
- Models must be versioned and changes documented.
- Domain models should be cross-referenced with related artifacts (e.g., usecases).
- Language should be clear, consistent, and unambiguous.
- Domain models must be reviewed and approved by relevant stakeholders.

#### Entity Relationship Diagram (ERD) 🗃️

Our ERDs must adhere to the following criteria:
- Metadata identifiers should be unique and follow a consistent format across all ERDs.
  - For ERD *usecase id*-ERD: UC-001-ERD, UC-002-ERD and ect.
  - For project ERD ERD-*increadable number*: ERD-001, ERD-002 and ect.
  - Cross-reference to DM.
- ERDs should accurately represent the data model and relationships within the system.
- ERDs should be reviewed and approved by relevant stakeholders.

#### Use Case Brief 📄

Our use case briefs must adhere to the following criteria:
- Metadata identifiers should be unique and follow a consistent format across all briefs.
  - For usecase brief *usecase id*-UB: UC-001-UB, UC-002-UB and ect.
- Briefs should provide a clear and concise overview of the use case, including goals, actors, and scenarios.
- Use case briefs should be reviewed and approved by relevant stakeholders.
- Use case briefs only have success scenario.

#### Use Case Casual 📃

Our use case casuals must adhere to the following criteria:
- Metadata identifiers should be unique and follow a consistent format across all casuals.
  - For usecase casual *usecase id*-BC: UC-001-BC, UC-002-BC and ect.
- Casuals should detail the flow of events, including alternative and exception scenarios.
- Use case casuals should be reviewed and approved by relevant stakeholders.

#### System Sequence Diagram (SSD) 🔗

Our SSDs must adhere to the following criteria:
- Metadata identifiers should be unique and follow a consistent format across all SSDs.
  - For SSD *usecase id*-SSD: UC-001-SSD, UC-002-SSD and ect.

#### Operation Contract (OC) 📜

Our OCs must adhere to the following criteria:
- Metadata identifiers should be unique and follow a consistent format across all OCs.
  - For OC *usecase id*-OC: UC-001-OC, UC-002-OC and ect.
  - Cross-reference to SSD.
- For each operation in SSD, there should be a corresponding OC.
- For each operation, clearly define the inputs, outputs, preconditions, and postconditions.

### Low Level Design (LLD) 🧬

#### Sequence Diagram (SD) 🔁

Our SDs must adhere to the following criteria:
- Metadata identifiers should be unique and follow a consistent format across all SDs.
- For SD *usecase id*-SD: UC-001-SD, UC-002-SD and ect.
  - Cross-reference to OC, SSD.
- SDs should accurately represent the interactions between system components.
- SDs should be reviewed and approved by development team.
- Language should be English.

#### Design Class Diagram (DCD) 🧱

Our DCDs must adhere to the following criteria:
- Metadata identifiers should be unique and follow a consistent format across all DCDs.
  - For DCD *usecase id*-DCD: UC-001-DCD, UC-002-DCD and ect.
  - For project DCD DCD-*increadable number*: DCD-001, DCD-002 and ect.
  - Cross-reference to DM, OC.
- DCDs should accurately represent the design and architecture of the system.
- DCDs should be reviewed and approved by relevant stakeholders.

---

## Compliance ✅
- Adhere to relevant legal and regulatory requirements.
- Maintain proper documentation for audits and reviews.

By meeting these quality criteria, we can ensure the success of our projects and the satisfaction of our stakeholders.