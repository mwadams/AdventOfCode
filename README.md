If anyone is thinking about doing advent of code in C# this year, I've built a solution that has a command line tool and benchmarks.

If you log into the advent of code website near the end of November, you can use your browser tools to inspect the webpage and retrieve your session cookie. If you plumb it into this file, it will automatically download the day's input for you.

https://github.com/mwadams/AdventOfCode/blob/main/AdventOfCode.Common/InputReader.cs

(Don't check your session cookie in; I could not be bothered to bring all the config stuff in which is where it should go, really!)

You can supply command line parameters for the year, day, and test mode - or just run without params and it will ask for the values (which default to the current year, day, and test mode)

If you choose Test Mode (which is the default option) and there is no test file created, it will open VSCode with a test file for the chosen day.

Hopefully this will reduce the tedium of setting up a new day.
