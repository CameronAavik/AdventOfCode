using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day04 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;

        foreach (Range passportRange in input.Split("\n\n"u8))
        {
            ReadOnlySpan<byte> passport = input[passportRange];
            byte fieldFlags = 0;
            bool hasInvalidField = false;

            int i = 0;
            while (i < passport.Length)
            {
                ReadOnlySpan<byte> fieldName = passport.Slice(i, 3);
                int fieldLength = passport.Slice(i + 4).IndexOfAny("\n "u8);
                if (fieldLength == -1)
                {
                    fieldLength = passport.Length - i - 4;
                }

                ReadOnlySpan<byte> fieldValue = passport.Slice(i + 4, fieldLength);

                switch (fieldName[0])
                {
                    case (byte)'b': // byr
                        fieldFlags |= 1 << 0;
                        hasInvalidField = hasInvalidField || !IsValidBirthYear(fieldValue);
                        break;
                    case (byte)'i': // iyr
                        fieldFlags |= 1 << 1;
                        hasInvalidField = hasInvalidField || !IsValidIssueYear(fieldValue);
                        break;
                    case (byte)'e' when fieldName[1] == 'y': // eyr
                        fieldFlags |= 1 << 2;
                        hasInvalidField = hasInvalidField || !IsValidExpirationYear(fieldValue);
                        break;
                    case (byte)'h' when fieldName[1] == 'g': // hgt
                        fieldFlags |= 1 << 3;
                        hasInvalidField = hasInvalidField || !IsValidHeight(fieldValue);
                        break;
                    case (byte)'h': // hcl
                        fieldFlags |= 1 << 4;
                        hasInvalidField = hasInvalidField || !IsValidHairColour(fieldValue);
                        break;
                    case (byte)'e': // ecl
                        fieldFlags |= 1 << 5;
                        hasInvalidField = hasInvalidField || !IsValidEyeColour(fieldValue);
                        break;
                    case (byte)'p': // pid
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

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    public static bool IsValidBirthYear(ReadOnlySpan<byte> span)
    {
        if (span.Length != 4)
        {
            return false;
        }

        return span[0] switch
        {
            (byte)'1' => span[1] == '9' &&
                span[2] is >= (byte)'2' and <= (byte)'9' &&
                span[3] is >= (byte)'0' and <= (byte)'9',
            (byte)'2' => span[1] == '0' &&
                span[2] == '0' &&
                span[3] is >= (byte)'0' and <= (byte)'2',
            _ => false,
        };
    }

    public static bool IsValidIssueYear(ReadOnlySpan<byte> span)
    {
        if (span.Length != 4 || span[0] != '2' || span[1] != '0')
        {
            return false;
        }

        return span[2] switch
        {
            (byte)'1' => span[3] is >= (byte)'0' and <= (byte)'9',
            (byte)'2' => span[3] == '0',
            _ => false,
        };
    }

    public static bool IsValidExpirationYear(ReadOnlySpan<byte> span)
    {
        if (span.Length != 4 || span[0] != '2' || span[1] != '0')
        {
            return false;
        }

        return span[2] switch
        {
            (byte)'2' => span[3] is >= (byte)'0' and <= (byte)'9',
            (byte)'3' => span[3] == '0',
            _ => false,
        };
    }

    public static bool IsValidHeight(ReadOnlySpan<byte> span)
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
                    (byte)'5' => span[1] == '9',
                    (byte)'6' => span[1] is >= (byte)'0' and <= (byte)'9',
                    (byte)'7' => span[1] is >= (byte)'0' and <= (byte)'6',
                    _ => false
                };
            case 5: // centimetres
                if (span[0] != '1' || span[3] != 'c' || span[4] != 'm')
                {
                    return false;
                }

                return span[1] switch
                {
                    (byte)'9' => span[2] is >= (byte)'0' and <= (byte)'6',
                    >= (byte)'5' => span[2] is >= (byte)'0' and <= (byte)'9',
                    _ => false
                };
            default:
                return false;
        }
    }

    public static bool IsValidHairColour(ReadOnlySpan<byte> span)
    {
        if (span.Length != 7 || span[0] != '#')
        {
            return false;
        }

        for (int i = 0; i < 6; i++)
        {
            if (span[i + 1] is not ((>= (byte)'0' and <= (byte)'9') or (>= (byte)'a' and <= (byte)'f')))
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsValidEyeColour(ReadOnlySpan<byte> span)
    {
        if (span.Length != 3)
        {
            return false;
        }

        return span[0] switch
        {
            (byte)'a' => span[1] == 'm' && span[2] == 'b',
            (byte)'b' => span[1] switch { (byte)'l' => span[2] == 'u', (byte)'r' => span[2] == 'n', _ => false },
            (byte)'g' => span[1] == 'r' && span[2] is (byte)'y' or (byte)'n',
            (byte)'h' => span[1] == 'z' && span[2] == 'l',
            (byte)'o' => span[1] == 't' && span[2] == 'h',
            _ => false
        };
    }

    public static bool IsValidPassportId(ReadOnlySpan<byte> span)
    {
        if (span.Length != 9)
        {
            return false;
        }

        for (int i = 0; i < 9; i++)
        {
            if (span[i] is not (>= (byte)'0' and <= (byte)'9'))
            {
                return false;
            }
        }

        return true;
    }
}
