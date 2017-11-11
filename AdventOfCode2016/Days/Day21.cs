using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Extensions;

namespace AdventOfCode2016.Days
{
    // ReSharper disable once UnusedMember.Global
    class Day21 : Day
    {
        public Day21() : base(21) {}

        private class Instruction
        {
            public InstructionType InstructionType;
            public int X, Y, Steps;
            public RotationDirection Direction;
            public char A, B, RotationLetter;
        }

        private enum InstructionType
        {
            SwapPosition,
            SwapLetter,
            RotateSteps,
            RotateByLetterIndex,
            ReversePositions,
            MovePosition
        }
        

        private enum RotationDirection
        {
            Left,
            Right
        }

        private List<Instruction> ParseInstructions(List<string> inputLines)
        {
            List<Instruction> instructions = new List<Instruction>();
            foreach (string line in inputLines)
            {
                string[] parts = line.Split(' ');
                
                InstructionType type;

                switch (parts[0])
                {
                    case "swap":
                        type = parts[1] == "position" ? InstructionType.SwapPosition : InstructionType.SwapLetter;
                        break;
                    case "reverse":
                        type = InstructionType.ReversePositions;
                        break;
                    case "rotate":
                        type = parts[1] == "based" ? InstructionType.RotateByLetterIndex : InstructionType.RotateSteps;
                        break;
                    case "move":
                        type = InstructionType.MovePosition;
                        break;
                    default:
                        throw new Exception("Couldn't determine instruction type: " + line);
                }
                
                switch (type)
                {
                    case InstructionType.SwapPosition:
                        instructions.Add(new Instruction
                        {
                            InstructionType = type,
                            X = parts[2].ToInt(),
                            Y = parts[5].ToInt()
                        });
                        break;
                    case InstructionType.SwapLetter:
                        instructions.Add(new Instruction
                        {
                            InstructionType = type,
                            A = parts[2][0],
                            B = parts[5][0]
                        });
                        break;
                    case InstructionType.RotateSteps:
                        instructions.Add(new Instruction
                        {
                            InstructionType = type,
                            Direction = (RotationDirection) Enum.Parse(typeof(RotationDirection), parts[1], true),
                            Steps = parts[2].ToInt()
                        });
                        break;
                    case InstructionType.RotateByLetterIndex:
                        instructions.Add(new Instruction
                        {
                            InstructionType = type,
                            Direction = RotationDirection.Right,
                            RotationLetter = parts[6][0]
                        });
                        break;
                    case InstructionType.ReversePositions:
                        instructions.Add(new Instruction
                        {
                            InstructionType = type,
                            X = parts[2].ToInt(),
                            Y = parts[4].ToInt()
                        });
                        break;
                    case InstructionType.MovePosition:
                        instructions.Add(new Instruction
                        {
                            InstructionType = type,
                            X = parts[2].ToInt(),
                            Y = parts[5].ToInt()
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return instructions;
        }

        private void SwapPosition(List<char> characters,  int x, int y)
        {
            char z = characters[y];
            characters[y] = characters[x];
            characters[x] = z;
        }

        private void SwapLetter(List<char> characters, char a, char b)
        {
            SwapPosition(characters, characters.IndexOf(a), characters.IndexOf(b));
        }

        private void Rotate(List<char> characters, RotationDirection direction, int steps)
        {
            for (int s = 0; s < steps; s++)
            {
                if (direction == RotationDirection.Right)
                {
                    char temp = characters[characters.Count - 1];
                    characters.Insert(0, temp);
                    characters.RemoveAt(characters.Count - 1);
                }
                else
                {
                    char temp = characters[0];
                    characters.Add(temp);
                    characters.RemoveAt(0);
                }
            }
        }

        private void RotateByLetterIndex(List<char> characters, char letter)
        {
            int index = characters.IndexOf(letter);

            Rotate(characters, RotationDirection.Right, 1 + index + (index >= 4 ? 1 : 0));
        }

        private void ReversePositions(List<char> characters, int x, int y)
        {
            characters.Reverse(x, (y - x) + 1);
        }

        private void MovePosition(List<char> characters, int x, int y)
        {
            char c = characters[x];
            characters.RemoveAt(x);
            characters.Insert(y, c);
        }

        private void ExecuteInstruction(List<char> characters, Instruction instruction)
        {
            switch (instruction.InstructionType)
            {
                case InstructionType.SwapPosition:
                    SwapPosition(characters, instruction.X, instruction.Y);
                    break;
                case InstructionType.SwapLetter:
                    SwapLetter(characters, instruction.A, instruction.B);
                    break;
                case InstructionType.RotateSteps:
                    Rotate(characters, instruction.Direction, instruction.Steps);
                    break;
                case InstructionType.RotateByLetterIndex:
                    RotateByLetterIndex(characters, instruction.RotationLetter);
                    break;
                case InstructionType.ReversePositions:
                    ReversePositions(characters, instruction.X, instruction.Y);
                    break;
                case InstructionType.MovePosition:
                    MovePosition(characters, instruction.X, instruction.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ReverseInstruction(List<char> characters, Instruction instruction)
        {
            switch (instruction.InstructionType)
            {
                case InstructionType.SwapPosition:
                    SwapPosition(characters, instruction.Y, instruction.X);
                    break;
                case InstructionType.SwapLetter:
                    SwapLetter(characters, instruction.B, instruction.A);
                    break;
                case InstructionType.RotateSteps:
                    if (instruction.Direction == RotationDirection.Left)
                    {
                        Rotate(characters, RotationDirection.Right, instruction.Steps);
                    }
                    else
                    {
                        Rotate(characters, RotationDirection.Left, instruction.Steps);
                    }
                    
                    break;
                case InstructionType.RotateByLetterIndex:
                    //rotate based on position of letter X means that the whole string should be rotated to the right based on the index of letter X (counting from 0) as determined
                    //before this instruction does any rotations. Once the index is determined, rotate the string to the right one time, plus a number of times equal to that index,
                    //plus one additional time if the index was at least 4.
                    int rotationAmount = 1; //todo: figure out correct rotation amount
                    int newCharIndex = characters.IndexOf(instruction.RotationLetter);

                    List<char> startingList = new List<char>(characters);
                    int i = 1;
                    Rotate(startingList, RotationDirection.Left, i);
                    List<char> tempList = new List<char>(startingList);
                    RotateByLetterIndex(tempList, instruction.RotationLetter);
                    while (tempList.IndexOf(instruction.RotationLetter) != newCharIndex)
                    {
                        i++;
                        if (i > characters.Count)
                        {
                            throw new Exception("We've gone a whole round, something's off.");
                        }
                        startingList = new List<char>(characters);
                        Rotate(startingList, RotationDirection.Left, i);
                        tempList = new List<char>(startingList);
                        RotateByLetterIndex(tempList, instruction.RotationLetter);
                    }

                    
                    Rotate(characters, RotationDirection.Left, i);
                    break;
                case InstructionType.ReversePositions:
                    ReversePositions(characters, instruction.X, instruction.Y);
                    break;
                case InstructionType.MovePosition:
                    MovePosition(characters, instruction.Y, instruction.X);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExecuteInstructions(List<char> characters, List<Instruction> instructions, bool reverse = false)
        {
            if (reverse)
            {
                for (int i = instructions.Count - 1; i >= 0; i--)
                {
                    Instruction instruction = instructions[i];
                    ReverseInstruction(characters, instruction);
                }
            }
            else
            {
                for (int i = 0; i < instructions.Count; i++)
                {
                    Instruction instruction = instructions[i];
                    ExecuteInstruction(characters, instruction);
                }
            }
        }
        
        protected override object GetSolutionPart1()
        {
            /*
             * --- Day 21: Scrambled Letters and Hash ---

                The computer system you're breaking into uses a weird scrambling function to store its passwords. It shouldn't be much trouble to create your own scrambled password so
                you can add it to the system; you just have to implement the scrambler.

                The scrambling function is a series of operations (the exact list is provided in your puzzle input). Starting with the password to be scrambled, apply each operation
                in succession to the string. The individual operations behave as follows:

                    swap position X with position Y means that the letters at indexes X and Y (counting from 0) should be swapped.
                    swap letter X with letter Y means that the letters X and Y should be swapped (regardless of where they appear in the string).
                    rotate left/right X steps means that the whole string should be rotated; for example, one right rotation would turn abcd into dabc.

                    rotate based on position of letter X means that the whole string should be rotated to the right based on the index of letter X (counting from 0) as determined
                        before this instruction does any rotations. Once the index is determined, rotate the string to the right one time, plus a number of times equal to that index,
                        plus one additional time if the index was at least 4.

                    reverse positions X through Y means that the span of letters at indexes X through Y (including the letters at X and Y) should be reversed in order.
                    move position X to position Y means that the letter which is at index X should be removed from the string, then inserted such that it ends up at index Y.

                For example, suppose you start with abcde and perform the following operations:

                    swap position 4 with position 0 swaps the first and last letters, producing the input for the next step, ebcda.
                    swap letter d with letter b swaps the positions of d and b: edcba.
                    reverse positions 0 through 4 causes the entire string to be reversed, producing abcde.
                    rotate left 1 step shifts all letters left one position, causing the first letter to wrap to the end of the string: bcdea.
                    move position 1 to position 4 removes the letter at position 1 (c), then inserts it at position 4 (the end of the string): bdeac.
                    move position 3 to position 0 removes the letter at position 3 (a), then inserts it at position 0 (the front of the string): abdec.
                    rotate based on position of letter b finds the index of letter b (1), then rotates the string right once plus a number of times equal to that index (2): ecabd.
                    rotate based on position of letter d finds the index of letter d (4), then rotates the string right once, plus a number of times equal to that index, plus
                    an additional time because the index was at least 4, for a total of 6 right rotations: decab.

                After these steps, the resulting scrambled password is decab.

                Now, you just need to generate a new scrambled password and you can access the system. Given the list of scrambling operations in your puzzle input, what is the
                result of scrambling abcdefgh?
             */

            #region Testrun

            List<Instruction> testInstructions = ParseInstructions(new List<string>
            {
                "swap position 4 with position 0",
                "swap letter d with letter b",
                "reverse positions 0 through 4",
                "rotate left 1 step",
                "move position 1 to position 4",
                "move position 3 to position 0",
                "rotate based on position of letter b",
                "rotate based on position of letter d"
            });
            string testPw = "abcde";
            List<char> testChars = testPw.ToCharArray().ToList();
            string testResult = string.Join("", testChars);
            ExecuteInstruction(testChars, testInstructions[0]); //swap position 4 with position 0 swaps the first and last letters, producing the input for the next step, ebcda.
            testResult = string.Join("", testChars);

            ExecuteInstruction(testChars, testInstructions[1]); //swap letter d with letter b swaps the positions of d and b: edcba.
            testResult = string.Join("", testChars);

            ExecuteInstruction(testChars, testInstructions[2]); //reverse positions 0 through 4 causes the entire string to be reversed, producing abcde.
            testResult = string.Join("", testChars);

            ExecuteInstruction(testChars, testInstructions[3]); //rotate left 1 step shifts all letters left one position, causing the first letter to wrap to the end of the string: bcdea.
            testResult = string.Join("", testChars);

            ExecuteInstruction(testChars, testInstructions[4]); //move position 1 to position 4 removes the letter at position 1 (c), then inserts it at position 4 (the end of the string): bdeac.
            testResult = string.Join("", testChars);

            ExecuteInstruction(testChars, testInstructions[5]); //move position 3 to position 0 removes the letter at position 3 (a), then inserts it at position 0 (the front of the string): abdec.
            testResult = string.Join("", testChars);

            ExecuteInstruction(testChars, testInstructions[6]); //rotate based on position of letter b finds the index of letter b (1), then rotates the string right once plus a number of times equal to that index (2): ecabd.
            testResult = string.Join("", testChars);

            ExecuteInstruction(testChars, testInstructions[7]); //rotate based on position of letter d finds the index of letter d (4), then rotates the string right once, plus a number of times equal to that index, plus an additional time because the index was at least 4, for a total of 6 right rotations: decab.
            //ExecuteInstructions(testChars, testInstructions);

            testResult = string.Join("", testChars);
            if (testResult != "decab")
            {
                throw new Exception($"Test failed! Expected: decab, actual: {testResult}");
            }


            #endregion

            List<Instruction> instructions = ParseInstructions(InputLines);
            string originalPassword = "abcdefgh";
            List<char> pwChars = originalPassword.ToCharArray().ToList();
            ExecuteInstructions(pwChars, instructions);

            string result = string.Join("", pwChars);
            return result; //bgfacdeh
        }

        protected override object GetSolutionPart2()
        {
            /*
             * You scrambled the password correctly, but you discover that you can't actually modify the password file on the system.
             * You'll need to un-scramble one of the existing passwords by reversing the scrambling process.

                What is the un-scrambled version of the scrambled password fbgdceah?
             */

            #region Testrun
            List<Instruction> testInstructions = ParseInstructions(new List<string>
            {
                "swap position 4 with position 0",
                "swap letter d with letter b",
                "reverse positions 0 through 4",
                "rotate left 1 step",
                "move position 1 to position 4",
                "move position 3 to position 0",
                "rotate based on position of letter b",
                "rotate based on position of letter d"
            });
            string testScrambledPassword = "decab";
            List<char> testPwChars = testScrambledPassword.ToCharArray().ToList();
            
            string testResult = string.Join("", testPwChars);
            
            ReverseInstruction(testPwChars, testInstructions[7]);
            testResult = string.Join("", testPwChars);

            ReverseInstruction(testPwChars, testInstructions[6]);
            testResult = string.Join("", testPwChars);

            ReverseInstruction(testPwChars, testInstructions[5]);
            testResult = string.Join("", testPwChars);

            ReverseInstruction(testPwChars, testInstructions[4]);
            testResult = string.Join("", testPwChars);

            ReverseInstruction(testPwChars, testInstructions[3]);
            testResult = string.Join("", testPwChars);

            ReverseInstruction(testPwChars, testInstructions[2]);
            testResult = string.Join("", testPwChars);

            ReverseInstruction(testPwChars, testInstructions[1]);
            testResult = string.Join("", testPwChars);

            ReverseInstruction(testPwChars, testInstructions[0]);
            testResult = string.Join("", testPwChars);
            

            if (testResult != "abcde")
            {
                throw new Exception($"Test failed! Expected: abcde, actual: {testResult}");
            }
            #endregion

            List<Instruction> instructions = ParseInstructions(InputLines);
            string scrambledPassword = "fbgdceah";
            List<char> pwChars = scrambledPassword.ToCharArray().ToList();
            ExecuteInstructions(pwChars, instructions, reverse: true);
            string result = string.Join("", pwChars);
            return result; //bdgheacf
        }
    }
}
