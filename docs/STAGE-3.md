# Stage 3 — The requirements change

Stage 2 is merged. You built three televisions that behave differently behind one
contract, and a remote that drove a Sony it had never heard of. You also went back and
fixed two tests that were passing for the wrong reason, which is a harder habit to
learn than writing them in the first place.

Every test you've written is green. This stage is about what happens when that stops
being true.

---

## The change

The product owner has been talking to customers, and has changed their mind:

> 1. **Samsung's maximum volume is now 50**, not 100.
> 2. **LG now steps volume by 3**, not 2.
> 3. **Both Samsung and LG must allow volume changes while switched off.**
>
> Sony is fine as it is. Don't touch it.

That's the whole brief. It's a small change, and it's the kind you'll get every week of
your working life.

---

## Part A — Change the code, and only the code

Make those three changes in `SamsungTelevision` and `LGTelevision`.

> **Do not open a single test file yet.**

Not to peek, not to "just update that one number". The tests are going to tell you
something, and they can only do that if you haven't already decided what they'll say.

Then run:

```bash
dotnet test SystemDesign.sln
```

A lot of it will go red. That's expected, and it's the point of the stage.

---

## Part B — Read the red before you touch it

Your instinct will be to make it green again as fast as possible. Resist it for a bit.

That list of failures is a free report of every behaviour your change touched. Nobody
had to click through the app, nobody had to remember what depended on what. A past
version of you wrote those tests, and she's now telling you exactly what moved.

Go through the failures **one at a time**, and for each one answer a single question:

> **Is the test wrong now, or is my code wrong now?**

Those are completely different situations, and from the console they look identical: a
red line with an expected value and an actual value.

- **The test is out of date** when it describes a world that no longer exists. It was
  right yesterday; the requirement moved underneath it. You update the test.
- **The code is broken** when the test still describes what we want, and the code no
  longer does it. The test just caught a real bug. You fix the code and leave the test
  alone.

A good way to tell them apart: **read the test's name and its expected value, and ask
whether they're still true under the new requirements.** If they are, and it's still
failing, the problem isn't in the test.

### Write it down

Before changing anything, make a table in your PR description, one row per failing test:

| Test | Test wrong or code wrong? | Why, in one sentence |
|---|---|---|
| `SamsungTelevisionTests.VolumeUp_WhenAtMaxVolume_StopsIncreasingVolume` | ? | ? |
| … | | |

If you can't fill in the "why" for a row, you're not ready to change that test yet.
That's fine; it just means that one needs more looking at.

---

## Part C — Make it green again, deliberately

Now work through your table:

- For the **test-wrong** rows, update the test so it describes the new requirement.
  Fix the name too if it no longer says what the test does.
- For the **code-wrong** rows, fix the code. Don't touch the test.

Run the suite after each change, not at the end. One failure at a time.

### The trap

The fastest way to green is to copy the "actual" value from the failure message into
the "expected" value in the test. It works every time, and it's how people quietly
delete their own safety net.

**If you change an expected value, you must be able to say why the old one was wrong.**
"Because the test said so" doesn't count; that's the test grading itself.

---

## Part D — Check your net still catches things

Once everything is green:

1. Pick one guard in `LGTelevision` (a check on the maximum, the minimum, or whether
   it's on) and break it on purpose.
2. Run the tests. Something should go red.
3. Put it back.

If nothing goes red, you've found a behaviour with no test protecting it. That might be
because you "fixed" a test in Part C that was actually protecting it, or because it was
never tested. Either way, write the missing test (and watch it fail first).

---

## A question to answer in your PR

Your `UniversalRemoteControl` tests use a Sony. The requirements changed, a pile of
tests went red — and the remote's tests almost certainly didn't.

**Why not?** Is that good news, or is it hiding something? There's no single right
answer. Write down what you think; we'll pick it up in Stage 4.

---

## You're done when

- `dotnet test SystemDesign.sln` is green.
- Samsung maxes out at 50, LG steps by 3, and both change volume while off.
- Your PR description has the table from Part B, with every row filled in.
- For every test you changed, you can say in one sentence why the old version was wrong.
- For every code fix, you can point to the test that caught it.
- You broke a guard on purpose in Part D and saw a test catch it (or wrote the test that
  should have).
- You've answered the remote question.

Open a PR when you get there.

---

## One last thing

Every failure you saw in this stage was a message from a past version of you about
something you'd otherwise have shipped. The more tests you've written, the louder those
messages get. That's the deal: you pay a bit when you write them, and they pay you back
every time something changes.
