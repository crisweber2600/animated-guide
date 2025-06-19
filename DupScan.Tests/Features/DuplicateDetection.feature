Feature: Duplicate detection
  Files with identical hashes should be grouped and ranked.


  Scenario: Detecting duplicates by hash
    Given the following files
      | Id | Path   | Hash | Size |
      | 1  | /a.txt | h1   | 10   |
      | 2  | /b.txt | h1   | 20   |
      | 3  | /c.txt | h2   | 5    |
    When I detect duplicates
    Then one group should contain 2 files with hash h1
    And the recoverable bytes should be 10

  Scenario: Duplicate groups ranked by recoverable bytes
    Given the following files
      | Id | Path | Hash | Size |
      | 1  | /a   | h1   | 10   |
      | 2  | /b   | h1   | 20   |
      | 3  | /c   | h2   | 30   |
      | 4  | /d   | h2   | 40   |
      | 5  | /e   | h3   | 5    |
      | 6  | /f   | h3   | 15   |
    When I detect duplicates
    Then groups should be ordered by recoverable bytes
      | Hash | Recoverable |
      | h2   | 30 |
      | h1   | 10 |
      | h3   | 5  |
