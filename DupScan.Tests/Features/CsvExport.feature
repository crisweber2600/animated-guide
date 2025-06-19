Feature: CSV export
  Writing duplicate summaries to CSV files.

  Scenario: Export summary lines
    Given duplicate summary groups
      | Hash | Count | RecoverableBytes |
      | h1   | 3     | 30               |
      | h2   | 2     | 5                |
    When I export the summary
    Then the csv should contain
      | Hash | Count | RecoverableBytes |
      | h1   | 3     | 30               |
      | h2   | 2     | 5                |

