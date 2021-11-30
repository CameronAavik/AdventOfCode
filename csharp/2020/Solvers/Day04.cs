using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day04 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)
    {
        int part1 = 0;
        int part2 = 0;

        foreach (ReadOnlySpan<char> passport in input.Split("\n\n"))
        {
            byte fieldFlags = 0;
            bool hasInvalidField = false;

            int i = 0;
            while (i < passport.Length)
            {
                ReadOnlySpan<char> fieldName = passport.Slice(i, 3);
                int fieldLength = passport.Slice(i + 4).IndexOfAny("\n ");
                if (fieldLength == -1)
                {
                    fieldLength = passport.Length - i - 4;
                }

                ReadOnlySpan<char> fieldValue = passport.Slice(i + 4, fieldLength);

                switch (fieldName[0])
                {
                    case 'b': // byr
                        fieldFlags |= 1 << 0;
                        hasInvalidField = hasInvalidField || !IsValidBirthYear(fieldValue);
                        break;
                    case 'i': // iyr
                        fieldFlags |= 1 << 1;
                        hasInvalidField = hasInvalidField || !IsValidIssueYear(fieldValue);
                        break;
                    case 'e' when fieldName[1] == 'y': // eyr
                        fieldFlags |= 1 << 2;
                        hasInvalidField = hasInvalidField || !IsValidExpirationYear(fieldValue);
                        break;
                    case 'h' when fieldName[1] == 'g': // hgt
                        fieldFlags |= 1 << 3;
                        hasInvalidField = hasInvalidField || !IsValidHeight(fieldValue);
                        break;
                    case 'h': // hcl
                        fieldFlags |= 1 << 4;
                        hasInvalidField = hasInvalidField || !IsValidHairColour(fieldValue);
                        break;
                    case 'e': // ecl
                        fieldFlags |= 1 << 5;
                        hasInvalidField = hasInvalidField || !IsValidEyeColour(fieldValue);
                        break;
                    case 'p': // pid
                        fieldFlags |= 1 << 6;
                        hasInvalidField = hasInvalidField || !IsValidPassportId(fieldValue);
                        break;
                }

                i += 5 + fieldLength;
            }

            if (fieldFlags == 0b1111111)
            {
                part1 += 1;
                if (!hasInvalidField)
                {
                    part2 += 1;
                }
            }
        }

        return new Solution(part1, part2);
    }

    public static bool IsValidBirthYear(ReadOnlySpan<char> span)
    {
        if (span.Length != 4)
        {
            return false;
        }

        return span[0] switch
        {
            '1' => span[1] == '9' &&
                span[2] is >= '2' and <= '9' &&
                span[3] is >= '0' and <= '9',
            '2' => span[1] == '0' &&
                span[2] == '0' &&
                span[3] is >= '0' and <= '2',
            _ => false,
        };
    }

    public static bool IsValidIssueYear(ReadOnlySpan<char> span)
    {
        if (span.Length != 4 || span[0] != '2' || span[1] != '0')
        {
            return false;
        }

        return span[2] switch
        {
            '1' => span[3] is >= '0' and <= '9',
            '2' => span[3] == '0',
            _ => false,
        };
    }

    public static bool IsValidExpirationYear(ReadOnlySpan<char> span)
    {
        if (span.Length != 4 || span[0] != '2' || span[1] != '0')
        {
            return false;
        }

        return span[2] switch
        {
            '2' => span[3] is >= '0' and <= '9',
            '3' => span[3] == '0',
            _ => false,
        };
    }

    public static bool IsValidHeight(ReadOnlySpan<char> span)
    {
        switch (span.Length)
        {
            case 4: // inches
                if (span[2] != 'i' || span[3] != 'n')
                {
                    return false;
                }

                return span[0] switch
                {
                    '5' => span[1] == '9',
                    '6' => span[1] is >= '0' and <= '9',
                    '7' => span[1] is >= '0' and <= '6',
                    _ => false
                };
            case 5: // centimetres
                if (span[0] != '1' || span[3] != 'c' || span[4] != 'm')
                {
                    return false;
                }

                return span[1] switch
                {
                    >= '5' and <= '8' => span[2] is >= '0' and <= '9',
                    '9' => span[2] is >= '0' and <= '6',
                    _ => false
                };
            default:
                return false;
        }
    }

    public static bool IsValidHairColour(ReadOnlySpan<char> span)
    {
        if (span.Length != 7 || span[0] != '#')
        {
            return false;
        }

        for (int i = 0; i < 6; i++)
        {
            if (span[i + 1] is not ((>= '0' and <= '9') or (>= 'a' and <= 'f')))
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsValidEyeColour(ReadOnlySpan<char> span)
    {
        if (span.Length != 3)
        {
            return false;
        }

        return span[0] switch
        {
            'a' => span[1] == 'm' && span[2] == 'b',
            'b' => span[1] switch { 'l' => span[2] == 'u', 'r' => span[2] == 'n', _ => false },
            'g' => span[1] == 'r' && span[2] is 'y' or 'n',
            'h' => span[1] == 'z' && span[2] == 'l',
            'o' => span[1] == 't' && span[2] == 'h',
            _ => false
        };
    }

    public static bool IsValidPassportId(ReadOnlySpan<char> span)
    {
        if (span.Length != 9)
        {
            return false;
        }

        for (int i = 0; i < 9; i++)
        {
            if (span[i] is not (>= '0' and <= '9'))
            {
                return false;
            }
        }

        return true;
    }
}
