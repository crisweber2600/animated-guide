Feature: Google scanning
  Lists files from Google Drive for duplicate detection.

  Scenario: Scanning Drive
    Given Google Drive files
      | Id | Name | Md5  | Size |
      | 1  | a.txt| h1   | 10   |
      | 2  | b.txt| h2   | 20   |
    When I scan Google Drive
    Then two FileItem objects should be returned
