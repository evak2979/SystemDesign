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
- **Does it survive a new brand?** A television invented next year, by someone who's
  never seen your remote — does your design still work, with no changes to the remote?
- **Can one remote drive a different television later?** Should it be able to? That's
  a design decision, not a fact — but decide it deliberately rather than by accident.
- **Does the remote need to know anything about the television beyond the contract?**
  If yes, that's worth a conversation with your mentor before you build it.

### Let the test tell you

Here's the useful trick, and it's the real reason test-first helps with design rather
than just catching bugs:

**Write the test before you write the class.** In that test you'll have to create a
`UniversalRemoteControl` and somehow arrange for it to have a television. The moment
you type that line, you're designing — because a test is the first thing that ever has
to *use* what you built.

If the test is awkward to write, your design is awkward to use. You'll have found that
out in thirty seconds instead of a fortnight.

### Things to work out along the way

- `PressPower()` is a single button, but `ITelevision` has separate `TurnOn()` and
  `TurnOff()`. So the remote has to decide which to call. Where does it find out whether
  the television is currently on? (You already put that on the contract.)
- `HasBattery` is on your interface. What should pressing a button do when it's `false`?
  You invented that rule in Stage 1 — now you have to honour it. Test it.

That battery case is a proper sad path, and a good one: **pressing a button on a dead
remote must not change the television.** Write that test.

### Testing the remote

Your remote tests need *a* television. For now, use a real `SamsungTelevision` — press
the button, assert the television changed. That's the straightforward route and it works.

Keep a note of anything that feels awkward about it. Stage 4 is about exactly that
awkwardness, and it'll mean more if you've felt it first.

### When it's working

Go and look at what you ended up with, and at how the television gets into the remote.

If your answer was "it's handed one, from outside" — that has a name. It's called
**dependency injection**, it is one of the most common patterns in professional .NET
code, and you just arrived at it on your own because the constraints left nowhere else
to go. That's the honest way to learn a pattern: meet the problem first, then the name.

If you ended up somewhere else, bring it to your mentor before Part E. Not because it's
wrong — because the reasoning is the interesting part.

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

## Part F — Someone wants to change the battery

Your remote works. Now the requirements change, which is what requirements do.

> **The remote's battery can be taken out, put in, and runs down as it's used.**
> A remote with no battery in it does nothing. A remote whose battery is flat
> does nothing either.

That's the whole brief. How you build it is the exercise.

### The one rule

**Write the test first.** As always — but here it matters more than usual, because
the test is what will tell you whether your design is any good.

Start with this one, in words: *"a remote with a flat battery does not change the
television."* You already wrote something close to it in Part D, using `HasBattery`.

Now try to write it again, for real. Somewhere in that test you will need a flat
battery. **Pay close attention to how easy or hard that is to arrange.** If you find
yourself unable to set up the situation you want to test, that is not a problem with
the test. It is the design telling you something, and the whole point of this part is
to hear it.

### Questions to answer before you write the code

Write your answers down — they matter more than the code, and your mentor will ask.

1. **Where does the battery come from?** You already solved this exact problem once
   in Part D, for the television. Does the same answer apply here? Why, or why not?
2. **Should `Battery` be an interface, or an ordinary class?** Be careful — this is
   not automatically "interface". Use the test you learned in Part C:

   > An interface earns its place when there is more than one kind of the thing,
   > behaving differently.

   So: is there a second kind of battery? Does a battery *do* anything, or does it
   just hold a number? If it only holds a charge level and nothing else, it might
   be a **model**, not an interface — and wrapping it in one would be exactly the
   ceremony you were warned about.

   Either answer can be right. An answer you can't justify can't.
3. **Does `IRemoteControl` need to change?** Is "putting a battery in" something you
   can do to *any* remote control — in which case it belongs on the contract — or is
   it something you do once when the remote is built? Both are defensible designs,
   and they lead to different code.
4. **What does a flat battery do to `HasBattery`?** That property is already on your
   contract. Does it still mean the same thing now that batteries run down?

### Things that should make you suspicious

- If `UniversalRemoteControl` contains `new Battery()`, ask yourself how a test is
  supposed to make that battery go flat.
- If your answer involves adding a method whose only purpose is to let a test change
  something, stop. Tests shouldn't need special access. Needing it means the thing
  should have come from outside in the first place.
- If the remote now has *two* things coming in from outside, that's not a problem —
  that's normal, and it has a name you already met in Part D.

### You're done with this part when

- A flat battery genuinely stops the remote working, proven by a test.
- Setting up a flat battery in a test is *easy*, and doesn't require any method that
  exists only for testing.
- You can explain your answer to question 2 without using the word "best practice".

---

## You're done when

- `dotnet test SystemDesign.sln` is green.
- Every test you wrote, you saw fail first.
- `Implementations/` has three televisions and one remote.
- Samsung and LG genuinely behave differently — someone reading the tests can tell
  them apart without looking at the classes.
- `UniversalRemoteControl` contains no brand name at all.
- A flat battery stops the remote working, and that's covered by a test that was
  straightforward to set up.
- You can explain why a hardcoded `return true` passing a test is useful information.
- You can justify whether the battery is an interface or a plain class — either way.

Open a PR when you get there.

---

## One last thing

You'll notice you've been writing `new SamsungTelevision()` by hand in every test, and
you'd have to do the same in the real application. Something, somewhere, has to decide
*which* television to build — and right now that decision would be scattered everywhere.

Keep that thought. It's Stage 3.
