Feature: Duplicate detection
  Files with identical hashes should be grouped and ranked.

  @ignore
  Scenario: Detecting duplicates by hash
    Given the following files
      | Id | Path   | Hash | Size |
      | 1  | /a.txt | h1   | 10   |
      | 2  | /b.txt | h1   | 20   |
      | 3  | /c.txt | h2   | 5    |
    When I detect duplicates
    Then one group should contain 2 files with hash "h1"
    And the recoverable bytes should be 10
