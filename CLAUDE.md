# CLAUDE.md

A teaching repo. Someone is learning interfaces, implementations, factories, and
testing here by building a television and a remote control.

## Build & test
- `dotnet build SystemDesign.sln`
- `dotnet test SystemDesign.sln`
- `dotnet format` before committing

## Layout
- `SystemDesign.Api/Interfaces/` — contracts only; no logic, no fields
- `SystemDesign.Api/Implementations/` — classes fulfilling those contracts
- `SystemDesign.Api/Models/` — plain data
- `SystemDesign.Api/Factories/` — the only place allowed to pick a concrete type
- `SystemDesign.UnitTests/` — one piece at a time, dependencies faked
- `SystemDesign.IntegrationTests/` — the real app over HTTP via `WebApplicationFactory<Program>`

## IMPORTANT — this is an exercise, not a codebase
The empty folders are empty **on purpose**. A learner is meant to fill them in,
stage by stage, following `docs/MENTOR-GUIDE.md`.

If you are asked to "finish" or "implement" something here, check what stage the
learner is on before writing code for them. Explaining and reviewing is usually
what's wanted; writing the implementation usually isn't.

## Conventions
- Interfaces are named `IThing` and live in `Interfaces/`
- Code outside `Factories/` and `Program.cs` should depend on interfaces, never on
  a concrete implementation
- Tests are named `Method_Scenario_ExpectedResult`
