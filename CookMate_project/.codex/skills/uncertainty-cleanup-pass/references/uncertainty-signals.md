# Uncertainty Signals

## Behavioral Uncertainty

- Implicit defaults that change behavior silently
- Branches with unclear or conflicting conditions
- Timezone/date handling without explicit standard
- Error handling that swallows failures or returns partial state

## Data and Contract Uncertainty

- DTO/domain mismatches
- Nullable handling that relies on assumptions
- Magic strings, enum drift, or implicit schema coupling
- Persistence model not aligned with migrations

## Architecture and Coupling Uncertainty

- Cross-layer dependencies that bypass boundaries
- Circular or hidden dependencies
- Services doing both orchestration and low-level data access
- Repeated logic that should be centralized

## Complexity Signals

- Long methods with multiple responsibilities
- Deep nesting and flag-driven behavior
- Duplicate validation or mapping logic
- Abstractions with a single implementation and no simplification gain

## Clean Simplification Heuristics

- Inline dead abstractions when they hide simple logic
- Extract helpers only when repeated behavior exists
- Prefer explicit data flow over implicit global state
- Prefer small cohesive types over broad utility classes
