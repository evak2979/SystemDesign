# Stage 4 — Stubs, mocks, and the battery

Stage 3 is merged. You read a wall of red one failure at a time, sorted "test wrong" from
"code wrong", and fixed the real bug without touching the test that caught it. That's the
habit most developers take years to build.

You also answered the remote question: the remote's tests stayed green because they only
ever use a Sony. This stage starts exactly there.

It has two halves, and they turn out to be the same lesson:

- **A–D: test the remote without a real television.** Then meet Moq, a library that does
  it for you.
- **E: give the remote a real battery.** No instructions for this one. Like the remote in
  Stage 2, the design is yours.

---

## Part A — What's wrong with testing the remote against a Sony?

Look at one of your remote tests:

```csharp
var tv = new SonyTelevision();
var sut = new UniversalRemoteControl(tv) { HasBattery = true };

sut.PressPower();
sut.PressVolumeUp();

Assert.Equal(25, tv.Volume);
```

Where does the `25` come from? Not from the remote. It's 20 (the Sony's starting
volume) plus 5 (the Sony's step size). This test is checking two things at once, and only
one of them is the remote.

Ask yourself:

- If the Sony's step changed to 4 tomorrow, would this test fail?
- Would the remote be broken?
- When this test goes red, how would you know which of the two to look at?

The remote's job isn't to make the volume 25. Its job is to **ask the television to turn
the volume up**, if it has a battery. What the television does with that request is the
television's business, and the television already has its own tests for it.

So the question this stage asks is: **how do you test the remote on its own?**

---

## Part B — Write a television that only takes notes

You already know how to write a television. You've written three.

> In `SystemDesign.UnitTests`, write a `StubTelevision` that implements `ITelevision`.
> It doesn't play anything, it doesn't have a volume step, it doesn't care about limits.
> It just **remembers what it was asked to do**.

How it remembers is up to you. A counter per method? A list of calls? Something else?
Pick whatever makes your tests easiest to read.

You'll also want a way for a test to decide whether the stub television starts **on or
off**, because `PressPower()` asks the television that question. This is a test helper,
so it's fine for it to have a setter — that's what it's for.

Two things to notice once it compiles:

1. **It lives in the test project, not the API.** No real code will ever use it.
2. **The remote accepted it without a single change.** Your remote has now driven four
   televisions, and it can't tell any of them apart. That's the interface from Stage 1,
   still paying you back.

---

## Part C — Retest the remote against your stub

Rewrite your `UniversalRemoteControlTests` so they use `StubTelevision` instead of
`SonyTelevision`. The assertions change shape. Instead of:

> "the Sony's volume is now 25"

you're now saying:

> "the remote asked the television to turn the volume up, exactly once"

That second sentence is about the remote and nothing else. It's what we actually want to
know.

Cases worth covering, one behaviour per test:

- Power pressed, television off, battery present → it asked the television to **turn on**.
- Power pressed, television on, battery present → it asked the television to **turn off**.
- Volume up / volume down, battery present → it asked for **exactly one** volume change.
- **Any button, no battery → it asked the television for nothing at all.** Not one call.

That last one is your own sad path from Stage 1, and with a stub it gets much stronger.
Before, you could only check the Sony's volume hadn't moved. Now you can check the remote
didn't even *try*.

Then the usual question: **delete the battery check from `PressVolumeUp` on purpose.**
Which test goes red? If none does, write the one that should have.

Keep your Sony tests? Delete them? Your call — but be ready to say why.

---

## Part D — Stub, mock, fake, and then Moq

### The words

People use these words loosely, so it's worth knowing the distinction exists:

- A **stub** stands in for something real and gives canned answers.
  *"Pretend the TV is on."*
- A **mock** also remembers how it was used, so the test can check that afterwards.
  *"Did the remote call `TurnOn` exactly once?"*
- A **fake** is a working but simplified version of the real thing.
  *An in-memory database instead of a real one.*

**Question for your PR:** your `StubTelevision` — is it really a stub? Look at what your
tests check, and decide which of the three words fits best.

All three are called **test doubles**, like a stunt double in a film: something that stands
in for the real thing so the scene can be shot safely.

### Now meet Moq

You've just written a test double by hand. In real projects, people usually let a library
generate them, and in .NET the most common one is **Moq**.

Add it to the test project:

```bash
dotnet add SystemDesign.UnitTests package Moq
```

Then rewrite **two** of your remote tests using Moq instead of your `StubTelevision`. You'll
need three ideas, and the reading below covers all of them:

- `new Mock<ITelevision>()` — make a test double for an interface
- `Setup(...)` and `Returns(...)` — give it canned answers (the *stub* part)
- `Verify(..., Times.Once)` — check how it was used (the *mock* part)

**Reading, in this order:**

1. **The ideas:** [Mocks Aren't Stubs — Martin Fowler](https://martinfowler.com/articles/mocksArentStubs.html).
   The classic article on this topic, and where the stub/mock distinction comes from.
   It's long, so read up to the section "Classical and Mockist Testing" and don't worry
   about the rest yet. The examples are in Java, but they read almost exactly like C#.
2. **Hands-on:** [How to use Moq for mocking objects with xUnit and .NET — Round The Code](https://www.roundthecode.com/dotnet-tutorials/moq-mocking-objects-xunit-dotnet).
   A step-by-step walkthrough of `Setup`, `Returns` and `Verify` with xUnit, the same
   test framework you're using.
3. **Reference, when you're stuck:** [Moq Quickstart](https://github.com/devlooped/moq/wiki/Quickstart).
   The official cheat sheet. Not one to read top to bottom — dip in when you need
   something.

**Then compare.** Put your hand-written version and your Moq version side by side, and
write a few sentences in your PR:

- What did Moq save you from writing?
- Which version would someone new to the codebase understand faster?
- Is there anything your hand-written stub made obvious that Moq hides?

There's no right answer. The point is that Moq isn't magic: it's generating the same kind
of class you just wrote by hand.

---

## Part E — The battery

This part is like the remote in Stage 2: you get the goal, not the design.

> **The remote's battery can be taken out, put in, and runs down as it's used.
> A remote with no battery, or a flat one, does nothing.**

That's the brief. Some of it is deliberately vague — how fast does it run down? What's
"flat"? Decide, and write your rules down in the PR description so we can talk about them.

### Things to judge your design against

- **Can you test "a flat battery does nothing"?** You'll need to. Whatever you design has
  to let a test set up a remote with a flat battery, without pressing a button a thousand
  times first.
- **Does it describe something real?** Right now `HasBattery` has a public setter. In real
  life, you can't flip a "has battery" switch on a remote. You can take the battery out or
  put one in. Does your design say what really happens?
- **Should `Battery` be an interface?** Don't decide by reflex. Ask what a battery actually
  *does*. If it only holds a charge level, is that behaviour or just data? Would there ever
  be a second kind of battery that behaves differently? Your answer either way is fine —
  your reasoning is what we'll talk about.

### Things to test

- No battery → no button does anything.
- A flat battery → no button does anything.
- A good battery → buttons work, and the battery runs down as you use them.
- Taking the battery out, or putting a new one in, behaves the way you'd expect.

These are sad paths again, and they're where the bugs live. Write each test before the
code, and watch it fail first.

---

## You're done when

- `dotnet test SystemDesign.sln` is green.
- `StubTelevision` lives in the test project, and your remote tests use it instead of a
  real television.
- Your remote tests check what the remote **asked for**, not what a particular television
  did.
- You deleted a battery check on purpose and watched a test catch it.
- Two remote tests are rewritten with Moq, and your PR compares them with the
  hand-written versions.
- Your PR says which word — stub, mock or fake — fits your `StubTelevision`, and why.
- The remote has a battery that can be taken out, put in, and runs down, and your PR
  explains your rules and whether `Battery` is an interface.
- Every test you wrote, you saw fail first.

Open a PR when you get there.

---

## One last thing

Count how many times you've written `new SonyTelevision()`, `new SamsungTelevision()` or
`new LGTelevision()` across your tests. Now imagine the program has to pick a brand
from a settings file, at runtime, without anyone typing `new` for a specific brand.

Something has to make that choice. That's Stage 5.
