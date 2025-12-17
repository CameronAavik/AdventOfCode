# Advent of Code

These are my solutions in C# and F# for Advent of Code 2015-2024.

You can find benchmarks at the bottom of this README.

About my solutions:
- My solutions tend to favour high performance over readability, I use Advent of Code as a way to practice my skills in writing high-performance code.
- If you are struggling to understand any of my solutions, feel free to create an issue to ask about it and I will provide an explanation for you.
- Oftentimes my solutions may not solve the general case as most of the problems are not clear on all the input constraints, however it will solve all actual inputs that could appear.
- Feel free to copy my solutions if you are stuck, credit is always appreciated but not required since many of my approaches are not unique.
- All my solutions are single-threaded, but they will often make use of SIMD as a way to emulate parallelism on a single thread.
- This repo follows the [automation guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/automation/) on the /r/adventofcode community wiki.
  You can find input caching and `User-Agent` configuration implemented in [AdventRunner.GetInputAsync()](./csharp/Runner/AdventRunner.cs).
  There is no polling implemented in this repository for which throttling is required.

## Solutions

| Day | 2015 | 2016 | 2017 | 2018 | 2019 | 2020 | 2021 | 2022 | 2023 | 2024 | 2025 |
| ---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| 1  | [C#](/csharp/2015/Solvers/Day01.cs) \| [F#](/fsharp/2015/Solutions/Day01.fs) | [C#](/csharp/2016/Solvers/Day01.cs) \| [F#](/fsharp/2016/Solutions/Day01.fs) | [C#](/csharp/2017/Solvers/Day01.cs) \| [F#](/fsharp/2017/Solutions/Day01.fs) | [C#](/csharp/2018/Solvers/Day01.cs) \| [F#](/fsharp/2018/Solutions/Day01.fs) | [C#](/csharp/2019/Solvers/Day01.cs) \| [F#](/fsharp/2019/Solutions/Day01.fs) | [C#](/csharp/2020/Solvers/Day01.cs) | [C#](/csharp/2021/Solvers/Day01.cs) | [C#](/csharp/2022/Solvers/Day01.cs) | [C#](/csharp/2023/Solvers/Day01.cs) | [C#](/csharp/2024/Solvers/Day01.cs) | [C#](/csharp/2025/Solvers/Day01.cs) |
| 2  | [C#](/csharp/2015/Solvers/Day02.cs) \| [F#](/fsharp/2015/Solutions/Day02.fs) | [C#](/csharp/2016/Solvers/Day02.cs) \| [F#](/fsharp/2016/Solutions/Day02.fs) | [C#](/csharp/2017/Solvers/Day02.cs) \| [F#](/fsharp/2017/Solutions/Day02.fs) | [C#](/csharp/2018/Solvers/Day02.cs) \| [F#](/fsharp/2018/Solutions/Day02.fs) | [C#](/csharp/2019/Solvers/Day02.cs) \| [F#](/fsharp/2019/Solutions/Day02.fs) | [C#](/csharp/2020/Solvers/Day02.cs) | [C#](/csharp/2021/Solvers/Day02.cs) | [C#](/csharp/2022/Solvers/Day02.cs) | [C#](/csharp/2023/Solvers/Day02.cs) | [C#](/csharp/2024/Solvers/Day02.cs) | [C#](/csharp/2025/Solvers/Day02.cs) |
| 3  | [C#](/csharp/2015/Solvers/Day03.cs) \| [F#](/fsharp/2015/Solutions/Day03.fs) | [C#](/csharp/2016/Solvers/Day03.cs) \| [F#](/fsharp/2016/Solutions/Day03.fs) | [F#](/fsharp/2017/Solutions/Day03.fs) | [F#](/fsharp/2018/Solutions/Day03.fs) | [F#](/fsharp/2019/Solutions/Day03.fs) | [C#](/csharp/2020/Solvers/Day03.cs) | [C#](/csharp/2021/Solvers/Day03.cs) | [C#](/csharp/2022/Solvers/Day03.cs) | [C#](/csharp/2023/Solvers/Day03.cs) | [C#](/csharp/2024/Solvers/Day03.cs) | [C#](/csharp/2025/Solvers/Day03.cs) |
| 4  | [C#](/csharp/2015/Solvers/Day04.cs) \| [F#](/fsharp/2015/Solutions/Day04.fs) | [C#](/csharp/2016/Solvers/Day04.cs) \| [F#](/fsharp/2016/Solutions/Day04.fs) | [F#](/fsharp/2017/Solutions/Day04.fs) | [F#](/fsharp/2018/Solutions/Day04.fs) | [F#](/fsharp/2019/Solutions/Day04.fs) | [C#](/csharp/2020/Solvers/Day04.cs) | [C#](/csharp/2021/Solvers/Day04.cs) | [C#](/csharp/2022/Solvers/Day04.cs) | [C#](/csharp/2023/Solvers/Day04.cs) | [C#](/csharp/2024/Solvers/Day04.cs) | [C#](/csharp/2025/Solvers/Day04.cs) |
| 5  | [C#](/csharp/2015/Solvers/Day05.cs) \| [F#](/fsharp/2015/Solutions/Day05.fs) | [C#](/csharp/2016/Solvers/Day05.cs) \| [F#](/fsharp/2016/Solutions/Day05.fs) | [F#](/fsharp/2017/Solutions/Day05.fs) | [F#](/fsharp/2018/Solutions/Day05.fs) | [F#](/fsharp/2019/Solutions/Day05.fs) | [C#](/csharp/2020/Solvers/Day05.cs) | [C#](/csharp/2021/Solvers/Day05.cs) | [C#](/csharp/2022/Solvers/Day05.cs) | [C#](/csharp/2023/Solvers/Day05.cs) | [C#](/csharp/2024/Solvers/Day05.cs) | [C#](/csharp/2025/Solvers/Day05.cs) |
| 6  | [C#](/csharp/2015/Solvers/Day06.cs) \| [F#](/fsharp/2015/Solutions/Day06.fs) | [C#](/csharp/2016/Solvers/Day06.cs) \| [F#](/fsharp/2016/Solutions/Day06.fs) | [F#](/fsharp/2017/Solutions/Day06.fs) | [F#](/fsharp/2018/Solutions/Day06.fs) | [F#](/fsharp/2019/Solutions/Day06.fs) | [C#](/csharp/2020/Solvers/Day06.cs) | [C#](/csharp/2021/Solvers/Day06.cs) | [C#](/csharp/2022/Solvers/Day06.cs) | [C#](/csharp/2023/Solvers/Day06.cs) | | [C#](/csharp/2025/Solvers/Day06.cs) |
| 7  | [C#](/csharp/2015/Solvers/Day07.cs) \| [F#](/fsharp/2015/Solutions/Day07.fs) | [C#](/csharp/2016/Solvers/Day07.cs) \| [F#](/fsharp/2016/Solutions/Day07.fs) | [F#](/fsharp/2017/Solutions/Day07.fs) | [F#](/fsharp/2018/Solutions/Day07.fs) | [F#](/fsharp/2019/Solutions/Day07.fs) | [C#](/csharp/2020/Solvers/Day07.cs) | [C#](/csharp/2021/Solvers/Day07.cs) | [C#](/csharp/2022/Solvers/Day07.cs) | [C#](/csharp/2023/Solvers/Day07.cs) | [C#](/csharp/2024/Solvers/Day07.cs) | [C#](/csharp/2025/Solvers/Day07.cs) |
| 8  | [C#](/csharp/2015/Solvers/Day08.cs) \| [F#](/fsharp/2015/Solutions/Day08.fs) | [C#](/csharp/2016/Solvers/Day08.cs) \| [F#](/fsharp/2016/Solutions/Day08.fs) | [F#](/fsharp/2017/Solutions/Day08.fs) | [F#](/fsharp/2018/Solutions/Day08.fs) | [F#](/fsharp/2019/Solutions/Day08.fs) | [C#](/csharp/2020/Solvers/Day08.cs) | [C#](/csharp/2021/Solvers/Day08.cs) | | [C#](/csharp/2023/Solvers/Day08.cs) | | [C#](/csharp/2025/Solvers/Day08.cs) |
| 9  | [C#](/csharp/2015/Solvers/Day09.cs) \| [F#](/fsharp/2015/Solutions/Day09.fs) | [C#](/csharp/2016/Solvers/Day09.cs) \| [F#](/fsharp/2016/Solutions/Day09.fs) | [F#](/fsharp/2017/Solutions/Day09.fs) | [F#](/fsharp/2018/Solutions/Day09.fs) | [F#](/fsharp/2019/Solutions/Day09.fs) | [C#](/csharp/2020/Solvers/Day09.cs) | [C#](/csharp/2021/Solvers/Day09.cs) | | [C#](/csharp/2023/Solvers/Day09.cs) | | [C#](/csharp/2025/Solvers/Day09.cs) |
| 10 | [C#](/csharp/2015/Solvers/Day10.cs) \| [F#](/fsharp/2015/Solutions/Day10.fs) | [C#](/csharp/2016/Solvers/Day10.cs) \| [F#](/fsharp/2016/Solutions/Day10.fs) | [F#](/fsharp/2017/Solutions/Day10.fs) | [F#](/fsharp/2018/Solutions/Day10.fs) | [F#](/fsharp/2019/Solutions/Day10.fs) | [C#](/csharp/2020/Solvers/Day10.cs) | [C#](/csharp/2021/Solvers/Day10.cs) | | [C#](/csharp/2023/Solvers/Day10.cs) | | [C#](/csharp/2025/Solvers/Day10.cs) |
| 11 | [C#](/csharp/2015/Solvers/Day11.cs) \| [F#](/fsharp/2015/Solutions/Day11.fs) | [C#](/csharp/2016/Solvers/Day11.cs) \| [F#](/fsharp/2016/Solutions/Day11.fs) | [F#](/fsharp/2017/Solutions/Day11.fs) | [F#](/fsharp/2018/Solutions/Day11.fs) | [F#](/fsharp/2019/Solutions/Day11.fs) | [C#](/csharp/2020/Solvers/Day11.cs) | [C#](/csharp/2021/Solvers/Day11.cs) | | [C#](/csharp/2023/Solvers/Day11.cs) | | |
| 12 | [C#](/csharp/2015/Solvers/Day12.cs) \| [F#](/fsharp/2015/Solutions/Day12.fs) | [C#](/csharp/2016/Solvers/Day12.cs) \| [F#](/fsharp/2016/Solutions/Day12.fs) | [F#](/fsharp/2017/Solutions/Day12.fs) | [F#](/fsharp/2018/Solutions/Day12.fs) | [F#](/fsharp/2019/Solutions/Day12.fs) | [C#](/csharp/2020/Solvers/Day12.cs) | [C#](/csharp/2021/Solvers/Day12.cs) | | [C#](/csharp/2023/Solvers/Day12.cs) | | |
| 13 | [C#](/csharp/2015/Solvers/Day13.cs) \| [F#](/fsharp/2015/Solutions/Day13.fs) | [C#](/csharp/2016/Solvers/Day13.cs) \| [F#](/fsharp/2016/Solutions/Day13.fs) | [F#](/fsharp/2017/Solutions/Day13.fs) | [F#](/fsharp/2018/Solutions/Day13.fs) | [F#](/fsharp/2019/Solutions/Day13.fs) | [C#](/csharp/2020/Solvers/Day13.cs) | [C#](/csharp/2021/Solvers/Day13.cs) | | [C#](/csharp/2023/Solvers/Day13.cs) | |
| 14 | [C#](/csharp/2015/Solvers/Day14.cs) \| [F#](/fsharp/2015/Solutions/Day14.fs) | [F#](/fsharp/2016/Solutions/Day14.fs) | [F#](/fsharp/2017/Solutions/Day14.fs) | [F#](/fsharp/2018/Solutions/Day14.fs) | [F#](/fsharp/2019/Solutions/Day14.fs) | [C#](/csharp/2020/Solvers/Day14.cs) | [C#](/csharp/2021/Solvers/Day14.cs) | | [C#](/csharp/2023/Solvers/Day14.cs) | |
| 15 | [C#](/csharp/2015/Solvers/Day15.cs) \| [F#](/fsharp/2015/Solutions/Day15.fs) | [F#](/fsharp/2016/Solutions/Day15.fs) | [F#](/fsharp/2017/Solutions/Day15.fs) | [F#](/fsharp/2018/Solutions/Day15.fs) | [F#](/fsharp/2019/Solutions/Day15.fs) | [C#](/csharp/2020/Solvers/Day15.cs) | [C#](/csharp/2021/Solvers/Day15.cs) | | [C#](/csharp/2023/Solvers/Day15.cs) | |
| 16 | [C#](/csharp/2015/Solvers/Day16.cs) \| [F#](/fsharp/2015/Solutions/Day16.fs) | [F#](/fsharp/2016/Solutions/Day16.fs) | [F#](/fsharp/2017/Solutions/Day16.fs) | [F#](/fsharp/2018/Solutions/Day16.fs) | [F#](/fsharp/2019/Solutions/Day16.fs) | [C#](/csharp/2020/Solvers/Day16.cs) | [C#](/csharp/2021/Solvers/Day16.cs) | | [C#](/csharp/2023/Solvers/Day16.cs) | |
| 17 | [C#](/csharp/2015/Solvers/Day17.cs) \| [F#](/fsharp/2015/Solutions/Day17.fs) | [F#](/fsharp/2016/Solutions/Day17.fs) | [F#](/fsharp/2017/Solutions/Day17.fs) | [F#](/fsharp/2018/Solutions/Day17.fs) | [F#](/fsharp/2019/Solutions/Day17.fs) | [C#](/csharp/2020/Solvers/Day17.cs) | [C#](/csharp/2021/Solvers/Day17.cs) | | [C#](/csharp/2023/Solvers/Day17.cs) | |
| 18 | [C#](/csharp/2015/Solvers/Day18.cs) \| [F#](/fsharp/2015/Solutions/Day18.fs) | [F#](/fsharp/2016/Solutions/Day18.fs) | [F#](/fsharp/2017/Solutions/Day18.fs) | [F#](/fsharp/2018/Solutions/Day18.fs) | [F#](/fsharp/2019/Solutions/Day18.fs) | [C#](/csharp/2020/Solvers/Day18.cs) | [C#](/csharp/2021/Solvers/Day18.cs) | | [C#](/csharp/2023/Solvers/Day18.cs) | |
| 19 | [C#](/csharp/2015/Solvers/Day19.cs) \| [F#](/fsharp/2015/Solutions/Day19.fs) | [F#](/fsharp/2016/Solutions/Day19.fs) | [F#](/fsharp/2017/Solutions/Day19.fs) | [F#](/fsharp/2018/Solutions/Day19.fs) | [F#](/fsharp/2019/Solutions/Day19.fs) | [C#](/csharp/2020/Solvers/Day19.cs) | [C#](/csharp/2021/Solvers/Day19.cs) | [C#](/csharp/2022/Solvers/Day19.cs) | [C#](/csharp/2023/Solvers/Day19.cs) | |
| 20 | [C#](/csharp/2015/Solvers/Day20.cs) \| [F#](/fsharp/2015/Solutions/Day20.fs) | [F#](/fsharp/2016/Solutions/Day20.fs) | [F#](/fsharp/2017/Solutions/Day20.fs) | [F#](/fsharp/2018/Solutions/Day20.fs) | [F#](/fsharp/2019/Solutions/Day20.fs) | [C#](/csharp/2020/Solvers/Day20.cs) | [C#](/csharp/2021/Solvers/Day20.cs) | [C#](/csharp/2022/Solvers/Day20.cs) | [C#](/csharp/2023/Solvers/Day20.cs) | |
| 21 | [C#](/csharp/2015/Solvers/Day21.cs) \| [F#](/fsharp/2015/Solutions/Day21.fs) | [F#](/fsharp/2016/Solutions/Day21.fs) | [F#](/fsharp/2017/Solutions/Day21.fs) | [F#](/fsharp/2018/Solutions/Day21.fs) | [F#](/fsharp/2019/Solutions/Day21.fs) | [C#](/csharp/2020/Solvers/Day21.cs) | [C#](/csharp/2021/Solvers/Day21.cs) | | [C#](/csharp/2023/Solvers/Day21.cs) | |
| 22 | [C#](/csharp/2015/Solvers/Day22.cs) \| [F#](/fsharp/2015/Solutions/Day22.fs) | [F#](/fsharp/2016/Solutions/Day22.fs) | [F#](/fsharp/2017/Solutions/Day22.fs) | [F#](/fsharp/2018/Solutions/Day22.fs) | [F#](/fsharp/2019/Solutions/Day22.fs) | [C#](/csharp/2020/Solvers/Day22.cs) | [C#](/csharp/2021/Solvers/Day22.cs) | | [C#](/csharp/2023/Solvers/Day22.cs) | |
| 23 | [C#](/csharp/2015/Solvers/Day23.cs) \| [F#](/fsharp/2015/Solutions/Day23.fs) | [F#](/fsharp/2016/Solutions/Day23.fs) | [F#](/fsharp/2017/Solutions/Day23.fs) | [F#](/fsharp/2018/Solutions/Day23.fs) | [F#](/fsharp/2019/Solutions/Day23.fs) | [C#](/csharp/2020/Solvers/Day23.cs) | [C#](/csharp/2021/Solvers/Day23.cs) | | [C#](/csharp/2023/Solvers/Day23.cs) | |
| 24 | [C#](/csharp/2015/Solvers/Day24.cs) \| [F#](/fsharp/2015/Solutions/Day24.fs) | [F#](/fsharp/2016/Solutions/Day24.fs) | [F#](/fsharp/2017/Solutions/Day24.fs) | [F#](/fsharp/2018/Solutions/Day24.fs) | [F#](/fsharp/2019/Solutions/Day24.fs) | [C#](/csharp/2020/Solvers/Day24.cs) | [C#](/csharp/2021/Solvers/Day24.cs) | | [C#](/csharp/2023/Solvers/Day24.cs) | |
| 25 | [C#](/csharp/2015/Solvers/Day25.cs) \| [F#](/fsharp/2015/Solutions/Day25.fs) | [F#](/fsharp/2016/Solutions/Day25.fs) | [F#](/fsharp/2017/Solutions/Day25.fs) | [F#](/fsharp/2018/Solutions/Day25.fs) | [F#](/fsharp/2019/Solutions/Day25.fs) | [C#](/csharp/2020/Solvers/Day25.cs) | [C#](/csharp/2021/Solvers/Day25.cs) | | [C#](/csharp/2023/Solvers/Day25.cs) | |

## C# Benchmarks

### Benchmarking Environment

``` ini
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7171/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 5950X 3.40GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
```

### 2025

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **9.77 μs** | **9.77 μs** | **9.79 μs** | **--** |
| **2** | **0.31 μs** | **0.32 μs** | **0.32 μs** | **--** |
| **3** | **77.75 μs** | **78.70 μs** | **79.88 μs** | **--** |
| **4** | **231.67 μs** | **233.66 μs** | **244.78 μs** | **94.5 KB** |
| **5** | **19.64 μs** | **19.69 μs** | **19.87 μs** | **4.1 KB** |
| **6** | **19.50 μs** | **19.55 μs** | **19.64 μs** | **--** |
| **7** | **2.40 μs** | **2.44 μs** | **2.50 μs** | **--** |
| **8** | **793.67 μs** | **797.98 μs** | **834.26 μs** | **393.9 KB** |
| **9** | **100.16 μs** | **100.39 μs** | **100.73 μs** | **34.9 KB** |
| **10** | **69.98 μs** | **70.22 μs** | **71.16 μs** | **376 B** |

### 2024
| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **15.13 μs** | **15.24 μs** | **15.59 μs** | **17.6 KB** |
| **2** | **15.28 μs** | **15.45 μs** | **15.88 μs** | **--** |
| **3** | **7.15 μs** | **7.22 μs** | **7.31 μs** | **--** |
| **4** | **6.47 μs** | **6.56 μs** | **6.78 μs** | **--** |
| **5** | **10.93 μs** | **11.05 μs** | **12.31 μs** | **--** |
| **7** | **40.62 μs** | **41.40 μs** | **46.08 μs** | **--** |

### 2023

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **15.69 μs** | **15.78 μs** | **15.95 μs** | **--** |
| **2** | **2.64 μs** | **2.79 μs** | **2.86 μs** | **--** |
| **3** | **9.24 μs** | **9.30 μs** | **9.44 μs** | **584 B** |
| **4** | **10.94 μs** | **11.09 μs** | **12.30 μs** | **--** |
| **5** | **7.48 μs** | **7.57 μs** | **7.75 μs** | **8.3 KB** |
| **6** | **0.11 μs** | **0.11 μs** | **0.11 μs** | **--** |
| **7** | **30.45 μs** | **30.83 μs** | **31.28 μs** | **16.0 KB** |
| **8** | **98.79 μs** | **99.74 μs** | **104.74 μs** | **21.5 KB** |
| **9** | **19.74 μs** | **20.00 μs** | **20.54 μs** | **--** |
| **10** | **20.90 μs** | **20.98 μs** | **21.29 μs** | **--** |
| **11** | **2.30 μs** | **2.32 μs** | **2.35 μs** | **584 B** |
| **12** | **1,221.36 μs** | **1,230.64 μs** | **1,249.56 μs** | **162.7 KB** |
| **13** | **8.35 μs** | **8.41 μs** | **8.56 μs** | **--** |
| **14** | **1,672.49 μs** | **1,679.72 μs** | **1,694.68 μs** | **11.7 KB** |
| **15** | **25.20 μs** | **25.48 μs** | **26.00 μs** | **65.6 KB** |
| **16** | **248.97 μs** | **250.24 μs** | **254.56 μs** | **908.4 KB** |
| **17** | **3,548.23 μs** | **3,557.76 μs** | **3,590.55 μs** | **411.7 KB** |
| **18** | **2.67 μs** | **2.68 μs** | **2.72 μs** | **--** |
| **19** | **29.31 μs** | **29.51 μs** | **29.73 μs** | **--** |
| **20** | **3.26 μs** | **3.27 μs** | **3.30 μs** | **15.9 KB** |
| **21** | **15.01 μs** | **15.09 μs** | **15.53 μs** | **--** |
| **22** | **26.03 μs** | **26.36 μs** | **27.11 μs** | **17.3 KB** |
| **23** | **190.34 μs** | **192.65 μs** | **197.80 μs** | **22.9 KB** |
| **24** | **157.77 μs** | **159.51 μs** | **161.61 μs** | **13.0 KB** |
| **25** | **200.96 μs** | **202.67 μs** | **206.26 μs** | **276.3 KB** |
| **All** | **8,017.65 μs** | **8,034.16 μs** | **8,072.53 μs** | **1.9 MB** |

2023 benchmarks are a result of measuring my solver on 5 separate inputs in a row.
This means the results should be more representative and less susceptible to branch prediction biases.

### 2022

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **5.08 μs** | **5.48 μs** | **5.77 μs** | **--** |
| **2** | **4.93 μs** | **4.96 μs** | **5.00 μs** | **--** |
| **3** | **4.61 μs** | **4.65 μs** | **4.68 μs** | **--** |
| **4** | **3.97 μs** | **4.02 μs** | **4.15 μs** | **--** |
| **5** | **5.60 μs** | **5.68 μs** | **5.73 μs** | **1.8 KB** |
| **6** | **6.07 μs** | **6.10 μs** | **6.15 μs** | **--** |
| **7** | **5.24 μs** | **5.28 μs** | **5.33 μs** | **1.1 KB** |
| **19** | **85.89 μs** | **86.40 μs** | **88.39 μs** | **177.5 KB** |
| **20** | **5,226.16 μs** | **5,245.14 μs** | **5,273.71 μs** | **105.9 KB** |

### 2021

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **3.35 μs** | **3.37 μs** | **3.39 μs** | **--** |
| **2** | **0.80 μs** | **0.81 μs** | **0.81 μs** | **--** |
| **3** | **7.34 μs** | **7.39 μs** | **7.48 μs** | **--** |
| **4** | **6.11 μs** | **6.16 μs** | **6.20 μs** | **--** |
| **5** | **178.60 μs** | **179.63 μs** | **180.37 μs** | **23.2 KB** |
| **6** | **0.32 μs** | **0.32 μs** | **0.32 μs** | **--** |
| **7** | **10.70 μs** | **10.85 μs** | **11.23 μs** | **8.4 KB** |
| **8** | **9.90 μs** | **9.96 μs** | **10.01 μs** | **--** |
| **9** | **16.39 μs** | **16.47 μs** | **16.89 μs** | **--** |
| **10** | **12.28 μs** | **12.91 μs** | **14.27 μs** | **--** |
| **11** | **16.68 μs** | **16.78 μs** | **16.99 μs** | **--** |
| **12** | **19.37 μs** | **19.52 μs** | **19.91 μs** | **50.6 KB** |
| **13** | **9.16 μs** | **9.28 μs** | **9.53 μs** | **27.7 KB** |
| **14** | **6.01 μs** | **6.03 μs** | **6.06 μs** | **--** |
| **15** | **3,579.26 μs** | **3,610.55 μs** | **3,647.05 μs** | **--** |
| **16** | **5.82 μs** | **5.83 μs** | **5.85 μs** | **--** |
| **17** | **1.28 μs** | **1.29 μs** | **1.30 μs** | **--** |
| **18** | **1,211.07 μs** | **1,220.80 μs** | **1,234.70 μs** | **--** |
| **19** | **2,301.07 μs** | **2,417.58 μs** | **2,522.23 μs** | **78.6 KB** |
| **20** | **2,001.82 μs** | **2,018.46 μs** | **2,033.79 μs** | **--** |
| **21** | **519.79 μs** | **520.98 μs** | **524.82 μs** | **--** |
| **22** | **1,157.07 μs** | **1,158.51 μs** | **1,163.94 μs** | **702.3 KB** |
| **23** | **1,114.05 μs** | **1,118.17 μs** | **1,148.80 μs** | **599.4 KB** |
| **24** | **0.05 μs** | **0.05 μs** | **0.05 μs** | **--** |
| **25** | **341.46 μs** | **343.67 μs** | **346.71 μs** | **--** |

### 2020

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **2.21 μs** | **2.22 μs** | **2.25 μs** | **4.1 KB** |
| **2** | **9.37 μs** | **9.41 μs** | **9.46 μs** | **--** |
| **3** | **0.96 μs** | **0.96 μs** | **0.97 μs** | **--** |
| **4** | **17.89 μs** | **18.04 μs** | **18.64 μs** | **--** |
| **5** | **4.10 μs** | **4.11 μs** | **4.12 μs** | **--** |
| **6** | **8.20 μs** | **8.22 μs** | **8.27 μs** | **--** |
| **7** | **151.48 μs** | **153.20 μs** | **156.29 μs** | **363.1 KB** |
| **8** | **2.41 μs** | **2.42 μs** | **2.43 μs** | **9.9 KB** |
| **9** | **13.58 μs** | **13.62 μs** | **13.73 μs** | **8.0 KB** |
| **10** | **0.66 μs** | **0.66 μs** | **0.69 μs** | **456 B** |
| **11** | **4,351.47 μs** | **4,363.01 μs** | **4,423.28 μs** | **878.5 KB** |
| **12** | **1.89 μs** | **1.90 μs** | **1.91 μs** | **--** |
| **13** | **1.12 μs** | **1.13 μs** | **1.15 μs** | **1.3 KB** |
| **14** | **2,357.38 μs** | **2,478.76 μs** | **2,583.43 μs** | **8.5 MB** |
| **15** | **185,442.80 μs** | **187,125.30 μs** | **189,936.50 μs** | **120.0 MB** |
| **16** | **22.52 μs** | **22.73 μs** | **23.30 μs** | **5.6 KB** |
| **17** | **97.09 μs** | **98.67 μs** | **100.33 μs** | **527.5 KB** |
| **18** | **37.98 μs** | **39.80 μs** | **40.51 μs** | **1.6 KB** |
| **19** | **362.20 μs** | **364.10 μs** | **385.11 μs** | **15.1 KB** |
| **20** | **473.96 μs** | **476.81 μs** | **489.69 μs** | **414.9 KB** |
| **21** | **134.19 μs** | **136.08 μs** | **140.47 μs** | **142.4 KB** |
| **22** | **9,420.17 μs** | **9,483.58 μs** | **9,692.30 μs** | **9.9 MB** |
| **23** | **137,736.35 μs** | **138,419.77 μs** | **139,212.15 μs** | **4.0 MB** |
| **24** | **3,072.52 μs** | **3,080.75 μs** | **3,119.53 μs** | **65.6 KB** |
| **25** | **1,035.56 μs** | **1,039.21 μs** | **1,044.09 μs** | **--** |

### 2019

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **0.85 μs** | **0.85 μs** | **0.86 μs** | **--** |
| **2** | **541.20 μs** | **542.00 μs** | **549.33 μs** | **1.6 KB** |

### 2018

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **28.68 μs** | **29.45 μs** | **29.87 μs** | **47.3 KB** |
| **2** | **15.37 μs** | **15.43 μs** | **15.47 μs** | **2.0 KB** |

### 2017

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **5.15 μs** | **5.16 μs** | **5.19 μs** | **--** |
| **2** | **2.92 μs** | **2.93 μs** | **2.95 μs** | **216 B** |

### 2016

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **4.11 μs** | **4.17 μs** | **4.21 μs** | **27.7 KB** |
| **2** | **5.75 μs** | **5.77 μs** | **5.80 μs** | **--** |
| **3** | **33.54 μs** | **34.37 μs** | **36.94 μs** | **--** |
| **4** | **63.61 μs** | **63.92 μs** | **64.21 μs** | **128 B** |
| **5** | **389,624.20 μs** | **390,698.40 μs** | **391,177.60 μs** | **744 B** |
| **6** | **4.99 μs** | **5.01 μs** | **5.03 μs** | **1.1 KB** |
| **7** | **300.93 μs** | **302.06 μs** | **304.86 μs** | **1.4 KB** |
| **8** | **8.05 μs** | **8.07 μs** | **8.12 μs** | **344 B** |
| **9** | **28.52 μs** | **28.84 μs** | **29.40 μs** | **--** |
| **10** | **5.72 μs** | **5.77 μs** | **5.81 μs** | **30.6 KB** |
| **11** | **15,286.89 μs** | **15,386.05 μs** | **15,539.35 μs** | **11.0 MB** |
| **12** | **30,895.29 μs** | **31,066.14 μs** | **31,853.72 μs** | **488 B** |
| **13** | **16.02 μs** | **16.11 μs** | **16.20 μs** | **70.2 KB** |

### 2015

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **3.79 μs** | **3.82 μs** | **3.85 μs** | **--** |
| **2** | **5.19 μs** | **5.20 μs** | **5.20 μs** | **--** |
| **3** | **100.01 μs** | **104.03 μs** | **106.93 μs** | **123.5 KB** |
| **4** | **140,766.15 μs** | **140,911.20 μs** | **141,351.90 μs** | **472 B** |
| **5** | **113.90 μs** | **114.25 μs** | **114.60 μs** | **--** |
| **6** | **17,413.24 μs** | **17,466.34 μs** | **17,516.42 μs** | **4.0 MB** |
| **7** | **45.55 μs** | **45.94 μs** | **46.26 μs** | **106.9 KB** |
| **8** | **6.81 μs** | **6.97 μs** | **7.28 μs** | **--** |
| **9** | **424.56 μs** | **426.42 μs** | **449.27 μs** | **9.3 KB** |
| **10** | **23,711.97 μs** | **24,054.05 μs** | **24,194.37 μs** | **46.1 MB** |
| **11** | **418.49 μs** | **419.89 μs** | **436.38 μs** | **--** |
| **12** | **94.35 μs** | **95.10 μs** | **96.15 μs** | **--** |
| **13** | **5,121.54 μs** | **5,126.22 μs** | **5,138.65 μs** | **11.1 KB** |
| **14** | **34.61 μs** | **34.74 μs** | **34.86 μs** | **1.1 KB** |
| **15** | **1,420.59 μs** | **1,422.32 μs** | **1,425.23 μs** | **288 B** |
| **16** | **37.58 μs** | **38.18 μs** | **38.78 μs** | **58.0 KB** |
| **17** | **18.67 μs** | **19.10 μs** | **19.59 μs** | **74.2 KB** |
| **18** | **5,697.55 μs** | **5,704.43 μs** | **5,724.68 μs** | **83.4 KB** |
| **19** | **10.48 μs** | **10.57 μs** | **10.64 μs** | **24.6 KB** |
| **20** | **1,619.44 μs** | **1,628.32 μs** | **1,631.02 μs** | **181.7 KB** |
| **21** | **4.04 μs** | **4.05 μs** | **4.07 μs** | **21.1 KB** |
| **22** | **1,754.93 μs** | **1,765.37 μs** | **1,772.08 μs** | **3.0 MB** |
| **23** | **4.59 μs** | **4.76 μs** | **4.86 μs** | **3.1 KB** |
| **24** | **2,161.86 μs** | **2,170.68 μs** | **2,180.56 μs** | **16.6 KB** |
| **25** | **0.16 μs** | **0.16 μs** | **0.16 μs** | **32 B** |
