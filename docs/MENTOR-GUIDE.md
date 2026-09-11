# Mentor guide

Your copy. The junior's `README.md` deliberately contains **Stage 1 only**, so there's
no implementation material for her to read ahead into.

Hand over each stage as she finishes the one before it — by talking through it, or by
copying the stage into an issue, or however you prefer.

---

## The shape of the whole thing

Each stage exists to make the *next* one feel necessary. That's the design: nothing is
introduced as good practice, it's introduced because the previous stage got annoying
without it.

| Stage | She builds | The thing it teaches |
|---|---|---|
| 1 | Two interfaces | A contract says *what*, never *how* |
| 2 | One television | The compiler enforces the contract |
| 3 | A second television | **The payoff** — same contract, different innards |
| 4 | A remote control | Code written against a contract doesn't know or care which implementation it got |
| 5 | A factory | Something has to choose, and only one place should know the concrete types |
| 6 | A fake television | A fake is just another implementation — which is why interfaces make testing possible |
| 7 | HTTP endpoints | Wiring it into a real application |
| 8 | Integration tests | Unit tests prove logic; integration tests prove wiring |

**Stage 3 is the one that matters.** Everything before it can feel like paperwork.
If she leaves with one thing, it should be the moment in Stage 3 where the remote
works with a TV it was never written for.

---

## Stage 2 — One television

> Create `SonyTelevision` in `Implementations/`, implementing `ITelevision`.
> Make it actually work — track whether it's on, what channel it's on, what the
> volume is. Decide what happens if someone changes channel while it's off.

**What it teaches:** the contract has teeth. Delete a method from the class and the
build fails, with an error naming exactly what's missing.

**Worth doing out loud:** have her delete one method and read the compiler error.
`'SonyTelevision' does not implement interface member 'ITelevision.TurnOn()'` is
the contract enforcing itself, and seeing it once is worth a paragraph of explanation.

**Watch for:** public methods on `SonyTelevision` that aren't on the interface. Not
wrong, but worth asking about — anything reached through `ITelevision` can't see them,
which is a good way to surface what the contract is actually for.

---

## Stage 3 — A second television

> Create `SamsungTelevision`, also implementing `ITelevision`. Make it behave
> *differently* inside — a different volume range, a startup delay, channels
> numbered from 0 instead of 1. Same promises, different machine.

**What it teaches:** this is the entire point of interfaces, and it's the first stage
where that's visible.

**The demonstration:** once both exist, write one line somewhere that declares
`ITelevision tv = new SonyTelevision();`, then change it to `new SamsungTelevision();`.
Nothing else changes. Ask her *why* that works — the answer is that the variable's type
is the contract, not the class.

**Common wrong turn:** making the second TV a near-copy of the first. Push for a real
internal difference. If both work identically, the lesson doesn't land.

---

## Stage 4 — The remote control

> Create `BasicRemoteControl` implementing `IRemoteControl`. It takes an `ITelevision`
> in its constructor and holds onto it. Its job is to translate button presses into
> calls on whatever TV it was handed.

**What it teaches:** writing code against a contract instead of a concrete class.

**The key constraint:** `BasicRemoteControl` must not contain the words `Sony` or
`Samsung` anywhere. If it does, it has been written against an implementation and
the design has gone wrong. This is a good grep to run together.

**The question to ask:** "How many televisions does this remote work with?" The answer
is "all of them, including ones nobody has written yet" — and that's a genuinely
surprising thing the first time you see it.

**Vocabulary:** this is dependency injection. Passing a thing's dependencies in from
outside rather than letting it build its own. Worth naming once she's done it, not before.

---

## Stage 5 — The factory

> Something has to decide *which* television to build. Create `TelevisionFactory` in
> `Factories/` with a method that takes a brand name and returns an `ITelevision`.

**What it teaches:** concentrating knowledge of concrete types in one place.

**Motivate it properly, or it looks like ceremony.** The setup that makes a factory
obviously worth having: the brand comes from configuration, so you genuinely don't know
which TV you need until the program is running. `new SonyTelevision()` can't express
that. A factory can.

**The question to ask:** "We're about to add an LG television. Which files change?"
With a factory: one. Without: every place that ever constructed a TV. That's the
argument, and it's much better felt than told.

**Watch for:** a factory that returns `SonyTelevision` rather than `ITelevision`. It
compiles, and it quietly undoes the whole exercise — the caller is back to knowing
the concrete type.

---

## Stage 6 — Unit tests and a fake

> In `SystemDesign.UnitTests`, create `FakeTelevision` implementing `ITelevision`.
> It doesn't do anything real — it just records what it was asked to do. Then test
> `BasicRemoteControl` against it.

**What it teaches:** why any of this was worth the trouble.

This is where interfaces and testing connect, and it's the second big moment after
Stage 3. She wants to test that pressing volume-up asks the TV to turn the volume up.
She doesn't need a real TV for that — she needs something that can be asked. And she
already knows how to build one, because a fake is just another implementation.

**The realisation to steer toward:** "I've written three televisions now, and the
remote can't tell them apart." That sentence means she's understood it.

**The test to insist on:** have her break `BasicRemoteControl` on purpose and watch the
test go red. A test that passes whether or not the code works is worse than no test.
(There's a worked example of this in the `ClaudeAutomationDemo` repo, in `Switch.cs`.)

---

## Stage 7 — Over HTTP

> Register the factory and a remote in `Program.cs`, and add endpoints —
> `POST /tv/power`, `POST /tv/channel/{number}`, `GET /tv/status`.

**What it teaches:** how the pieces get assembled in a real application, and that
`Program.cs` is where the abstract wiring becomes concrete.

**Worth pointing out:** `Program.cs` and the factory are now the *only* files that
name a concrete television. Everything else in the codebase talks to `ITelevision`.
That's the shape of most well-arranged .NET applications.

---

## Stage 8 — Integration tests

> In `SystemDesign.IntegrationTests`, test the endpoints end-to-end using
> `WebApplicationFactory<Program>` — the pattern is already there in
> `ScaffoldingTests.cs`.

**What it teaches:** the difference between the two kinds of test, felt rather than defined.

| | Unit test (Stage 6) | Integration test (Stage 8) |
|---|---|---|
| What's real | Just the remote | The whole application |
| The TV is | A fake she wrote | A real one, chosen by the factory |
| Speed | Instant | Slower |
| Catches | Wrong logic | Wrong wiring |
| When it fails | A rule is wrong | Something isn't plugged in |

**The question that lands it:** "Which kind of test catches it if we forget to register
the factory in `Program.cs`?" Only the integration test. Every unit test still passes,
because unit tests build the remote by hand and never go near `Program.cs`. That's the
clearest statement of why you need both.

---

## Things to watch for throughout

- **Interfaces that grow.** If `ITelevision` reaches ten members, ask which of them a
  remote actually needs. Contracts should be small.
- **`new` outside the factory.** After Stage 5, a `new SonyTelevision()` anywhere else
  is worth a conversation.
- **Concrete types in signatures.** `void Connect(SonyTelevision tv)` throws away
  everything the interface bought.
- **Tests that can't fail.** Ask for a deliberate break at every testing stage.
- **"Why not just use the class directly?"** This is the right question and she should
  ask it. The honest answer for a one-TV program is: you shouldn't. Interfaces pay for
  themselves at the second implementation — which is exactly why Stage 3 exists.

---

## A note on ordering

The stages run interface → implementation. If she stalls in Stage 1 — staring at an
empty file, unsure what a TV "should" do — the other order works too: let her write
`SonyTelevision` as a plain class with no interface, then `SamsungTelevision`, then
point out how much of the two are the same shape and extract `ITelevision` from them.

Same destination. Some people need to see two concrete things before the abstraction
means anything. Switch if it's not landing; don't switch pre-emptively.
