namespace AdventOfCode.Common;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
/// Loads input for a given day.
/// </summary>
public static class InputReader
{
    const string SessionCookie = "<Your cookie here>";

    /// <summary>
    /// Gets the input as a string.
    /// </summary>
    /// <param name="day">The day for which to get the input.</param>
    /// <param name="year">The year for which to get the input.</param>
    /// <param name="test">Indicates whether to use the test data.</param>
    /// <returns>The input file as a single string.</returns>
    public static async Task<string> ReadFile(int year, int day, bool test)
    {
        string path = await EnsureInput(year, day, SessionCookie, test);
        return await File.ReadAllTextAsync(path);
    }

    /// <summary>
    /// Gets the input as an array of lines.
    /// </summary>
    /// <param name="day">The day for which to get the input.</param>
    /// <param name="year">The year for which to get the input.</param>
    /// <param name="test">Indicates whether to use the test data.</param>
    /// <returns>The input file as a an array of strings - one for each line.</returns>
    public static async Task<string[]> ReadLines(int year, int day, bool test)
    {
        string path = await EnsureInput(year, day, SessionCookie, test);
        return await File.ReadAllLinesAsync(path);
    }

    /// <summary>
    /// Ensures that the input file has been downloaded.
    /// </summary>
    /// <param name="day">The day for which to download input.</param>
    /// <param name="year">The year for which to download input.</param>
    /// <param name="sessionCookie">The session cookie for advent of code.</param>
    /// <returns>A task, which, when complete, indicates that the input is available.</returns>
    private static async ValueTask<string> EnsureInput(int year, int day, string sessionCookie, bool test)
    {
        string directory = $"input{Path.DirectorySeparatorChar}{year}{Path.DirectorySeparatorChar}{day:D2}"; 
        
        string path = test ? Path.Combine(directory, "test.txt") : Path.Combine(directory, "input.txt");

        if (!File.Exists(path))
        {
            if (test)
            {
                string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), path);
                string localAppData = Environment.GetEnvironmentVariable("LOCALAPPDATA")!;
                string vsCodeLocation = Path.Combine(localAppData, "Programs\\Microsoft VS Code\\Code.exe"); 
                var startInfo = new ProcessStartInfo
                {
                    FileName = $"\"{vsCodeLocation}\"",
                    UseShellExecute = true,
                    Arguments = absolutePath,
                };

                Process.Start(startInfo);
                
                throw new InvalidOperationException($"When running in test mode, you must provide test data at: {absolutePath}");
            }

            // Turn off default cookie handling
            using HttpClientHandler handler = new() { UseCookies = false };
            using HttpClient client = new(handler);
            // Add the session cookie
            client.DefaultRequestHeaders.Add("Cookie", $"session={sessionCookie}");
            using HttpResponseMessage response = await client.GetAsync($"https://adventofcode.com/{year}/day/{day}/input");

            if (response.IsSuccessStatusCode)
            {
                Directory.CreateDirectory(directory);
                using FileStream fileStream = File.Create(path);
                await response.Content.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
        }

        return path;
    }

}
