namespace AdventOfCode.Runner.Year2023.Day07
{
    using AdventOfCode.Common;
    using System.Threading.Tasks;

    public class DayRunner : IDay<DayRunner>
    {
        private readonly string[] lines;

        public DayRunner(string[] lines)
        {
            this.lines = lines;
        }

        public static async ValueTask<DayRunner> Initialize(bool test = false)
        {
            var lines = await InputReader.ReadLines(2023, 7, test);
            return new(lines);
        }

        public void SolvePart1<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            int result = 0;

            Span<(int Score, int Index)> scores = stackalloc (int Score, int Index)[lines.Length];

            int count = 0;
            foreach (var line in lines)
            {
                ReadOnlySpan<char> lineSpan = line.AsSpan();
                int index = count;
                scores[count++] = (ScoreHand(lineSpan[..5]), index);
            }

            scores.Sort(SortScores);

            for (int i = 0; i < scores.Length; ++i)
            {
                int winnings = int.Parse(lines[scores[i].Index].AsSpan()[6..]);
                result += (i + 1) * winnings;
            }

            formatter.Format(result);
        }

        public void SolvePart2<TFormatter>(TFormatter formatter)
            where TFormatter : IResultFormatter
        {
            int result = 0;

            Span<(int Score, int Index)> scores = stackalloc (int Score, int Index)[lines.Length];

            int count = 0;
            foreach (var line in lines)
            {
                ReadOnlySpan<char> lineSpan = line.AsSpan()[..5];
                int index = count;
                // Count the jokers
                int jokerCount = lineSpan.Count('J');
                // Start with the first character larger than a joker
                int nonJokerIndex = lineSpan.IndexOfAnyExcept('J');
                if (nonJokerIndex == -1)
                {
                    // 5 of a kind Jokers
                    scores[count++] = (7, index);
                }
                else
                {
                    scores[count++] = (ScoreHandForJoker(lineSpan[nonJokerIndex..], jokerCount), index);
                }
            }

            scores.Sort(SortScoresForJoker);

            for (int i = 0; i < scores.Length; ++i)
            {
                int winnings = int.Parse(lines[scores[i].Index].AsSpan()[6..]);
                result += (i + 1) * winnings;
            }

            formatter.Format(result);
        }


        private int SortScores((int Score, int Index) x, (int Score, int Index) y)
        {
            int comparison = x.Score.CompareTo(y.Score);
            if (comparison != 0)
            {
                return comparison;
            }

            ReadOnlySpan<char> x1 = lines[x.Index].AsSpan()[..5];
            ReadOnlySpan<char> y1 = lines[y.Index].AsSpan()[..5];
            for (int i = 0; i < 5; ++i)
            {
                int compare = ScoreCard(x1[i]).CompareTo(ScoreCard(y1[i]));
                if (compare != 0)
                {
                    return compare;
                }
            }

            // This indicates a duplicate card
            return 0;
        }

        private int SortScoresForJoker((int Score, int Index) x, (int Score, int Index) y)
        {
            int comparison = x.Score.CompareTo(y.Score);
            if (comparison != 0)
            {
                return comparison;
            }

            ReadOnlySpan<char> x1 = lines[x.Index].AsSpan()[..5];
            ReadOnlySpan<char> y1 = lines[y.Index].AsSpan()[..5];
            for (int i = 0; i < 5; ++i)
            {
                int compare = ScoreCardForJoker(x1[i]).CompareTo(ScoreCardForJoker(y1[i]));
                if (compare != 0)
                {
                    return compare;
                }
            }

            // This indicates a duplicate card
            return 0;
        }

        private static int ScoreHandForJoker(ReadOnlySpan<char> hand, int jokerCount)
        {
            int count = hand.Count(hand[0]) + jokerCount;
            if (count == 5)
            {
                // 5 of a kind
                return 7;
            }

            if (count == 4)
            {
                // Four of a kind
                return 6;
            }

            if (count == 3)
            {
                if (hand.Length > 3)
                {
                    int notItIndex = hand[1..].IndexOfAnyExcept(hand[0], 'J') + 1;
                    if (hand[1..].Count(hand[notItIndex]) == 2)
                    {
                        if (jokerCount == 2)
                        {
                            // Four of a kind with the new value
                            return 6;
                        }

                        // Full House
                        return 5;
                    }
                }

                // Three of a kind
                return 4;
            }

            if (count == 2)
            {
                if (hand.Length > 3)
                {
                    int firstNotItIndex = hand[1..].IndexOfAnyExcept(hand[0], 'J') + 1;
                    int secondCount = hand[1..].Count(hand[firstNotItIndex]);
                    if (secondCount == 3)
                    {
                        if (jokerCount == 1)
                        {
                            // Four of a kind
                            return 6;
                        }

                        // Full House
                        return 5;
                    }
                    else if (secondCount == 2)
                    {
                        if (jokerCount == 1)
                        {
                            // Three of a kind with the new value
                            return 4;
                        }

                        // Two pair
                        return 3;
                    }

                    if (hand.Length > firstNotItIndex + 1)
                    {
                        int secondNotItIndex = hand[(firstNotItIndex + 1)..].IndexOfAnyExcept(hand[0], hand[firstNotItIndex], 'J') + (firstNotItIndex + 1);
                        if (hand[(firstNotItIndex + 1)..].Count(hand[secondNotItIndex]) == 2)
                        {
                            if (jokerCount == 1)
                            {
                                // Three of a kind with the new value
                                return 4;
                            }

                            // Two Pair
                            return 3;
                        }
                    }
                }

                // One pair
                return 2;
            }

            // There was only 1 of this card
            if (hand.Length > 3)
            {
                // The joker count must be zero, for us to have a score of 1
                return ScoreHandForJoker(hand[1..], 0);
            }

            if (hand[^1] == hand[^2])
            {
                // One pair
                return 2;
            }

            return 1;
        }

        private static int ScoreHand(ReadOnlySpan<char> hand)
        {
            int count = hand.Count(hand[0]);
            if (count == 5)
            {
                // 5 of a kind
                return 7;
            }

            if (count == 4)
            {
                // Four of a kind
                return 6;
            }

            if (count == 3)
            {
                if (hand.Length > 3)
                {
                    int notItIndex = hand[1..].IndexOfAnyExcept(hand[0]) + 1;
                    if (hand[1..].Count(hand[notItIndex]) == 2)
                    {
                        // Full House
                        return 5;
                    }
                }

                // Three of a kind
                return 4;
            }

            if (count == 2)
            {
                if (hand.Length > 3)
                {
                    int firstNotItIndex = hand[1..].IndexOfAnyExcept(hand[0]) + 1;
                    int secondCount = hand[1..].Count(hand[firstNotItIndex]);
                    if (secondCount == 3)
                    {
                        // Full House
                        return 5;
                    }
                    else if (secondCount == 2)
                    {
                        // Two pair
                        return 3;
                    }

                    if (hand.Length > (firstNotItIndex + 1))
                    {
                        int secondNotItIndex = hand[(firstNotItIndex + 1)..].IndexOfAnyExcept(hand[0], hand[firstNotItIndex]) + (firstNotItIndex + 1);
                        if (hand[(firstNotItIndex + 1)..].Count(hand[secondNotItIndex]) == 2)
                        {
                            // Two Pair
                            return 3;
                        }
                    }
                }

                // One pair
                return 2;
            }

            // There was only 1 of this card
            if (hand.Length > 3)
            {
                return ScoreHand(hand[1..]);
            }

            if (hand[^1] == hand[^2])
            {
                // One pair
                return 2;
            }

            return 1;
        }

        private static int ScoreCard(char c)
        {
            return c switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => 11,
                'T' => 10,
                _ => c - '0'
            };
        }

        private static int ScoreCardForJoker(char c)
        {
            return c switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'T' => 10,
                
                'J' => 0,

                _ => c - '0'
            };
        }

    }
}
