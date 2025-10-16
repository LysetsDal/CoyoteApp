---
title: "Research Project: C# Thread Safety Testing Framework"
authors: "Asger Clement Nebelong Lysdahl", "Daniel Vikkelsø Held-Hansen"
---

# Research Project: C# Thread Safety Testing Framework


## Overview
This repository focuses on investigating and formalizing programming patterns that
ensure thread safety in C#. As the usage of concurrent programming becomes more 
common, it's crucial to address concurrency bugs efficiently. Leveraging the 
Coyote concurrency testing framework, we aim to provide developers with a set of
rules and automated tests to verify the thread-safety of their classes.

---

## Project Structure
- The `src/` directory contains subdirectories, each related to a specific concurrency issue. Each directory holds helper classes needed to provoke the concurrency bug.
- The `Tests/` directory contains all the test files (both Unit and Coyote tests).
- The `dll-dict.json` can be used to specify which assemblies to rewrite, and where to save the output of the `coyote rewrite` command [Documentation](https://microsoft.github.io/coyote/#concepts/binary-rewriting/#configuration).
- The `bin/` directory contains the .NET build artifacts, and the `rewritten/` folder, where the rewritten `.dll` files are saved.
- The `Utilities/Utils.cs` file holds various coyote test configuration.
---

## Test Structure

For each specific concurrency issue directory in the `src/` directory we have a similarly named Test file in `Tests/`.
Each test file has a normal xUnit test, annotated with `[Fact]`, and a coyote test annotated with `[Test]`.

### Example:

```csharp
public class TestExample
{
    /// <summary>
    /// Synchronous Unit test.
    /// </summary>
    [Fact]
    public async Task Test_SomeConcurrencyProperty()
    {
        // Perform some Task 
        var task1 = Task.Run(() => /* ... */));
        var task2 = Task.Run(() => /* ... */));

        // Await them
        await Task.WhenAll(task1, task2);
        
        // Assert on result
        Assert.Equal(expected, actual);
    }
    
    /// <summary>
    /// Concurrent Coyote test run with default Configuration. 
    /// </summary>
    [Fact]
    public async Task CoyoteTest_DataRace_Turnstile()
    {
        // Use coyote configuration
        var conf = Utils.GetDefaultConfiguration();
        
        // Create a coyote test engine with the configuration and method to test.
        var engine = TestingEngine.Create(conf, Test_SomeConcurrencyProperty);
        
        // Run the test.
        engine.Run();
        
        // Store result of test
        var reportText = engine.TestReport.GetText(conf, "[PROPERTY] "); ;
        
        // Assert no bugs found
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
}
```


## Running the project

To build the project:
```bash
dotnet clean ; dotnet build
```

To run the tests with coyote from the terminal, use these commands. This rewrites the project dll's to coyotes format.
```bash
coyote rewrite ./dll-dict.json
```

The rewritten binaries contains hooks and methods stub's that coyote can use to control the scheduling of events and provoke 'hard to trigger' concurrency errors.
Find a test annotated with `[Test]` (and not xUnit's `[Fact]`), and use the methods name as argument for the `--method` flag:
```bash
coyote test bin/rewritten/CoyoteApp.dll --method CoyoteTest_DataRace_Turnstile
```