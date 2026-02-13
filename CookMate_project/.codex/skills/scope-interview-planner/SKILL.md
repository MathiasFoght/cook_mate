---
name: scope-interview-planner
description: Run a structured scope interview from a vague prompt and convert it into a clear implementation scope with constraints, acceptance criteria, and out-of-scope boundaries. Use when requests are ambiguous, broad, or likely to cause rework; when planning features/refactors; or when translating stakeholder asks into concrete engineering tasks.
---

# Scope Interview Planner

## Overview

Turn an unclear request into an actionable scope quickly.
Ask only the highest-leverage questions first, then produce a concrete scope decision and delivery plan.

## Workflow

1. Restate the request in one sentence and name the likely outcome.
2. Ask 3-7 targeted clarifying questions from `references/interview-checklist.md`.
3. Extract explicit constraints: deadline, team capacity, tech boundaries, quality bar.
4. Split work into:
- Must-have for first delivery
- Nice-to-have for later
- Explicitly out-of-scope
5. Define acceptance criteria that are testable and observable.
6. Identify top risks and unknowns that could block delivery.
7. Output a final scope using `references/scope-output-template.md`.

## Question Rules

- Prioritize questions that change implementation direction.
- Ask one question at a time when user attention is low.
- Stop asking when risk is low and scope is sufficiently bounded.
- State assumptions explicitly when answers are missing.
- Prefer closed choices if speed matters.

## Scope Rules

- Keep first version minimal and end-to-end.
- Protect reliability and security requirements even in narrow scope.
- Avoid hidden scope creep by naming non-goals clearly.
- Prefer reversible decisions when uncertainty is high.

## Output Contract

Always return:
- Problem statement
- Assumptions
- In-scope
- Out-of-scope
- Acceptance criteria
- Risks and mitigations
- Step-by-step implementation plan
- Open questions that still need answers

## References

- Use `references/interview-checklist.md` for question categories and fast prompts.
- Use `references/scope-output-template.md` for final scoped output format.
