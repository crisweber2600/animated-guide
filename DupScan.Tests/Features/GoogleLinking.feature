Feature: Google linking
  Replaces duplicate files with Google Drive shortcuts.

  Scenario: Linking duplicates
    Given Google duplicate files
      | Id | Path | Hash | Size |
      | 1  | /a   | h1   | 10   |
      | 2  | /b   | h1   | 20   |
    When I link duplicates on Google
    Then the Google drive service should link 1 to 2
