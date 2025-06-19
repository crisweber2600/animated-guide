Feature: Graph linking
  Replaces duplicate files with Graph shortcuts.

  @integration
  Scenario: Linking duplicates
    Given duplicate files
      | Id | Path | Hash | Size |
      | 1  | /a   | h1   | 10   |
      | 2  | /b   | h1   | 20   |
    When I link duplicates on Graph
    Then the drive service should link 1 to 2
