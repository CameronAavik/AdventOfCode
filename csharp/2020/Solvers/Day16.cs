using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day16 : ISolver
    {
        public readonly struct Field
        {
            public readonly int L1;
            public readonly int R1;
            public readonly int L2;
            public readonly int R2;

            public Field(int l1, int r1, int l2, int r2)
            {
                L1 = l1;
                R1 = r1;
                L2 = l2;
                R2 = r2;
            }
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            var reader = new SpanReader(input);

            int departureFieldsIndex = 0;
            int[] departureFields = new int[6];
            var fieldList = new List<Field>();

            int maxFieldVal = int.MinValue;
            while (reader.Peek() != '\n')
            {
                var field = ParseField(ref reader, out bool isDeparture);
                fieldList.Add(field);
                if (isDeparture)
                {
                    departureFields[departureFieldsIndex++] = fieldList.Count - 1;
                }

                if (field.R2 > maxFieldVal)
                    maxFieldVal = field.R2;
            }

            var potentialFieldsByValue = new int[maxFieldVal + 1];
            var numFields = fieldList.Count;
            for (int i = 0; i < numFields; i++)
            {
                Field field = fieldList[i];
                for (int j = field.L1; j <= field.R1; j++)
                    potentialFieldsByValue[j] |= 1 << i;
                for (int j = field.L2; j <= field.R2; j++)
                    potentialFieldsByValue[j] |= 1 << i;
            }

            reader.SkipLength("\nyour ticket:\n".Length);
            int[] ticket = ParseTicket(ref reader, numFields);

            reader.SkipLength("\nnearby tickets:\n".Length);

            int[] potentialFields = new int[numFields];
            for (int i = 0; i < numFields; i++)
                potentialFields[i] = -1; // all 1 bits

            int part1 = 0;
            int[] nearbyTicket = new int[numFields];
            while (!reader.Done)
            {
                int errors = 0;
                for (int i = 0; i < numFields; i++)
                {
                    var value = reader.ReadPosIntUntil(i == numFields - 1 ? '\n' : ',');
                    if (value > maxFieldVal || potentialFieldsByValue[value] == 0)
                    {
                        errors += value;
                    }

                    nearbyTicket[i] = value;
                }

                if (errors > 0)
                {
                    part1 += errors;
                    continue;
                }

                for (int i = 0; i < numFields; i++)
                {
                    potentialFields[i] &= potentialFieldsByValue[nearbyTicket[i]];
                }
            }

            int[] fieldIndexes = new int[numFields];
            for (int i = 0; i < numFields; i++)
            {
                int fieldToRemove = 0;
                for (int j = 0; j < numFields; j++)
                {
                    var potentials = potentialFields[j];
                    if (potentials == 0)
                        continue;

                    if ((potentials & (potentials - 1)) == 0)
                    {
                        fieldIndexes[BitOperations.TrailingZeroCount(potentials)] = j;
                        fieldToRemove = potentials;
                        break;
                    }
                }

                for (int j = 0; j < numFields; j++)
                {
                    potentialFields[j] &= ~fieldToRemove;
                }
            }

            long part2 = 1;
            foreach (int i in departureFields)
            {
                part2 *= ticket[fieldIndexes[i]];
            }

            return new Solution(part1.ToString(), part2.ToString());
        }

        private static Field ParseField(ref SpanReader reader, out bool isDeparture)
        {
            ReadOnlySpan<char> fieldName = reader.ReadUntil(':');
            reader.SkipLength(1); // skip the space
            int l1 = reader.ReadPosIntUntil('-');
            int r1 = reader.ReadPosIntUntil(' ');
            reader.SkipLength("or ".Length);
            int l2 = reader.ReadPosIntUntil('-');
            int r2 = reader.ReadPosIntUntil('\n');
            isDeparture = fieldName.StartsWith("departure ");
            return new Field(l1, r1, l2, r2);
        }

        private static int[] ParseTicket(ref SpanReader reader, int numFields)
        {
            int[] ticket = new int[numFields];
            for (int i = 0; i < numFields; i++)
            {
                ticket[i] = reader.ReadPosIntUntil(i == numFields - 1 ? '\n' : ',');
            }

            return ticket;
        }
    }
}
