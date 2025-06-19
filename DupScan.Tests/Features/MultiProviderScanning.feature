Feature: Multi-provider scanning
  Scanning Google Drive and Microsoft Graph concurrently.

  @integration
  Scenario: Scan providers in parallel
    Given Google files for multi scan
      | Id | Name | Md5 | Size |
      | 1  | g.txt | h1 | 10 |
    And Graph items for multi scan
      | Id | Name | Hash | Size |
      | 2  | m.txt | h2 | 20 |
    When I scan providers in parallel
    Then both providers should have been queried
