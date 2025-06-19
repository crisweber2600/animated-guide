Feature: Worker parallelism
  Ensures tasks execute concurrently.

  Scenario: Channel worker runs in parallel
    Given a worker with degree 2
    When I enqueue 3 tasks lasting 100ms
    Then execution time should be under 250ms
