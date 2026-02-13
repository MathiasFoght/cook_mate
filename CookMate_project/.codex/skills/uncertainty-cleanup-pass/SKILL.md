---
name: uncertainty-cleanup-pass
description: Run a deep code uncertainty pass to find fragile logic, ambiguous behavior, hidden coupling, and unnecessary complexity, then propose simpler clean abstractions without over-engineering. Use when preparing refactors, reviewing risky changes, reducing maintenance burden, or improving clarity in existing codebases.
---

# Uncertainty Cleanup Pass

## Overview

Inspect code for uncertainty and maintenance risk, then propose concrete simplifications.
Bias toward simple, local improvements that preserve behavior.

## Workflow

1. Map architecture boundaries and data flow before proposing changes.
2. Scan for uncertainty signals using `references/uncertainty-signals.md`.
3. Rank findings by severity and likelihood of production impact.
4. Propose smallest-change fixes first; avoid framework-level rewrites by default.
5. Validate proposal against tests, runtime behavior, and rollout risk.
6. Produce a prioritized cleanup plan with explicit tradeoffs.

## Analysis Rules

- Explain why each issue is uncertain or fragile.
- Prefer evidence from code paths and concrete examples.
- Separate bugs from style-only feedback.
- Recommend abstractions only when they remove repeated complexity.
- Avoid adding indirection that does not reduce cognitive load.

## Clean-Abstraction Guardrails

- Extract helper/abstraction only when logic repeats or domain language becomes clearer.
- Keep dependencies directional and explicit.
- Keep function/class responsibilities narrow and testable.
- Reject abstractions that solve hypothetical future cases only.

## Output Contract

Always return:
- Findings ordered by severity with file references
- Why it matters (failure mode or maintenance cost)
- Minimal fix option
- Optional stronger refactor option (only when justified)
- Test impact and migration risk
- Recommended execution order

## References

- Use `references/uncertainty-signals.md` to drive deep scan heuristics.
- Use `references/cleanup-report-template.md` to present findings consistently.
