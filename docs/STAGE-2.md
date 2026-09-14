# Stage 2 — Two televisions, and a remote you design yourself

Stage 1 is merged. Your contracts are good: they're small, they describe behaviour
rather than mechanism, and `ITelevision` doesn't mention a brand anywhere. That last
one was the real question and you got it right.

This stage has two halves:

- **A–C: build two televisions, test-first.** Learn how a test gets hold of a real
  class and checks what it actually does.
- **D–E: design the universal remote.** No instructions for this one. That's the point.

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
        // Arrange
        var sut = new SamsungTelevision();

        // Act
        sut.TurnOn();

        // Assert
        Assert.True(sut.IsOn);
    }
}
```

Run `dotnet test SystemDesign.sln`. It won't compile — `SamsungTelevision` doesn't
exist. **That's red.** Read the error; it names exactly what's missing.

### What `sut` means, and why it isn't called `tv`

**SUT** stands for **System Under Test** — the one object this test is actually about.

Right now there's only one object in the test, so the name looks like fuss. It won't
stay that way. Soon a test will involve a remote *and* a television, and only one of
them is the thing being tested — the other is just there to make the test possible.
Naming the subject `sut` means anyone reading can see in one second which is which,
instead of working it out from the assertions.

It's a convention, not a rule. The compiler doesn't care. Use it anyway — it's common
in professional codebases, and it makes your tests readable to people who have never
seen your code.

### What a test actually is

Nothing magic is happening here. Look at what that test does:

1. It creates a real `SamsungTelevision` — the same class the application uses. Not a
   copy, not a special test version. The real one.
2. It calls a real method on it.
3. It checks a real property afterwards.

`[Fact]` marks a method as something the test runner should execute. `Assert.True`
throws an exception when what it's given is false, and a test that throws is a test that
failed. That is the entire mechanism.

A test is ordinary code that uses your class the way the rest of the program will, and
complains when it misbehaves. The three comments — **Arrange**, **Act**, **Assert** —
name the three things every test does: set up the world, do the one thing under test,
state what must now be true.

### Green

Create `SystemDesign.Api/Implementations/SamsungTelevision.cs` and write the *smallest*
thing that passes. Genuinely the smallest — if `IsOn` returning a hardcoded `true` makes
it pass, that is a legitimate green.

That feels like cheating. It isn't, and it's worth understanding why: a hardcoded `true`
passing tells you your **test is too weak**. The fix is another test — one this
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

Cases worth covering for each television — separate tests, one behaviour each:

- Turning on when already on (does anything break?)
- Volume up at maximum — does it stop, or keep climbing?
- Volume down at zero — can it go negative? *Should* it?
- Turning off and on again — what happens to the volume?

Those last ones are **sad paths** — where something is at a limit, or being asked to do
something odd. They're where bugs live.

### Printing

If you'd like to see something happen, put a `Console.WriteLine("Samsung: power on")`
inside the methods. That's fine — a nice way to watch the thing work when you run it.

**But don't write tests that check what was printed.** Test `IsOn` and `Volume` instead.
Printed text is a side effect; state is what you actually promised in your contract.
Chasing console output in tests gets fragile fast, and you'd be testing the decoration
rather than the behaviour.

---

## Part D — Design the universal remote

This part is different. Nobody is going to tell you what the code looks like.

> **We need a universal remote control. Design it.**

It implements `IRemoteControl`, it lives in `Implementations/`, and it has to work with
**any** television — including televisions nobody has written yet.

That's the brief. The design is yours.

### The one hard rule

> **`UniversalRemoteControl` must not contain the words `Samsung` or `LG` anywhere.**

Not in a type, not in an `if`, not in a comment. The moment it needs to know which
television it's holding, it isn't universal any more — it's two remotes wearing a coat.

This rule is the only thing constraining your design, and it's worth understanding why
it's so restrictive. It rules out the most obvious idea — having the remote make itself
a television — because any television it made would have to be a *particular brand*.

### The question you have to answer

**Where does the remote's television come from?**

Think it through properly before writing anything. There is more than one workable
answer, and a few that look workable until you try to use them.

Some things to judge your ideas against:

- **Can you test it?** You'll want a test that presses a button and checks the television
  changed. Whatever you design has to let a test decide which television is involved.
- **Does it survive a new brand?** A television invented next year, by someone who has
  never seen your remote — does your design still work, with no changes to the remote?
- **Can one remote drive a different television later?** Should it be able to? That's a
  design decision, not a fact — but decide it deliberately rather than by accident.
- **Does the remote need to know anything about the television beyond the contract?**
  If yes, that's worth a conversation with your mentor before you build it.

### Things to work out along the way

- `PressPower()` is a single button, but `ITelevision` has separate `TurnOn()` and
  `TurnOff()`. So the remote has to decide which to call. Where does it find out whether
  the television is currently on? (You already put that on the contract.)
- `HasBattery` is on your interface. What should pressing a button do when it's `false`?
  You invented that rule in Stage 1 — now you have to honour it. Test it.

That battery case is a proper sad path, and a good one: **pressing a button on a dead
remote must not change the television.** Write that test.

---

## Part E — Prove it's universal

A design isn't universal because you named the class `UniversalRemoteControl`. Prove it:

1. Write a **third** television. Call it `SonyTelevision`. Any behaviour you like.
2. Point your existing remote at it and write a test.
3. **Change nothing in `UniversalRemoteControl`.**

It will work. A remote you finished writing before Sony existed will drive a Sony.

That is the entire reason interfaces exist, and everything in Stage 1 was groundwork for
this moment. Sit with it — if it seems obvious now, that's the point.

### Then go and look at what you built

Look at how the television gets into the remote.

If your answer was "it's handed one, from outside" — that has a name. It's called
**dependency injection**, it's one of the most common patterns in professional .NET code,
and you arrived at it on your own because the constraints left nowhere else to go. That's
the honest way to learn a pattern: meet the problem first, then the name.

If you ended up somewhere else, bring it to your mentor. Not because it's wrong — because
the reasoning is the interesting part.

---

## You're done when

- `dotnet test SystemDesign.sln` is green.
- Every test you wrote, you saw fail first.
- `Implementations/` has three televisions and one remote.
- Samsung and LG genuinely behave differently — someone reading the tests can tell them
  apart without looking at the classes.
- `UniversalRemoteControl` contains no brand name at all, and drives the Sony it was
  never written for.
- You can explain what `sut` stands for and why a test bothers naming one.
- You can explain why a hardcoded `return true` passing a test is useful information.

Open a PR when you get there.

---

## One last thing

Every test you've written now passes. That's a nice feeling, and it's also the least
informative state a test suite is ever in — all it tells you is that nothing has changed
since you last looked.

In Stage 3, the requirements change, and a lot of that green goes red at once. What you
do next is the actual skill.
