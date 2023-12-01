namespace AdventOfCode.Runner;

using AdventOfCode.Common;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Threading.Tasks;

/// <summary>
/// Runs a particular day.
/// </summary>
public class RunDayCommand : AsyncCommand<RunDayCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "[year]")]
        public int Year { get; init; }

        [CommandArgument(0, "[day]")]
        public int Day { get; init; }

        [CommandOption("-p|--part <PART>")]
        [DefaultValue(0)]
        public int Part { get; init; }

        [CommandOption("-t|--test")]
        [DefaultValue(null)]
        public bool? Test { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        int year = settings.Year;
        int day = settings.Day;
        int part = settings.Part;
        bool test = false;

        if (settings.Year == 0)
        {
            year = AnsiConsole.Ask<int>("Year?", DateTime.Now.Year);
        }

        if (settings.Day == 0)
        {
            day = AnsiConsole.Ask<int>("Day?", DateTime.Now.Day);
        }

        if (settings.Part == 0)
        {
            part = AnsiConsole.Ask<int>("Part?", 1);
        }

        if (!settings.Test.HasValue)
        {
            test = AnsiConsole.Confirm("Test?", true);
        }

        AnsiConsole.MarkupLine($"[bold]Running day {day} of year {year} part {part}[/]");
        var formatter = new ConsoleResultFormatter();

        try
        {


            switch (year)
            {
                case 2022:
                    await Run2022(year, day, part, test, formatter);
                    break;
                case 2023:
                    await Run2023(year, day, part, test, formatter);
                    break;

                default:
                    AnsiConsole.MarkupLine($"[bold red]Year {year} is not implemented.[/]");
                    break;
            }

            return 0;
        }
        catch(Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
            return -1;
        }
    }

    private static async Task Run2022(int year, int day, int part, bool test, ConsoleResultFormatter formatter)
    {
        switch (day)
        {
            case 1:
                await Run<Year2022.Day01.DayRunner>(formatter, part, test);
                break;
            case 2:
                await Run<Year2022.Day02.DayRunner>(formatter, part, test);
                break;
            case 3:
                await Run<Year2022.Day03.DayRunner>(formatter, part, test);
                break;
            case 4:
                await Run<Year2022.Day04.DayRunner>(formatter, part, test);
                break;
            case 5:
                await Run<Year2022.Day05.DayRunner>(formatter, part, test);
                break;
            case 6:
                await Run<Year2022.Day06.DayRunner>(formatter, part, test);
                break;
            case 7:
                await Run<Year2022.Day07.DayRunner>(formatter, part, test);
                break;
            case 8:
                await Run<Year2022.Day08.DayRunner>(formatter, part, test);
                break;
            case 9:
                await Run<Year2022.Day09.DayRunner>(formatter, part, test);
                break;
            case 10:
                await Run<Year2022.Day10.DayRunner>(formatter, part, test);
                break;
            case 11:
                await Run<Year2022.Day11.DayRunner>(formatter, part, test);
                break;
            case 12:
                await Run<Year2022.Day12.DayRunner>(formatter, part, test);
                break;
            case 13:
                await Run<Year2022.Day13.DayRunner>(formatter, part, test);
                break;
            case 14:
                await Run<Year2022.Day14.DayRunner>(formatter, part, test);
                break;
            case 15:
                await Run<Year2022.Day15.DayRunner>(formatter, part, test);
                break;
            case 16:
                await Run<Year2022.Day16.DayRunner>(formatter, part, test);
                break;
            case 17:
                await Run<Year2022.Day17.DayRunner>(formatter, part, test);
                break;
            case 18:
                await Run<Year2022.Day18.DayRunner>(formatter, part, test);
                break;
            case 19:
                await Run<Year2022.Day19.DayRunner>(formatter, part, test);
                break;
            case 20:
                await Run<Year2022.Day20.DayRunner>(formatter, part, test);
                break;
            case 21:
                await Run<Year2022.Day21.DayRunner>(formatter, part, test);
                break;
            case 22:
                await Run<Year2022.Day22.DayRunner>(formatter, part, test);
                break;
            case 23:
                await Run<Year2022.Day23.DayRunner>(formatter, part, test);
                break;
            default:
                AnsiConsole.MarkupLine($"[bold red]Day {day} of year {year} is not implemented.[/]");
                break;
        }
    }

    private static async Task Run2023(int year, int day, int part, bool test, ConsoleResultFormatter formatter)
    {
        switch (day)
        {
            case 1:
                await Run<Year2023.Day01.DayRunner>(formatter, part, test);
                break;
            case 2:
                await Run<Year2023.Day02.DayRunner>(formatter, part, test);
                break;
            case 3:
                await Run<Year2023.Day03.DayRunner>(formatter, part, test);
                break;
            case 4:
                await Run<Year2023.Day04.DayRunner>(formatter, part, test);
                break;
            case 5:
                await Run<Year2023.Day05.DayRunner>(formatter, part, test);
                break;
            case 6:
                await Run<Year2023.Day06.DayRunner>(formatter, part, test);
                break;
            case 7:
                await Run<Year2023.Day07.DayRunner>(formatter, part, test);
                break;
            case 8:
                await Run<Year2023.Day08.DayRunner>(formatter, part, test);
                break;
            case 9:
                await Run<Year2023.Day09.DayRunner>(formatter, part, test);
                break;
            case 10:
                await Run<Year2023.Day10.DayRunner>(formatter, part, test);
                break;
            case 11:
                await Run<Year2023.Day11.DayRunner>(formatter, part, test);
                break;
            case 12:
                await Run<Year2023.Day12.DayRunner>(formatter, part, test);
                break;
            case 13:
                await Run<Year2023.Day13.DayRunner>(formatter, part, test);
                break;
            case 14:
                await Run<Year2023.Day14.DayRunner>(formatter, part, test);
                break;
            case 15:
                await Run<Year2023.Day15.DayRunner>(formatter, part, test);
                break;
            case 16:
                await Run<Year2023.Day16.DayRunner>(formatter, part, test);
                break;
            case 17:
                await Run<Year2023.Day17.DayRunner>(formatter, part, test);
                break;
            case 18:
                await Run<Year2023.Day18.DayRunner>(formatter, part, test);
                break;
            case 19:
                await Run<Year2023.Day19.DayRunner>(formatter, part, test);
                break;
            case 20:
                await Run<Year2023.Day20.DayRunner>(formatter, part, test);
                break;
            case 21:
                await Run<Year2023.Day21.DayRunner>(formatter, part, test);
                break;
            case 22:
                await Run<Year2023.Day22.DayRunner>(formatter, part, test);
                break;
            case 23:
                await Run<Year2023.Day23.DayRunner>(formatter, part, test);
                break;
            default:
                AnsiConsole.MarkupLine($"[bold red]Day {day} of year {year} is not implemented.[/]");
                break;
        }
    }

    private static async ValueTask Run<TRunner>(ConsoleResultFormatter formatter, int part, bool test)
        where TRunner : IDay<TRunner>
    {
        TRunner runner = await TRunner.Initialize(test);
        switch (part)
        {
            case 1:
                runner.SolvePart1(formatter);
                break;
            case 2:
                runner.SolvePart2(formatter);
                break;
            default:
                AnsiConsole.MarkupLine($"[bold red]Part {part} is not implemented.[/]");
                break;
        }
    }

    private class ConsoleResultFormatter : IResultFormatter
    {
        public void Format<T>(T result)
        {
            AnsiConsole.MarkupLine($"[bold green]Result:[/] {result}");
        }
    }
}
