var builder = WebApplication.CreateBuilder(args);

// Stage 7 is where you register your own services here.

var app = builder.Build();

// A tiny endpoint so the integration test project has something real to call
// before you've written any of your own. Leave it — it's the smoke test that
// proves the plumbing works.
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

// Lets the integration test project start this app in-memory.
// You don't need to understand this yet — Stage 8 explains it.
public partial class Program;
