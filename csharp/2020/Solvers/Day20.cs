using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day20 : ISolver
    {
        record TileRotationData(int Id, bool IsFlipped, int Rotations, TileEdges Edges, int[] Data);

        record TileEdges(int Top, int Bottom, int Left, int Right)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TileEdges Rotate(int width) => new(ReverseBits(Left, width), ReverseBits(Right, width), Bottom, Top);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TileEdges Flip(int width) => new(ReverseBits(Top, width), ReverseBits(Bottom, width), Right, Left);
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            (int TileId, int[] Tile)[] tiles = ParseTiles(input);

            int tileWidth = tiles[0].Tile.Length;
            int numTiles = tiles.Length;

            var leftEdges = new Dictionary<int, List<TileRotationData>>();
            var topEdges = new Dictionary<int, List<TileRotationData>>();

            foreach ((int tileId, int[] tile) in tiles)
            {
                TileEdges defaultEdges = GetEdges(tile);
                TileEdges flippedEdges = defaultEdges.Flip(tileWidth);

                for (int i = 0; i < 4; i++)
                {
                    AddToDictOfLists(leftEdges, defaultEdges.Left, new TileRotationData(tileId, false, i, defaultEdges, tile));
                    AddToDictOfLists(topEdges, defaultEdges.Top, new TileRotationData(tileId, false, i, defaultEdges, tile));
                    AddToDictOfLists(leftEdges, flippedEdges.Left, new TileRotationData(tileId, true, i, flippedEdges, tile));
                    AddToDictOfLists(topEdges, flippedEdges.Top, new TileRotationData(tileId, true, i, flippedEdges, tile));

                    defaultEdges = defaultEdges.Rotate(tileWidth);
                    flippedEdges = flippedEdges.Rotate(tileWidth);
                }
            }

            long part1 = 1;

            // after this, topLeft will contain the tile we will put in the top-left of the image
            TileRotationData? topLeft = default;
            foreach ((_, List<TileRotationData> tileRotations) in leftEdges)
            {
                if (tileRotations.Count == 1)
                {
                    // it is a corner tile if both the left and top edge are unique
                    // this will happen twice for each corner tile for flipped and not flipped, so only count the not flipped case
                    TileRotationData tile = tileRotations[0];
                    if (topEdges[tile.Edges.Top].Count == 1 && !tile.IsFlipped)
                    {
                        part1 *= tile.Id;
                        topLeft = tile;
                    }
                }
            }

            TileRotationData[,] tileLocations = GetTileLocations(topLeft!, leftEdges, topEdges, numTiles);

            int part2 = SolvePart2(tileLocations);

            return new Solution(part1.ToString(), part2.ToString());
        }

        private static int SolvePart2(TileRotationData[,] tileLocations)
        {
            bool[,] sea = GetSeaGrid(tileLocations);

            int seaMonsterCells = CountSeaMonsters(sea, 1, 0, false);
            if (seaMonsterCells == 0) seaMonsterCells = CountSeaMonsters(sea, 0, 1, false);
            if (seaMonsterCells == 0) seaMonsterCells = CountSeaMonsters(sea, -1, 0, false);
            if (seaMonsterCells == 0) seaMonsterCells = CountSeaMonsters(sea, 0, -1, false);
            if (seaMonsterCells == 0) seaMonsterCells = CountSeaMonsters(sea, 1, 0, true);
            if (seaMonsterCells == 0) seaMonsterCells = CountSeaMonsters(sea, 0, 1, true);
            if (seaMonsterCells == 0) seaMonsterCells = CountSeaMonsters(sea, -1, 0, true);
            if (seaMonsterCells == 0) seaMonsterCells = CountSeaMonsters(sea, 0, -1, true);

            int totalSea = 0;
            foreach (bool cell in sea)
            {
                if (cell)
                {
                    totalSea++;
                }
            }

            return totalSea - seaMonsterCells;
        }

        private static int CountSeaMonsters(bool[,] sea, int dx, int dy, bool isFlipped)
        {
            const int monsterHeight = 3;
            const int monsterWidth = 20;

            int dx2 = 0;
            int dy2 = 0;
            if (dx == 0)
            {
                dx2 = isFlipped ? -1 : 1;
            }
            else
            {
                dy2 = isFlipped ? -1 : 1;
            }

            var seaMonsters = new HashSet<int>();
            var potentialSeaMonsters = new HashSet<int>();

            int len = sea.GetLength(0);
            for (int y = 0; y < len; y++)
            {
                int yEnd = y + (dy == 0 ? dy2 * monsterHeight : dy * monsterWidth);
                if (yEnd < 0 || yEnd >= len)
                {
                    continue;
                }

                for (int x = 0; x < len; x++)
                {
                    int xEnd = x + (dx == 0 ? dx2 * monsterHeight : dx * monsterWidth);
                    if (xEnd < 0 || xEnd >= len)
                    {
                        continue;
                    }

                    if (IsSeaMonster(sea, x, y, dx, dy, dx2, dy2, potentialSeaMonsters))
                    {
                        seaMonsters.UnionWith(potentialSeaMonsters);
                    }

                    potentialSeaMonsters.Clear();
                }
            }

            return seaMonsters.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSeaMonster(bool[,] sea, int x, int y, int dx, int dy, int dx2, int dy2, HashSet<int> potentialSeaMonsters)
        {
            // get the cell on the first row belonging to the sea monster's head
            if (!sea[y + dy * 18, x + dx * 18])
                return false;
            potentialSeaMonsters.Add((x + dx * 18) << 16 | y + dy * 18);

            x += dx2;
            y += dy2;

            // check the 2nd and 3rd row of the monster
            for (int monsterX = 0; monsterX < 20; monsterX++)
            {
                if (monsterX is 0 or 5 or 6 or 11 or 12 or 17 or 18 or 19)
                {
                    if (!sea[y, x])
                        return false;
                    potentialSeaMonsters.Add(x << 16 | y);
                }
                else if ((monsterX - 1) % 3 == 0)
                {
                    if (!sea[y + dy2, x + dx2])
                        return false;
                    potentialSeaMonsters.Add((x + dx2) << 16 | y + dy2);
                }

                x += dx;
                y += dy;
            }

            return true;
        }

        private static bool[,] GetSeaGrid(TileRotationData[,] tileLocations)
        {
            int subTileWidth = tileLocations[0, 0].Data.Length - 2;
            int tilesPerRow = tileLocations.GetLength(0);
            int seaWidth = subTileWidth * tilesPerRow;

            bool[,] sea = new bool[seaWidth, seaWidth];

            for (int y = 0; y < tilesPerRow; y++)
            {
                int yStart = y * subTileWidth;
                for (int x = 0; x < tilesPerRow; x++)
                {
                    int xStart = x * subTileWidth;
                    TileRotationData tile = tileLocations[y, x];

                    // apply flip
                    int[] tileData = tile.Data;
                    if (tile.IsFlipped)
                    {
                        FlipTile(tileData);
                    }

                    // apply rotations
                    for (int i = 0; i < tile.Rotations; i++)
                    {
                        // rotating is the same as transposing, then reversing each row
                        TransposeTile(tileData);
                        FlipTile(tileData);
                    }

                    // insert into the sea grid
                    for (int i = 0; i < subTileWidth; i++)
                    {
                        int row = tileData[i + 1];
                        for (int j = 0; j < subTileWidth; j++)
                        {
                            sea[yStart + i, xStart + j] = (row & (1 << (tileData.Length - j - 2))) != 0;
                        }
                    }
                }
            }

            return sea;
        }

        // Algorithm from: http://graphics.stanford.edu/~seander/bithacks.html#ReverseParallel
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReverseBits(int v, int width)
        {
            // swap odd and even bits
            v = ((v >> 1) & 0x5555) | ((v & 0x5555) << 1);
            // swap consecutive pairs
            v = ((v >> 2) & 0x3333) | ((v & 0x3333) << 2);
            // swap nibbles
            v = ((v >> 4) & 0x0F0F) | ((v & 0x0F0F) << 4);
            // swap bytes
            v = ((v >> 8) & 0x00FF) | ((v & 0x00FF) << 8);

            return v >> (16 - width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TransposeTile(int[] tile)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int GetBit(int n, int i) => (n >> i) & 1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void SetBit(ref int n, int i, int bit) => n = (n & ~(1 << i)) | bit << i;

            int temp;
            int n = tile.Length;
            for (int i = 0; i < tile.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    temp = tile[i];
                    SetBit(ref tile[i], n - j - 1, GetBit(tile[j], n - i - 1));
                    SetBit(ref tile[j], n - i - 1, GetBit(temp, n - j - 1));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FlipTile(int[] tile)
        {
            for (int i = 0; i < tile.Length; i++)
            {
                tile[i] = ReverseBits(tile[i], tile.Length);
            }
        }

        private static (int TileId, int[] Tile)[] ParseTiles(ReadOnlySpan<char> input)
        {
            // tile side length is the length between the first and second newline
            int tileWidth = input.Slice(input.IndexOf('\n') + 1).IndexOf('\n');

            int numTiles = input.Count('T'); // T will appear once per tile

            var tiles = new (int, int[])[numTiles];

            var reader = new SpanReader(input);
            for (int i = 0; i < numTiles; i++)
            {
                reader.SkipLength("Tile ".Length);
                int tileId = reader.ReadPosIntUntil(':');
                reader.SkipLength(1); // skip newline

                int[] tile = new int[tileWidth];
                for (int row = 0; row < tileWidth; row++)
                {
                    int rowData = 0;
                    for (int col = 0; col < tileWidth; col++)
                    {
                        // ('#' & 1) == 1; ('.' & 1) == 0;
                        rowData |= (reader[col] & 1) << (tileWidth - col - 1);
                    }

                    tile[row] = rowData;
                    reader.SkipLength(tileWidth + 1);
                }

                reader.SkipLength(1); // skip newline
                tiles[i] = (tileId, tile);
            }

            return tiles;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TileEdges GetEdges(int[] tile)
        {
            int top = tile[0];
            int bottom = tile[tile.Length - 1];
            int left = 0;
            int right = 0;

            int leftFlag = 1 << (tile.Length - 1);
            for (int i = 0; i < tile.Length; i++)
            {
                int row = tile[i];
                left |= (row & leftFlag) >> i;
                right |= (row & 1) << (tile.Length - i - 1);
            }

            return new TileEdges(top, bottom, left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToDictOfLists<K, V>(Dictionary<K, List<V>> dict, K key, V value)
            where K : notnull
        {
            if (dict.TryGetValue(key, out List<V>? lst))
            {
                lst.Add(value);
            }
            else
            {
                dict[key] = new List<V> { value };
            }
        }

        private static TileRotationData[,] GetTileLocations(TileRotationData topLeft, Dictionary<int, List<TileRotationData>> leftEdges, Dictionary<int, List<TileRotationData>> topEdges, int numTiles)
        {
            int gridSize = (int)Math.Sqrt(numTiles);

            TileRotationData[,] tileLocations = new TileRotationData[gridSize, gridSize];
            tileLocations[0, 0] = topLeft;

            int x = 1;
            int y = 0;
            for (int i = 1; i < numTiles; i++)
            {
                TileRotationData neighbour;
                List<TileRotationData> candidates;
                if (y == 0)
                {
                    neighbour = tileLocations[y, x - 1];
                    candidates = leftEdges[neighbour.Edges.Right];
                }
                else
                {
                    neighbour = tileLocations[y - 1, x];
                    candidates = topEdges[neighbour.Edges.Bottom];
                }

                foreach (TileRotationData tile in candidates)
                {
                    if (tile.Id != neighbour.Id)
                    {
                        tileLocations[y, x] = tile;
                        break;
                    }
                }

                x++;
                if (x >= gridSize)
                {
                    x = 0;
                    y++;
                }
            }

            return tileLocations;
        }
    }
}
