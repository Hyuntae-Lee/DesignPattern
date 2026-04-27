1. Start with Use Case / Scenario first
2. Add Sequence Diagrams
3. Define Interface Contracts
4. Use Test-Driven Anchors
5. Add Decision Records
   - ADR (Architecture Decision Records).
   - A small document that answers: “Why did we design it this way?”
   - Example
   <pre>
   +------------------------------------------------------------+
   | ADR-003: Use file-based export instead of DB               |
   |                                                            |
   | Status: Accepted                                           |
   |                                                            |
   | Context:                                                   |
   | - Large reports cause DB performance issues                |
   | - Customers require offline access                         |
   |                                                            |
   | Decision:                                                  |
   | - Export reports as files instead of storing in DB         |
   |                                                            |
   | Consequences:                                              |
   | + Faster export                                            |
   | + Easier distribution                                      |
   | - No transactional consistency                             |
   +------------------------------------------------------------+
   </pre>
   - Where you store them
   <pre>
   /docs/adr/
      ADR-001-authentication.md
      ADR-002-database-choice.md
   </pre>
6. Define Boundaries
   - [UI]
   - [Application Logic]
   - [Domain Logic]
   - [Infrastructure]
