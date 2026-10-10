# Mentor guide

Your copy. The junior's `README.md` carries Stage 1 only; each later stage is handed
over as a separate file in `docs/` when she's ready for it.

| Stage | File | Status |
|---|---|---|
| 1 — Contracts | `README.md` | Merged in [#1](https://github.com/evak2979/SystemDesign/pull/1) |
| 2 — Two televisions, and designing the remote | `docs/STAGE-2.md` | Merged in [#2](https://github.com/evak2979/SystemDesign/pull/2) |
| 3 — The requirements change | `docs/STAGE-3.md` | Ready to hand over |
| 4 — Stubs, mocks, and the battery | below | Sketched, brief not written yet |
| 5 — The factory | below | Sketched |
| 6 — Over HTTP | below | Sketched |
| 7 — Integration tests | below | Sketched |

---

## The shape of the whole thing

Each stage exists to make the *next* one feel necessary. Nothing is introduced as good
practice — it's introduced because the previous stage got annoying without it.

| Stage | She builds | The thing it teaches |
|---|---|---|
| 1 | Two interfaces | A contract says *what*, never *how* |
| 2 | Samsung, LG, Sony, and a remote she designs | One contract, many machines — and a test you haven't seen fail isn't a test |
| 3 | Nothing — she changes the televisions | A wall of red is information, not a disaster |
| 4 | A stub television, and a real battery | Who builds a dependency decides who can test it |
| 5 | A factory | Something must choose, and only one place should know the concrete types |
| 6 | HTTP endpoints | Wiring it into a real application |
| 7 | Integration tests | Unit tests prove logic; integration tests prove wiring |

---

## Where Stage 1 landed

Her interfaces:

```csharp
public interface ITelevision
{
    int Volume { get; }  bool IsOn { get; }
    void TurnOn(); void TurnOff(); void VolumeUp(); void VolumeDown();
}

public interface IRemoteControl
{
    bool HasBattery { get; }
    void PressPower(); void PressVolumeUp(); void PressVolumeDown();
}
```

Worth knowing before you run Stage 2, because three of her choices shape it:

- **`IsOn` and `Volume` are on the contract.** She's given herself observable state,
  which means every Stage 2 behaviour is assertable without any test gymnastics. This
  is why Stage 2 tests state rather than printed output.
- **`PressPower()` is a single toggle**, but the TV has separate `TurnOn()`/`TurnOff()`.
  The remote has to work out which to call — a small, genuine design problem, and the
  answer is already on the contract she wrote.
- **`HasBattery` is a sad path she invented herself.** A dead remote must not change the
  television. Make her test it; it's the best sad-path case in the exercise and it's hers.

---

## Stage 2 — Two televisions and a universal remote

Full brief in `docs/STAGE-2.md`. Notes for you:

**The two big ideas are red-green-refactor and one-contract-many-machines**, and they
reinforce each other: TDD forces her to state each behavioural difference between
Samsung and LG as an assertion before she builds it.

**Insist on the deliberate weak green.** The brief tells her a hardcoded `return true`
is a legitimate pass. Juniors hate this and skip it. It's the single clearest
demonstration that a passing test can be worthless, and it costs thirty seconds.

**Watch for Samsung and LG being the same class twice.** This is the main failure mode.
The brief suggests concrete differences (step size, maximum, whether volume works while
off) — hold her to them. Two identical implementations teach nothing.

### Part D — design it, don't teach it

**She is given no code for the remote.** The brief states the goal, states the one hard
rule, and asks her how she'd design it. That's deliberate: the battery in Stage 4 checks
whether she *generalises* the pattern, and that check is worthless if Part D handed her
the pattern to copy.

**The brand-name rule is what makes the open question safe.** It quietly rules out the
most tempting idea — a remote that builds its own television — because any television it
built would have to be a particular brand. She's left to discover that something has to
come from outside. The constraint teaches; you don't have to.

```bash
grep -inw "samsung\|lg\|sony" SystemDesign.Api/Implementations/UniversalRemoteControl.cs
```

No output is the pass. Run it together. If she's written `if (tv is SamsungTelevision)`,
that's the teachable moment of the whole stage — it means something belongs on the
contract that isn't there, and the fix is to change the interface, not to special-case
the remote.

**Name the pattern only at the end.** The brief does this: once her remote works, it
tells her that being handed a television from outside is called dependency injection.
Meet the problem, then the name — not the other way round.

**If she stalls**, don't give her the constructor. Ask instead: *"write me the first
line of a test that presses power and checks the television turned on."* She can't write
it without deciding where the television comes from. The test drags the design question
into the open, which is the honest reason test-first helps with design at all.

**Plausible answers, all fine:** constructor injection (most likely), a `PairWith(tv)`
method, or a television passed to each button press (clumsy, but she'll discover why).
What matters is that the television comes from outside and the remote never names a brand.

**Part E is the payoff.** She writes a third TV and the remote drives it unchanged. If
she leaves Stage 2 with one thing, it's that. Don't let it pass without naming it.

---

## Where Stage 2 landed

Merged in [#2](https://github.com/evak2979/SystemDesign/pull/2), after one round of review.
What's worth knowing going into Stage 3:

- **Constructor injection, arrived at unprompted.** `UniversalRemoteControl(ITelevision)`,
  no brand names anywhere, drives the Sony unchanged. Part E landed.
- **She fixed two tests that passed for the wrong reason.** Her LG "at zero" and "while
  off" tests were each covered by the other's guard. She now turns the TV on and raises
  the volume first, so each test isolates its own check.
- **She dropped `MaxVolume` from `ITelevision`** when asked what used it. It's now a
  `private const` in each class. Good instinct: the contract only grows for a consumer.
- **The limit checks still use `!=`, not `<`.** `if (Volume != MaxVolume) Volume += step`
  only works when the maximum is a multiple of the step. Deliberately left in: Stage 3 is
  built to catch it (see below). Don't mention it before she runs the tests.
- **Still open, for Stage 4:** `IsOn` has a public setter on every TV, and the remote's
  `HasBattery` is a public settable property that defaults to `false`. The remote tests
  lean on both. The stub television and the battery work will surface them naturally.
- **Small leftovers:** `VolumeUp_WhenOn_DecreasesVolumeByTwo` actually tests
  `VolumeDown`, and a few names say `DoNothing` for `DoesNothing`. Stage 3 asks her to
  fix names on any test she touches, which covers the first.

---

## Stage 3 — The requirements change

Full brief in `docs/STAGE-3.md`. The change she's given:

> 1. Samsung's maximum volume is now 50.
> 2. LG now steps volume by 3.
> 3. Both Samsung and LG must allow volume changes while switched off.
>
> Sony is unchanged.

**What it teaches:** what a failing test is *for*. Everything up to now has trained the
green. This stage trains the red.

### Why "LG steps by 3" and not "by 1"

The original sketch had LG stepping by 1. Against her actual code, that change can't
produce a real bug: every step of 1 lands exactly on 50, so `!=` still works and every red
test is simply out of date. The stage would teach "update the tests" and nothing else.

**Step 3 doesn't divide 50.** Going up, LG goes 48 → 51 → 54 → … and never stops. Her
existing `VolumeUp_WhenAtMaxVolume_StopsIncreasingVolume` presses up 26 times and expects
50. That expectation is *still correct* under the new requirements — the maximum didn't
change — so when it fails, the only honest reading is that the code is broken. That's the
row of the table below that the whole stage is built around, and it's her own test from
Stage 2 doing the catching.

### What she'll probably see

Assuming she makes the change the obvious way (change the step and the maximum, drop the
`IsOn` check from LG's volume methods, leave the `!=` alone), seven failures:

| Test | Why it's red | Verdict |
|---|---|---|
| Samsung `VolumeUp_WhenAtMaxVolume_StopsIncreasingVolume` | Expects 100; max is now 50 | Test out of date |
| LG `TurnOn_WhenAlreadyOn_RemainsOnAndVolumeUnaffected` | Expects 2; one step is now 3 | Test out of date |
| LG `VolumeUp_WhenOn_IncreasesVolumeByTwo` | Expects 2; now 3 (and the name is wrong) | Test out of date |
| LG `VolumeUp_WhenOn_DecreasesVolumeByTwo` | Expects 2; now 3 (misnamed since Stage 2) | Test out of date |
| LG `VolumeUp_WhenOff_DoNothing` | Volume now changes while off | Test out of date — the behaviour itself is gone |
| LG `VolumeDown_WhenOff_DoNothing` | Same | Test out of date |
| LG `VolumeUp_WhenAtMaxVolume_StopsIncreasingVolume` | Volume sails past 50 | **Code broken** |

(The exact count depends on how she edits. What matters is the mix.)

**The two "while off" tests are the interesting out-of-date ones.** It isn't a number to
update: the behaviour they protect no longer exists. The right move is to *replace* them
with tests for the new behaviour ("volume up while off changes the volume"), not to tweak
the expected value until they pass. If she edits `Assert.Equal(4, …)` to `Assert.Equal(7, …)`
and keeps the name `…_DoNothing`, that's the trap in miniature.

**Samsung's max test is the subtle one.** It presses up 90 times from 10 and expects 100.
She'll likely change the 100 to 50 and leave 90 presses. That works (it clamps), but ask
her whether 90 is still the number that expresses "one past the maximum". It isn't: that's
now 40. Tests should say what they mean.

### How to run it

1. **Make her run the suite before changing any test.** The failure list is the point —
   it's a precise, free report of everything that requirement touched. Have her read it
   out loud.
2. **One failure at a time, and the question for each is the same:** *is this test wrong
   now, or is my code wrong now?* The brief asks her to fill in a table in the PR
   description before changing anything. Hold her to it.
3. **Only then update the tests** — to match the new requirement, deliberately, one by
   one.

### The distinction this stage exists to teach

| The test says | Reality | What it means | What to do |
|---|---|---|---|
| Max volume is 100 | The requirement is now 50 | The test is out of date | Update the test — it's describing a world that no longer exists |
| Volume ignored while off | The requirement is now "allowed" | The test is out of date | Replace it with a test for the new behaviour |
| Volume stops at the maximum | It now climbs past it | **The code is broken** | Fix the code. The test just caught a real bug |

**If she spots the `!=` bug while making the change and fixes it before running the
tests,** that's a good outcome, not a spoiled one. Ask her how she'd have found it
otherwise, and whether a test would have caught it. The Part D exercise below still
gives her the "caught by my own test" moment.

**Worth saying out loud afterwards:** every one of those failures was a message from a
past version of herself about something she'd otherwise have shipped.

### The trap to watch for

**Updating a test until it passes without understanding why it failed.** It's fast, it
feels productive, and it is how people delete their own safety net. If she changes an
expected value and can't say in a sentence why the old one was wrong, she's guessing. The
most likely place for this: copying 78 (or whatever LG's overshoot lands on) into the LG
max test. If she does that, the table in her PR will say "test wrong" for a test whose name
is "stops increasing volume" — point at the name.

### Part D — the deliberate break

The brief has her break a guard on purpose once everything is green. **There's a gap
waiting to be found:** the same `!=` bug exists at the bottom. Stepping down by 3 from 50
goes 2 → −1 → −4 → … and her "at zero" test doesn't catch it, because it starts at 0 and
never steps across it. If she fixes the top with `<` but leaves `!= 0` at the bottom, all
tests still pass.

If she doesn't find it herself, ask: *"What's the lowest volume LG can reach from 50?"*
Then have her write the test, and watch it fail first.

### The remote question

The brief asks why the remote's tests didn't go red. They use a Sony, and Sony didn't
change. The good answer has two halves:

- **Good news:** the remote doesn't depend on how any particular TV behaves. A change to
  Samsung or LG can't break it. That's the interface doing its job.
- **Hiding something:** the remote tests pass or fail based on *Sony's* behaviour (they
  assert `25`, Sony's +5 step). If Sony's step changed, the remote tests would go red even
  though the remote is fine. She's testing two things at once.

The second half is the doorway to Stage 4. Don't walk her through it; let her answer
stand, and open Stage 4 with it.

---

## Stage 4 — Stubs, mocks, and the battery

Two things that turn out to be the same thing, which is why they share a stage.

### Part one — testing the remote without a television

In Stage 2 she tested the remote using a real `SamsungTelevision`. That works, and it
has a flaw worth drawing out: **when that test fails, was it the remote or the
television?** She's testing two things and only one of them is the subject.

> Write a `StubTelevision` in `SystemDesign.UnitTests` — a television that does nothing
> except record what it was asked to do. Retest the remote against it.

**The point that lands it:** a stub isn't a special testing construct or a library. It's
just another implementation of the contract she wrote in Stage 1. She's been able to
write one since Stage 2 without knowing it.

**Vocabulary, once she's built one** — the words get used loosely in the wild, and it's
worth her knowing the distinction exists rather than memorising it:

- A **stub** stands in for something and returns canned answers. *"Pretend the TV is on."*
- A **mock** additionally remembers how it was used, so the test can assert on that.
  *"Check the remote actually called TurnOn once."*
- A **fake** is a working but simplified implementation — an in-memory database, say.

Have her write both by hand before mentioning that libraries like Moq or NSubstitute
generate them. Hand-written first, tooling second — otherwise the library is magic.

**The realisation to steer toward:** *"I've written four televisions now, and the remote
can't tell them apart."* That sentence means she's understood the whole thing.

### Part two — the battery

This is the same lesson pointed at her own design, and it's the better diagnostic of the two.

> The remote's battery can be taken out, put in, and runs down as it's used. A remote
> with no battery, or a flat one, does nothing.

Deliberately underspecified — what she reaches for is the assessment.

**Her own Stage 1 sad path does the teaching.** She has to test "a flat battery does
nothing", and if the remote builds its own battery internally she *cannot arrange a flat
one*. The difficulty is the design talking. Let her hit it before you say anything.

| What she does | What it means |
|---|---|
| `new Battery()` inside the remote | She solved Stage 2's Part D by pattern-matching rather than understanding. The most common outcome, and the most useful to catch. |
| Battery passed in from outside, like the television | She generalised her own reasoning to a new problem. This is what the whole arc is built to produce. |
| An `InsertBattery(...)` method on `IRemoteControl` | Also good, arguably better — she's modelled the physical act and can swap at runtime. Ask why she chose it. |
| A setter that exists only so tests can drain the battery | She felt the pain and worked around it. That method is the design asking to be changed. |

**On "should Battery be an interface?" — don't pre-load the answer.** Telling her "yes"
undercuts the rule from Stage 2: an interface earns its place at the *second
implementation*. The honest criterion is whether a battery **does** anything —

- Only holds a charge level → it's data, a **model**. An `IBattery` with one
  implementation forever is exactly the ceremony this repo argues against.
- Drains as it's used, and an alkaline drains differently from a rechargeable → real
  behaviour, real second implementation, and the interface pays for itself.

Both answers can be right. The justification is the assessment, not the choice. Reaching
for `IBattery` reflexively because "interfaces are good practice" is the same mistake as
hardcoding, pointing the other way.

## Stage 5 — The factory

> Something has to decide *which* television to build. Create `TelevisionFactory` in
> `Factories/` with a method taking a brand name and returning an `ITelevision`.

**Motivate it properly or it looks like ceremony.** The setup that makes a factory
obviously worth having: the brand comes from configuration, so you genuinely don't know
which TV you need until the program is running. `new SamsungTelevision()` can't express
that. A factory can.

**Stage 2 sets this up** — she'll have written `new SamsungTelevision()` by hand
in a dozen tests. The closing line of `STAGE-2.md` points at it deliberately.

**The question to ask:** "We're adding a Panasonic. Which files change?" With a factory:
one. Without: every place that ever constructed a TV.

**Watch for:** a factory returning `SamsungTelevision` rather than `ITelevision`. It
compiles, and it quietly undoes the whole exercise.

**Also worth asking:** what should the factory do with an unknown brand? Throw? Return
null? A default TV? No right answer — but it's a real design decision and she should
make it deliberately, with a test.

---

## Stage 6 — Over HTTP

> Register the factory and a remote in `Program.cs`, add endpoints — `POST /tv/power`,
> `POST /tv/volume/up`, `GET /tv/status`.

**Worth pointing out:** `Program.cs` and the factory are now the *only* files naming a
concrete television. Everything else talks to `ITelevision`. That's the shape of most
well-arranged .NET applications, and she's arrived at it by necessity rather than
being told.

---

## Stage 7 — Integration tests

> Test the endpoints end-to-end with `WebApplicationFactory<Program>` — the pattern is
> already working in `ScaffoldingTests.cs`.

| | Unit test (Stage 4) | Integration test (Stage 7) |
|---|---|---|
| What's real | Just the remote | The whole application |
| The TV is | A fake she wrote | A real one, chosen by the factory |
| Speed | Instant | Slower |
| Catches | Wrong logic | Wrong wiring |

**The question that lands it:** "Which kind of test catches it if we forget to register
the factory in `Program.cs`?" Only the integration test — every unit test still passes,
because unit tests build the remote by hand and never go near `Program.cs`.

---

## Things to watch for throughout

- **Interfaces that grow.** If `ITelevision` reaches ten members, ask which of them a
  remote actually needs.
- **`new` outside the factory.** After Stage 5, worth a conversation every time.
- **Concrete types in signatures.** `void Connect(SamsungTelevision tv)` throws away
  everything the interface bought.
- **Tests that can't fail.** Ask for a deliberate break at every testing stage. There's
  a worked happy-path/sad-path example in the `ClaudeAutomationDemo` repo, `Switch.cs`.
- **"Why not just use the class directly?"** The right question, and she should ask it.
  The honest answer for a one-TV program is: you shouldn't. Interfaces pay for themselves
  at the second implementation — which is why Stage 2 builds two.
