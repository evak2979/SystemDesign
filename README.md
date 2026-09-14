# SystemDesign

A practice repo for learning interfaces, implementations, factories, and testing —
by building a television and a remote control.

If you've just been handed this: **read "Stage 1" below and stop there.** The later
stages exist, but doing them in order is the whole point.

---

## What you'll end up understanding

- **Interfaces** — how to describe what something does without saying how
- **Implementations** — how to fulfil that description, more than one way
- **Factories** — how to pick between those ways at the moment you need to
- **Unit tests** — proving one piece works on its own
- **Integration tests** — proving the pieces work together for real

You don't need to know any of that yet. That's what the stages are for.

---

## Setup

You need the .NET 10 SDK. Check with `dotnet --version`.

```bash
git clone https://github.com/evak2979/SystemDesign.git
cd SystemDesign
dotnet build SystemDesign.sln
dotnet test SystemDesign.sln
```

You should see **Build succeeded** and two test projects passing. Those passing tests
don't test anything real yet — they prove your machine can build and run this repo,
so that when something does go red later, you know it's your code and not your setup.

If that worked, you're ready.

---

## What's in here

```
SystemDesign.Api/            The application
  Interfaces/                Contracts — what things can do     <- Stage 1 lives here
  Implementations/           The classes that do the work       <- later
  Models/                    Plain data
  Factories/                 Chooses which implementation       <- later
SystemDesign.UnitTests/      Tests one piece in isolation
SystemDesign.IntegrationTests/  Tests the real thing over HTTP
```

Each folder has a short README saying what belongs in it.

---

# Stage 1 — Write the contracts

**Goal: describe a television and a remote control without building either one.**

Everything you write in this stage goes in `SystemDesign.Api/Interfaces/`.
You will not write a single line of code that *does* anything. That's not a
limitation of the exercise — it's the exercise.

### The idea

An interface is a **promise about behaviour**. It says "anything calling itself a
television can do these things", and it says nothing whatsoever about how.

The useful way to think about it: a remote control is designed by people who will
never see the inside of your TV. They don't know its brand, its age, how its speaker
works, or what happens electrically when you press the volume button. They only know
that a TV can *turn on*, *change channel*, and *change volume*. That shared
understanding — that list of things you can ask of any TV — is the contract.

The word "contract" is doing real work there. A contract binds both sides:

- Whoever **writes** a television must provide everything the contract lists.
- Whoever **uses** a television may rely on everything the contract lists, and may
  rely on *nothing else*.

### Your task

Create two files in `SystemDesign.Api/Interfaces/`:

1. **`ITelevision.cs`** — what any television can do
2. **`IRemoteControl.cs`** — what any remote control can do

By convention, C# interface names start with a capital `I`. The compiler doesn't
care; every C# developer who reads your code does.

Here's the shape, using a completely different example so it doesn't do your
thinking for you:

```csharp
namespace SystemDesign.Api.Interfaces;

public interface IKettle
{
    bool IsBoiling { get; }

    void Boil();
    void Stop();
}
```

Note what's there and what isn't. Method names and what they take and return —
yes. Any actual behaviour — no. There are no `{ }` blocks with code in them,
no fields, no `new`. Just the promise.

### Rules for this stage

1. **Interfaces only.** Nothing in `Implementations/`. Nothing in `Factories/`.
   If you catch yourself thinking "but how would it actually change the channel" —
   that's the right instinct at the wrong time. Write the thought down and move on.
2. **Keep them small.** Four or five members each is plenty. A real TV does
   hundreds of things; a contract that lists all of them is useless to everybody.
3. **Behaviour, not mechanism.** `TurnOn()` describes behaviour. `SendInfraredPulse()`
   describes mechanism — it has leaked *how* into a document that should only say *what*.

### Questions to sit with

Don't rush these. They're more valuable than the code you'll write.

- Should `ITelevision` have a `Brand` property? Is a brand something every TV must
  tell you about, or a detail of one particular TV?
- Does your remote need to know *which* TV it's holding, or just that it's holding
  one? What changes about your design depending on the answer?
- If you wrote `IRemoteControl` with a `PowerButton()` method — what actually happens
  when it's pressed? Is that the remote's job to know, or the TV's?
- A TV has a volume. Should the contract expose it as a number you can set to
  anything, or as `VolumeUp()` / `VolumeDown()`? What can go wrong with each?

### You're done when

- `dotnet build SystemDesign.sln` succeeds.
- `SystemDesign.Api/Interfaces/` contains exactly two files.
- `Implementations/` and `Factories/` are still empty.
- You can explain, out loud, why `ITelevision` doesn't mention Sony or Samsung.

A project containing nothing but interfaces compiles perfectly well. If it builds,
your contracts are valid — even though nothing can do anything yet.

---

## STOP HERE

Seriously. Stage 2 is about building a television, and it will be much more useful
to you after you've argued about the questions above with whoever gave you this repo.

Tell them you've finished Stage 1 and they'll take you through what's next.

---

## Stages

Each one is unlocked when you've finished the one before it. Don't read ahead — every
stage is written assuming you've felt the problem the previous one leaves you with.

| | | |
|---|---|---|
| **Stage 1** | Write the contracts | above |
| **Stage 2** | Build two televisions and a universal remote, test-first | [docs/STAGE-2.md](docs/STAGE-2.md) |
