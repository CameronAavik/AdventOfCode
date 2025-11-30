using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day16 : ISolver
{
    public record Field(int L1, int R1, int L2, int R2);

    public record FieldsData(List<Field> Fields, int[] DepartureFields, int LargestFieldValue);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var reader = new SpanReader(input);

        // fields is a list of (l1 - r1), (l2 - r2) ranges for each field
        // departureFields is the list of the 6 indexes of the fields labeled as "departure"
        // maxFieldVal is the largest possible value any field can be
        (var fields, var departureFields, var maxFieldVal) = ParseFields(ref reader);

        var numFields = fields.Count;

        reader.SkipLength("\nyour ticket:\n".Length);
        var myTicket = new int[numFields];
        ParseTicket(ref reader, myTicket);
        reader.SkipLength("\nnearby tickets:\n".Length);

        // for each number up to maxFieldVal, store a bitset capturing which fields allow that value
        // e.g. if potentialFieldsByValue[100] == 0b10001000, this means only fields 3 and 7 allow the value 100
        var potentialFieldsByValue = new int[maxFieldVal + 1];
        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var flag = 1 << i;
            for (var j = field.L1; j <= field.R1; j++)
                potentialFieldsByValue[j] |= flag;
            for (var j = field.L2; j <= field.R2; j++)
                potentialFieldsByValue[j] |= flag;
        }

        // fieldCandidates stores all the potential fields that each entry in the ticket can be
        // e.g. if fieldCandidates[4] == 0b100100, then the 5th item of the ticket can only be either field 2 or 5
        var fieldCandidates = new int[numFields];
        for (var i = 0; i < fieldCandidates.Length; i++)
            fieldCandidates[i] = -1; // -1 has all 1 bits, so all fields are valid initially

        // reusable array for storing ticket arrays
        var ticket = new int[numFields];

        var part1 = 0;
        while (!reader.Done)
        {
            ParseTicket(ref reader, ticket);

            // get sum of invalid values in the ticket
            var isValid = true;
            foreach (var fieldVal in ticket)
            {
                // potentialFieldsByValue[fieldVal] will be 0 when no fields are valid for a fieldVal
                if (fieldVal > maxFieldVal || potentialFieldsByValue[fieldVal] == 0)
                {
                    part1 += fieldVal;
                    isValid = false;
                }
            }

            // only use valid tickets to refine fieldCandidates
            if (isValid)
            {
                for (var i = 0; i < fieldCandidates.Length; i++)
                {
                    // using a bitwise and here will set any fields to 0 that aren't valid for this ticket value
                    fieldCandidates[i] &= potentialFieldsByValue[ticket[i]];
                }
            }
        }

        // uses the fieldCandidates array to determine which index in the ticket correlates to which field.
        var fieldIndexes = GetFieldIndexes(fieldCandidates);

        long part2 = 1;
        foreach (var i in departureFields)
        {
            part2 *= myTicket[fieldIndexes[i]];
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int[] GetFieldIndexes(int[] candidates)
    {
        var fieldIndexes = new int[candidates.Length];

        // This loop will identify a single field index each iteration
        // so this loop runs candidates.Length times 
        for (var fieldsLeft = candidates.Length; fieldsLeft > 0; fieldsLeft--)
        {
            var fieldToRemove = 0;
            for (var j = 0; j < candidates.Length; j++)
            {
                var fields = candidates[j];

                // if we have already identified the field index, then fields will be 0 so we skip it
                if (fields == 0)
                    continue;

                // x & (x - 1) == 0 is a way to test that x only has 1 bit set
                // when there is only 1 bit set, it means we have identified the field index
                if ((fields & (fields - 1)) == 0)
                {
                    // TrailingZeroCount is a fast way to identify which bit is set to 1
                    var field = BitOperations.TrailingZeroCount(fields);
                    fieldIndexes[field] = j;
                    fieldToRemove = fields;
                    break;
                }
            }

            // remove the field from each of the candidates now that we have identified it's field index
            for (var j = 0; j < candidates.Length; j++)
            {
                candidates[j] &= ~fieldToRemove;
            }
        }

        return fieldIndexes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static FieldsData ParseFields(ref SpanReader reader)
    {
        var departureFieldsIndex = 0;
        var departureFields = new int[6];

        var fieldList = new List<Field>();

        var maxFieldVal = int.MinValue;
        while (reader.Peek() != '\n')
        {
            var fieldName = reader.ReadUntil(':');
            reader.SkipLength(1);
            var l1 = reader.ReadPosIntUntil('-');
            var r1 = reader.ReadPosIntUntil(' ');
            reader.SkipLength("or ".Length);
            var l2 = reader.ReadPosIntUntil('-');
            var r2 = reader.ReadPosIntUntil('\n');

            fieldList.Add(new Field(l1, r1, l2, r2));
            if (fieldName.StartsWith("de"u8))
            {
                departureFields[departureFieldsIndex++] = fieldList.Count - 1;
            }

            if (r2 > maxFieldVal)
            {
                maxFieldVal = r2;
            }
        }

        return new FieldsData(fieldList, departureFields, maxFieldVal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ParseTicket(ref SpanReader reader, int[] ticket)
    {
        for (var i = 0; i < ticket.Length - 1; i++)
        {
            ticket[i] = reader.ReadPosIntUntil(',');
        }

        ticket[^1] = reader.ReadPosIntUntil('\n');
    }
}
