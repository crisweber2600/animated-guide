Feature: CLI linking
  Replaces duplicates with shortcuts when using the --link option.

  @integration
  Scenario: Link duplicates via the CLI
    Given a Graph server expecting a shortcut from 1 to 2
    When I run the CLI with --link
    Then the Graph server should receive 2 requests
