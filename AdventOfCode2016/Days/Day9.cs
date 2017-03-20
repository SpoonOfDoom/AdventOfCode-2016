using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    class Day9 : Day
    {
        public Day9() : base(9) {}
        Regex regex = new Regex(@"(\(\d+x\d+\))");

        private class Marker
        {
            public int charCount;
            public int repeatCount;
            public int MarkerLength;

            public int DecompressedLength => charCount * repeatCount;
        }

        Dictionary<string, Marker> markerCache = new Dictionary<string, Marker>();

        private Marker ParseMarker(string markerString)
        {
            if (!markerCache.ContainsKey(markerString))
            {
                var parts = markerString.Replace("(", "").Replace(")", "").Split('x');
                var marker = new Marker
                             {
                                 charCount = parts[0].ToInt(),
                                 repeatCount = parts[1].ToInt(),
                                 MarkerLength = markerString.Length
                             };
                markerCache[markerString] = marker;
            }
            return markerCache[markerString];
        }

        private string DecompressPart(int count, string part)
        {
            return string.Concat(Enumerable.Repeat(part, count));
        }

        private string DecompressString(string s)
        {
            int currentIndex = 0;

            string newString = s;

            while (currentIndex < newString.Length-1)
            {
                Group g = regex.Match(newString, currentIndex).Groups[0];
                if (g.Length == 0)
                {
                    break;
                }
                string front = newString.Substring(0, g.Index);
                Marker marker = ParseMarker(g.Value);
                string decompressedMarker = DecompressPart(marker.repeatCount, newString.Substring(g.Index + marker.MarkerLength, marker.charCount));
                string remaining = newString.Substring(g.Index + marker.MarkerLength + marker.charCount);

                newString = front + decompressedMarker + remaining;
                currentIndex = g.Index + marker.DecompressedLength;
            }
            
            return newString;
        }

        private long DecompressStringV2(string s)
        {
            Console.Clear();
            long length = 0;
            string newString = s;
            Group g;
            Marker marker;
            string decompressedMarker;
            string remaining;
            
            while (newString.Length > 0)
            {
                g = regex.Match(newString).Groups[0];
                if (g.Length == 0)
                {
                    length += newString.Length;
                    break;
                }
                length += g.Index;
                marker = ParseMarker(g.Value);
                decompressedMarker = DecompressPart(marker.repeatCount, newString.Substring(g.Index + marker.MarkerLength, marker.charCount));
                remaining = newString.Substring(g.Index + marker.MarkerLength + marker.charCount);

                newString = decompressedMarker + remaining;
            }
            
            return length;
        }

        private long DecompressStringV3(string newString)
        {
            long length = 0;
            var g = regex.Match(newString).Groups[0];
            if (g.Length == 0)
            {
                return newString.Length;
            }
            length += g.Index;
            var marker = ParseMarker(g.Value);
            var decompressedMarker = DecompressPart(marker.repeatCount, newString.Substring(g.Index + marker.MarkerLength, marker.charCount));

            
            var remaining = newString.Substring(g.Index + marker.MarkerLength + marker.charCount);
            return length + DecompressStringV3(decompressedMarker + remaining);
        }

        private long DecompressWithVersion(string s, int version)
        {
            switch (version)
            {
                case 1:
                    return DecompressString(s).Length;
                case 2:
                    return DecompressStringV2(s);
                case 3:
                    return DecompressStringV3(s);
                default:
                    return -1;
            }
        }

        private void CacheAllMarkers()
        {
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                ParseMarker(match.Groups[0].Value);
            }
        }

        public override object GetSolutionPart1()
        {
            /*Wandering around a secure area, you come across a datalink port to a new part of the network. After briefly scanning it for interesting files,
            you find one file in particular that catches your attention.It's compressed with an experimental format, but fortunately, the documentation for the format is nearby.
            
            The format compresses a sequence of characters. Whitespace is ignored. To indicate that some sequence should be repeated, a marker is added to the file, like(10x2).
            To decompress this marker, take the subsequent 10 characters and repeat them 2 times. Then, continue reading the file after the repeated data.The marker itself is
            not included in the decompressed output.

            If parentheses or other characters appear within the data referenced by a marker, that's okay - treat it like normal data, not a marker, and then resume looking
            for markers after the decompressed section.

            For example:

                ADVENT contains no markers and decompresses to itself with no changes, resulting in a decompressed length of 6.
                A(1x5)BC repeats only the B a total of 5 times, becoming ABBBBBC for a decompressed length of 7.
                (3x3)XYZ becomes XYZXYZXYZ for a decompressed length of 9.
                A(2x2)BCD(2x2)EFG doubles the BC and EF, becoming ABCBCDEFEFG for a decompressed length of 11.
                (6x1)(1x3)A simply becomes(1x3)A - the(1x3) looks like a marker, but because it's within a data section of another marker, it is not treated any differently
                                                            from the A that comes after it. It has a decompressed length of 6.
                X(8x2)(3x3)ABCY becomes X(3x3)ABC(3x3)ABCY(for a decompressed length of 18), because the decompressed data from the (8x2) marker(the(3x3)ABC) is skipped and not processed further.

            What is the decompressed length of the file(your puzzle input) ? Don't count whitespace.
            */
        
            Dictionary<string, string> testResultsShouldBe = new Dictionary<string, string>
                                                    {
                                                        {"ADVENT", "ADVENT"},
                                                        {"A(1x5)BC", "ABBBBBC"},
                                                        {"(3x3)XYZ", "XYZXYZXYZ"},
                                                        {"A(2x2)BCD(2x2)EFG", "ABCBCDEFEFG"},
                                                        {"(6x1)(1x3)A", "(1x3)A"},
                                                        {"X(8x2)(3x3)ABCY", "X(3x3)ABC(3x3)ABCY"},
                                                    };

            foreach (var keyValuePair in testResultsShouldBe)
            {
                string testInput = keyValuePair.Key;
                string result = DecompressString(testInput);

                if (testResultsShouldBe[testInput] != result)
                {
                    string narf = DecompressString(testInput); //for easy jump into debugging

                    throw new Exception($"Test failed.\nExpected: {testResultsShouldBe[testInput]}\nGot instead: {result}"); //So we can't miss it if a test input fails and we don't have a breakpoint.
                }
            }

            return DecompressString(input).Length; //99145
        }

        public override object GetSolutionPart2()
        {
            /*
             *Apparently, the file actually uses version two of the format.

                In version two, the only difference is that markers within decompressed data are decompressed. This, the documentation explains, provides much more substantial compression capabilities, allowing many-gigabyte files to be stored in only a few kilobytes.

                For example:

                    (3x3)XYZ still becomes XYZXYZXYZ, as the decompressed section contains no markers.
                    X(8x2)(3x3)ABCY becomes XABCABCABCABCABCABCY, because the decompressed data from the (8x2) marker is then further decompressed, thus triggering the (3x3) marker twice for a total of six ABC sequences.
                    (27x12)(20x12)(13x14)(7x10)(1x12)A decompresses into a string of A repeated 241920 times.
                    (25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN becomes 445 characters long.

                Unfortunately, the computer you brought probably doesn't have enough memory to actually decompress the file; you'll have to come up with another way to get its decompressed length.

                What is the decompressed length of the file using this improved format?
             **/

            CacheAllMarkers();
            Dictionary<string, string> testResultsShouldBe = new Dictionary<string, string>
                                                    {
                                                        {"(3x3)XYZ", "XYZXYZXYZ"},
                                                        {"X(8x2)(3x3)ABCY", "XABCABCABCABCABCABCY"},
                                                        {"(27x12)(20x12)(13x14)(7x10)(1x12)A",string.Concat(Enumerable.Repeat("A", 241920))},
                                                        {"(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN", "<Meta:Length=445>"},
                                                    };
            const int testVersion = 2;
            foreach (var keyValuePair in testResultsShouldBe)
            {
                string testInput = keyValuePair.Key;
                long result = DecompressWithVersion(testInput, testVersion);
                bool resultCorrect;
                if (testInput == "(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN")
                {
                    resultCorrect = result == 445;
                }
                else
                {
                    resultCorrect = testResultsShouldBe[testInput].Length == result;
                }
                if (!resultCorrect)
                {
                    long narf = DecompressWithVersion(testInput, testVersion); //for easy jump into debugging

                    throw new Exception($"Test failed.\nExpected: {testResultsShouldBe[testInput]}\nGot instead: {result}"); //So we can't miss it if a test input fails and we don't have a breakpoint.
                }
            }
            return DecompressWithVersion(input, testVersion); //10943094568, runtime 1:03:31!
        }
    }
}
