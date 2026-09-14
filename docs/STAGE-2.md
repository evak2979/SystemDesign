# Stage 2 — Build two televisions and a universal remote, test-first

Stage 1 is merged. Your contracts are good: they're small, they describe behaviour
rather than mechanism, and `ITelevision` doesn't mention a brand anywhere. That last
one was the real question and you got it right.

Now you build things that fulfil those contracts — and you write the tests **first**.

---

## Part A — What test-first actually means

You're used to writing code and then, maybe, testing it. Turn that around:

1. **Red** — write a test for something that doesn't exist yet. Run it. Watch it fail.
2. **Green** — write the smallest amount of code that makes it pass. Not the best
   code. The smallest.
3. **Refactor** — now tidy it up, with a passing test to tell you if you broke it.

Then repeat, one small behaviour at a time.

### Why bother watching it fail?

This is the part people skip, and it's the part that matters.

**A test you have never seen fail is not a test.** It's a line of code that happens to
run. If you write the implementation first and then the test, and the test passes —
you've learned nothing. It might pass because the code works. It might pass because
the test doesn't actually check anything. You can't tell the two apart.

Seeing it go red first, for the reason you expected, is what earns the green.

### Your first red will be a compile error

In C#, writing a test for a class that doesn't exist yet means your test project won't
compile. That feels like a mistake. It isn't — **that's your red.** The compiler is
telling you the thing you're about to build doesn't exist, which is exactly what you
just asserted.

---

## Part B — The first cycle, in full

Do this one exactly as written, so the rhythm is in your hands before you're making
decisions as well.

### Red

In `SystemDesign.UnitTests`, create `SamsungTelevisionTests.cs`:

```csharp
using SystemDesign.Api.Implementations;
using Xunit;

namespace SystemDesign.UnitTests;

public class SamsungTelevisionTests
{
    [Fact]
    public void TurnOn_WhenOff_TurnsTheTelevisionOn()
    {
        var tv = new SamsungTelevision();

        tv.TurnOn();

        Assert.True(tv.IsOn);
    }
}
```

Run `dotnet test SystemDesign.sln`. It won't compile — `SamsungTelevision` doesn't
exist. **That's red.** Read the error; it names exactly what's missing.

### Green

Create `SystemDesign.Api/Implementations/SamsungTelevision.cs` and write the *smallest*
thing that passes. Genuinely the smallest — if `IsOn` returning a hardcoded `true` makes
it pass, that is a legitimate green.

That feels like cheating. It isn't, and it's worth understanding why: a hardcoded `true`
passing tells you your **test is too weak**. The fix is another test — one that this
implementation can't pass. Write `TurnOff_WhenOn_TurnsTheTelevisionOff` and the shortcut
dies immediately. This is the loop doing its job.

### Refactor

Tidy the class. Tests stay green, or you broke something.

---

## Part C — Two televisions that are actually different

Now build **Samsung** and **LG** in `Implementations/`, both implementing `ITelevision`.

The important part, and the reason this stage exists:

> **They must behave differently inside, while honouring the same contract.**

If Samsung and LG are copy-paste with the brand name swapped, you've written one
television twice and learned nothing. Give them genuinely different innards. Suggested:

| | Samsung | LG |
|---|---|---|
| Volume step | 1 at a time | 2 at a time |
| Maximum volume | 100 | 50 |
| Volume while off | Allowed, remembers it | Ignored — nothing happens |
| Volume on power-on | Starts at 10 | Starts at 0 |

Every one of those is checkable through `Volume` and `IsOn`, which your own interface
already exposes. Write the test first, every time.

Cases worth covering for each TV — write these as separate tests, one behaviour each:

- Turning on when already on (does anything break?)
- Volume up at maximum — does it stop, or keep climbing?
- Volume down at zero — can it go negative? *Should* it?
- Turning off and on again — what happens to the volume?

Those last ones are **sad paths** — the cases where something is at a limit or being
asked to do something odd. They're where bugs live.

### Printing

If you'd like to see something happen, put a `Console.WriteLine($"Samsung: power on")`
inside the methods. That's fine — it's a nice way to watch the thing work when you run it.

**But don't write tests that check what was printed.** Test `IsOn` and `Volume` instead.
Printed text is a side effect; state is the thing you actually promised in your contract.
Chasing console output in tests gets fragile fast, and you'd be testing the decoration
rather than the behaviour.

---

## Part D — The universal remote

Create `UniversalRemoteControl` in `Implementations/`, implementing `IRemoteControl`.

It takes an `ITelevision` in its constructor and keeps it:

```csharp
public class UniversalRemoteControl : IRemoteControl
{
    private readonly ITelevision _television;

    public UniversalRemoteControl(ITelevision television)
    {
        _television = television;
    }

    // ...
}
```

Passing a thing's dependencies in from outside instead of letting it build its own has
a name — **dependency injection**. That's all it is. You've just done it.

### The rule that makes it universal

> **`UniversalRemoteControl` must not contain the words `Samsung` or `LG` anywhere.**

Not in a type, not in an `if`, not in a comment. The moment it needs to know which
television it's holding, it isn't universal any more — it's two remotes wearing a coat.

If you find yourself wanting to write `if (tv is SamsungTelevision)`, stop. That itch
means something belongs on the contract that isn't there yet. Bring it to your mentor
rather than working around it.

### Things to work out

- `PressPower()` is a single button, but `ITelevision` has separate `TurnOn()` and
  `TurnOff()`. So the remote has to decide which to call. Where does it find out
  whether the TV is currently on? (You already put that on the contract.)
- `HasBattery` is on your interface. What should pressing a button do when it's `false`?
  You invented that rule in Stage 1 — now you have to honour it. Test it.

That battery case is a proper sad path, and it's a good one: **pressing a button on a
dead remote must not change the television.** Write that test.

### Testing the remote

Your remote tests need *a* television. For now, use a real `SamsungTelevision` — press
the button, assert the TV changed. That's the straightforward route, and it works.

Keep a note of anything that feels awkward about it. Stage 4 is about exactly that
awkwardness, and it'll mean more if you've felt it first.

---

## Part E — The finish

When both televisions and the remote are done and green, do this:

1. Write a **third** television. Call it `SonyTelevision`. Any behaviour you like.
2. Point your existing `UniversalRemoteControl` at it and write a test.
3. **Change nothing in `UniversalRemoteControl`.**

It will work. A remote you finished writing before Sony existed will drive a Sony.

That is the entire reason interfaces exist, and everything in Stage 1 was groundwork
for this moment. Sit with it — if it seems obvious now, that's the point.

---

## You're done when

- `dotnet test SystemDesign.sln` is green.
- Every test you wrote, you saw fail first.
- `Implementations/` has three televisions and one remote.
- Samsung and LG genuinely behave differently — someone reading the tests can tell
  them apart without looking at the classes.
- `UniversalRemoteControl` contains no brand name at all.
- You can explain why a hardcoded `return true` passing a test is useful information.

Open a PR when you get there.

---

## One last thing

You'll notice you've been writing `new SamsungTelevision()` by hand in every test, and
you'd have to do the same in the real application. Something, somewhere, has to decide
*which* television to build — and right now that decision would be scattered everywhere.

Keep that thought. It's Stage 3.
