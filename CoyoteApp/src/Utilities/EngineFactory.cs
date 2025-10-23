using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace CoyoteApp;

public static class EngineFactory
{
    // Initialize FileCount based on existing reports.
    private static int FileCount = InitializeFileCountFromExistingReports();
    private const string FILE_PREFIX = "Test_File_";

    public static TestingEngine GetDefaultTestEngine(Configuration conf, Func<Task> targetFunction, ITestOutputHelper logger)
    {
        var engine = TestingEngine.Create(conf, targetFunction);

        // --- Find or create the coyote_reports directory ---
        var reportDir = FindCoyoteReportsDirectory() ?? CreateFallbackReportsDirectory(logger);

        // --- Run the engine ---
        engine.Run();

        // --- Emit reports safely ---
        var fileName = $"{FILE_PREFIX}{Interlocked.Increment(ref FileCount)}";

        if (engine.TryEmitReports(reportDir, fileName, out var reportPaths))
        {
            logger.WriteLine($"Reports written to: {reportDir}");
            foreach (var path in reportPaths)
                logger.WriteLine($"  -> {Path.GetFileName(path)}");
        }
        else
        {
            logger.WriteLine("No reports generated.");
        }

        return engine;
    }

    /// <summary>
    /// Searches for the <c>CoyoteApp/CoyoteApp/coyote_reports</c> directory by walking up
    /// from the current working directory and, if not found, scanning the user's home folder.
    /// </summary>
    /// <returns>
    /// The absolute path to the discovered <c>coyote_reports</c> directory if found; otherwise <see langword="null"/>.
    /// </returns>
    private static string? FindCoyoteReportsDirectory()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (current != null)
        {
            var candidate = Path.Combine(current.FullName, "CoyoteApp", "CoyoteApp", "coyote_reports");
            if (Directory.Exists(candidate)) return candidate;

            current = current.Parent;
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var possible = Directory
                .GetDirectories(home, "coyote_reports", SearchOption.AllDirectories)
                .FirstOrDefault(p => p.Contains("CoyoteApp"));
        
        return possible;
    }

    /// <summary>
    /// Creates and returns a fallback reports directory under the system temporary path
    /// (e.g., <c>%TEMP%/CoyoteApp/CoyoteReports</c>), ensuring that it exists before returning.
    /// </summary>
    /// <returns>
    /// The absolute path to the newly created fallback reports directory.
    /// </returns>
    private static string CreateFallbackReportsDirectory(ITestOutputHelper logger)
    {
        var fallback = Path.Combine(Path.GetTempPath(), "CoyoteApp", "CoyoteReports");
        Directory.CreateDirectory(fallback);
        logger.WriteLine($"[WARN] Using fallback reports directory: {fallback}");
        return fallback;
    }

    /// <summary>
    /// Searches for the <c>coyote_reports</c> directory and scans for existing files
    /// matching the pattern <c>Test_File_&lt;number&gt;</c>. Determines the highest numeric
    /// suffix and initializes the static <see cref="FileCount"/> to continue numbering sequentially.
    /// </summary>
    /// <returns>
    /// The highest existing file number found, or <c>0</c> if no matching files exist.
    /// </returns>
    private static int InitializeFileCountFromExistingReports()
    {
        var reportDir = FindCoyoteReportsDirectory();
        // No existing coyote_reports directory found; starting from 0
        if (string.IsNullOrEmpty(reportDir) || !Directory.Exists(reportDir)) return 0;
        

        var files = Directory.GetFiles(reportDir, $@"{FILE_PREFIX}*", SearchOption.TopDirectoryOnly);
        // No existing report files found in coyote_reports; starting from 0.
        if (files.Length == 0) return 0;

        // Find current highest test file
        int maxNum = 0;
        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var match = Regex.Match(name, $@"{FILE_PREFIX}(\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var num))
            {
                if (num > maxNum)
                    maxNum = num;
            }
        }
        
        return maxNum;
    }
}
