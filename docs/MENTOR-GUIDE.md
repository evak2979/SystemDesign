# Mentor guide

Your copy. The junior's `README.md` carries Stage 1 only; each later stage is handed
over as a separate file in `docs/` when she's ready for it.

| Stage | File | Status |
|---|---|---|
| 1 — Contracts | `README.md` | Merged in [#1](https://github.com/evak2979/SystemDesign/pull/1) |
| 2 — Two TVs and a universal remote, test-first | `docs/STAGE-2.md` | Ready to hand over |
| 3 — The factory | below | Not written up yet |
| 4 — Fakes and test doubles | below | Not written up yet |
| 5 — Over HTTP | below | Not written up yet |
| 6 — Integration tests | below | Not written up yet |

---

## The shape of the whole thing

Each stage exists to make the *next* one feel necessary. Nothing is introduced as good
practice — it's introduced because the previous stage got annoying without it.

| Stage | She builds | The thing it teaches |
|---|---|---|
| 1 | Two interfaces | A contract says *what*, never *how* |
| 2 | Samsung, LG, a universal remote — test-first | Same contract, different machines; and a test you haven't seen fail isn't a test |
| 3 | A factory | Something must choose, and only one place should know the concrete types |
| 4 | A fake television | A fake is just another implementation — which is why interfaces make testing possible |
| 5 | HTTP endpoints | Wiring it into a real application |
| 6 | Integration tests | Unit tests prove logic; integration tests prove wiring |

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

**Run the grep together.** `UniversalRemoteControl` must not contain "Samsung" or "LG":

```bash
grep -inw "samsung\|lg\|sony" SystemDesign.Api/Implementations/UniversalRemoteControl.cs
```

No output is the pass. If she's written `if (tv is SamsungTelevision)`, that's the
teachable moment of the whole stage — it means something belongs on the contract that
isn't there, and the fix is to change the interface, not to special-case the remote.

**Part E is the payoff.** She writes a third TV and the remote drives it unchanged. If
she leaves Stage 2 with one thing, it's that. Don't let it pass without naming it.

### On printing

Her brief says: print if you like, but assert on `IsOn` and `Volume`, not on console
output. Two reasons, if it comes up —

Her contract returns `void`, so asserting on text means either capturing the console or
changing the interface to return strings. The second undoes the Stage 1 lesson.

And console capture is genuinely unreliable here: `Console.Out` is global static state
and xUnit runs separate test classes in parallel by default, so `SamsungTelevisionTests`
and `LgTelevisionTests` would fight over it. Intermittent failures while she's learning
to trust tests is the worst possible outcome of this stage.

---

## Stage 3 — The factory

> Something has to decide *which* television to build. Create `TelevisionFactory` in
> `Factories/` with a method taking a brand name and returning an `ITelevision`.

**Motivate it properly or it looks like ceremony.** The setup that makes a factory
obviously worth having: the brand comes from configuration, so you genuinely don't know
which TV you need until the program is running. `new SamsungTelevision()` can't express
that. A factory can.

**Her own Stage 2 sets this up** — she'll have written `new SamsungTelevision()` by hand
in a dozen tests. The closing line of `STAGE-2.md` points at it deliberately.

**The question to ask:** "We're adding a Panasonic. Which files change?" With a factory:
one. Without: every place that ever constructed a TV.

**Watch for:** a factory returning `SamsungTelevision` rather than `ITelevision`. It
compiles, and it quietly undoes the whole exercise.

**Also worth asking:** what should the factory do with an unknown brand? Throw? Return
null? A default TV? No right answer — but it's a real design decision and she should
make it deliberately, with a test.

---

## Stage 4 — Fakes and test doubles

> In `SystemDesign.UnitTests`, create `FakeTelevision` implementing `ITelevision` that
> records what it was asked to do. Rewrite the remote's tests against it.

**This is where the awkwardness from Stage 2 pays off.** She tested the remote using a
real `SamsungTelevision`, which works but has a flaw worth drawing out: when a remote
test fails, was it the remote or the television? Testing one thing means controlling
everything around it.

**The realisation to steer toward:** "I've written four televisions now, and the remote
can't tell them apart." That sentence means she's understood the whole thing.

**The point that lands it:** a fake isn't a special testing construct. It's just another
implementation of a contract she already wrote. She's been able to build one since Stage 2.

**Insist on the deliberate break** — break `UniversalRemoteControl`, watch the test go
red. Same discipline as Stage 2, and it never stops being worth doing.

---

## Stage 5 — Over HTTP

> Register the factory and a remote in `Program.cs`, add endpoints — `POST /tv/power`,
> `POST /tv/volume/up`, `GET /tv/status`.

**Worth pointing out:** `Program.cs` and the factory are now the *only* files naming a
concrete television. Everything else talks to `ITelevision`. That's the shape of most
well-arranged .NET applications, and she's arrived at it by necessity rather than
being told.

---

## Stage 6 — Integration tests

> Test the endpoints end-to-end with `WebApplicationFactory<Program>` — the pattern is
> already working in `ScaffoldingTests.cs`.

| | Unit test (Stage 4) | Integration test (Stage 6) |
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
- **`new` outside the factory.** After Stage 3, worth a conversation every time.
- **Concrete types in signatures.** `void Connect(SamsungTelevision tv)` throws away
  everything the interface bought.
- **Tests that can't fail.** Ask for a deliberate break at every testing stage. There's
  a worked happy-path/sad-path example in the `ClaudeAutomationDemo` repo, `Switch.cs`.
- **"Why not just use the class directly?"** The right question, and she should ask it.
  The honest answer for a one-TV program is: you shouldn't. Interfaces pay for themselves
  at the second implementation — which is why Stage 2 builds two.
