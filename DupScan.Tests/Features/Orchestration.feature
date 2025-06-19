Feature: Orchestration
  Aggregates multiple providers and detects duplicates.

  Scenario: Scanning Graph and Google
    Given Graph items for orchestration
      | Id | Name | Hash | Size |
      | 1  | a.txt | h1  | 10   |
    And Google files for orchestration
      | Id | Name | Hash | Size |
      | g1 | b.txt | h1 | 20   |
    When I orchestrate scanning
    Then the orchestrator result should contain 2 files with hash h1
