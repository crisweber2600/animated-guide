Feature: Graph scanning
  Lists files from OneDrive for duplicate detection.

  Scenario: Scanning Graph
    Given Graph drive items
      | Id | Name | Hash | Size |
      | 1  | a.txt| q1   | 10   |
      | 2  | b.txt| q2   | 20   |
    When I scan Graph
    Then two FileItem objects should be returned from Graph
