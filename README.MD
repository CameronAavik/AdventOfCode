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

| Day | 2015 | 2016 | 2017 | 2018 | 2019 | 2020 | 2021 | 2022 | 2023 | 2024 |
| ---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| 1  | [C#](/csharp/2015/Solvers/Day01.cs) \| [F#](/fsharp/2015/Solutions/Day01.fs) | [C#](/csharp/2016/Solvers/Day01.cs) \| [F#](/fsharp/2016/Solutions/Day01.fs) | [C#](/csharp/2017/Solvers/Day01.cs) \| [F#](/fsharp/2017/Solutions/Day01.fs) | [C#](/csharp/2018/Solvers/Day01.cs) \| [F#](/fsharp/2018/Solutions/Day01.fs) | [C#](/csharp/2019/Solvers/Day01.cs) \| [F#](/fsharp/2019/Solutions/Day01.fs) | [C#](/csharp/2020/Solvers/Day01.cs) | [C#](/csharp/2021/Solvers/Day01.cs) | [C#](/csharp/2022/Solvers/Day01.cs) | [C#](/csharp/2023/Solvers/Day01.cs) | |
| 2  | [C#](/csharp/2015/Solvers/Day02.cs) \| [F#](/fsharp/2015/Solutions/Day02.fs) | [C#](/csharp/2016/Solvers/Day02.cs) \| [F#](/fsharp/2016/Solutions/Day02.fs) | [C#](/csharp/2017/Solvers/Day02.cs) \| [F#](/fsharp/2017/Solutions/Day02.fs) | [C#](/csharp/2018/Solvers/Day02.cs) \| [F#](/fsharp/2018/Solutions/Day02.fs) | [C#](/csharp/2019/Solvers/Day02.cs) \| [F#](/fsharp/2019/Solutions/Day02.fs) | [C#](/csharp/2020/Solvers/Day02.cs) | [C#](/csharp/2021/Solvers/Day02.cs) | [C#](/csharp/2022/Solvers/Day02.cs) | [C#](/csharp/2023/Solvers/Day02.cs) | |
| 3  | [C#](/csharp/2015/Solvers/Day03.cs) \| [F#](/fsharp/2015/Solutions/Day03.fs) | [C#](/csharp/2016/Solvers/Day03.cs) \| [F#](/fsharp/2016/Solutions/Day03.fs) | [F#](/fsharp/2017/Solutions/Day03.fs) | [F#](/fsharp/2018/Solutions/Day03.fs) | [F#](/fsharp/2019/Solutions/Day03.fs) | [C#](/csharp/2020/Solvers/Day03.cs) | [C#](/csharp/2021/Solvers/Day03.cs) | [C#](/csharp/2022/Solvers/Day03.cs) | [C#](/csharp/2023/Solvers/Day03.cs) | |
| 4  | [C#](/csharp/2015/Solvers/Day04.cs) \| [F#](/fsharp/2015/Solutions/Day04.fs) | [C#](/csharp/2016/Solvers/Day04.cs) \| [F#](/fsharp/2016/Solutions/Day04.fs) | [F#](/fsharp/2017/Solutions/Day04.fs) | [F#](/fsharp/2018/Solutions/Day04.fs) | [F#](/fsharp/2019/Solutions/Day04.fs) | [C#](/csharp/2020/Solvers/Day04.cs) | [C#](/csharp/2021/Solvers/Day04.cs) | [C#](/csharp/2022/Solvers/Day04.cs) | [C#](/csharp/2023/Solvers/Day04.cs) | |
| 5  | [C#](/csharp/2015/Solvers/Day05.cs) \| [F#](/fsharp/2015/Solutions/Day05.fs) | [C#](/csharp/2016/Solvers/Day05.cs) \| [F#](/fsharp/2016/Solutions/Day05.fs) | [F#](/fsharp/2017/Solutions/Day05.fs) | [F#](/fsharp/2018/Solutions/Day05.fs) | [F#](/fsharp/2019/Solutions/Day05.fs) | [C#](/csharp/2020/Solvers/Day05.cs) | [C#](/csharp/2021/Solvers/Day05.cs) | [C#](/csharp/2022/Solvers/Day05.cs) | [C#](/csharp/2023/Solvers/Day05.cs) | |
| 6  | [C#](/csharp/2015/Solvers/Day06.cs) \| [F#](/fsharp/2015/Solutions/Day06.fs) | [C#](/csharp/2016/Solvers/Day06.cs) \| [F#](/fsharp/2016/Solutions/Day06.fs) | [F#](/fsharp/2017/Solutions/Day06.fs) | [F#](/fsharp/2018/Solutions/Day06.fs) | [F#](/fsharp/2019/Solutions/Day06.fs) | [C#](/csharp/2020/Solvers/Day06.cs) | [C#](/csharp/2021/Solvers/Day06.cs) | [C#](/csharp/2022/Solvers/Day06.cs) | [C#](/csharp/2023/Solvers/Day06.cs) | |
| 7  | [C#](/csharp/2015/Solvers/Day07.cs) \| [F#](/fsharp/2015/Solutions/Day07.fs) | [C#](/csharp/2016/Solvers/Day07.cs) \| [F#](/fsharp/2016/Solutions/Day07.fs) | [F#](/fsharp/2017/Solutions/Day07.fs) | [F#](/fsharp/2018/Solutions/Day07.fs) | [F#](/fsharp/2019/Solutions/Day07.fs) | [C#](/csharp/2020/Solvers/Day07.cs) | [C#](/csharp/2021/Solvers/Day07.cs) | [C#](/csharp/2022/Solvers/Day07.cs) | [C#](/csharp/2023/Solvers/Day07.cs) | |
| 8  | [C#](/csharp/2015/Solvers/Day08.cs) \| [F#](/fsharp/2015/Solutions/Day08.fs) | [C#](/csharp/2016/Solvers/Day08.cs) \| [F#](/fsharp/2016/Solutions/Day08.fs) | [F#](/fsharp/2017/Solutions/Day08.fs) | [F#](/fsharp/2018/Solutions/Day08.fs) | [F#](/fsharp/2019/Solutions/Day08.fs) | [C#](/csharp/2020/Solvers/Day08.cs) | [C#](/csharp/2021/Solvers/Day08.cs) | | [C#](/csharp/2023/Solvers/Day08.cs) | |
| 9  | [C#](/csharp/2015/Solvers/Day09.cs) \| [F#](/fsharp/2015/Solutions/Day09.fs) | [C#](/csharp/2016/Solvers/Day09.cs) \| [F#](/fsharp/2016/Solutions/Day09.fs) | [F#](/fsharp/2017/Solutions/Day09.fs) | [F#](/fsharp/2018/Solutions/Day09.fs) | [F#](/fsharp/2019/Solutions/Day09.fs) | [C#](/csharp/2020/Solvers/Day09.cs) | [C#](/csharp/2021/Solvers/Day09.cs) | | [C#](/csharp/2023/Solvers/Day09.cs) | |
| 10 | [C#](/csharp/2015/Solvers/Day10.cs) \| [F#](/fsharp/2015/Solutions/Day10.fs) | [C#](/csharp/2016/Solvers/Day10.cs) \| [F#](/fsharp/2016/Solutions/Day10.fs) | [F#](/fsharp/2017/Solutions/Day10.fs) | [F#](/fsharp/2018/Solutions/Day10.fs) | [F#](/fsharp/2019/Solutions/Day10.fs) | [C#](/csharp/2020/Solvers/Day10.cs) | [C#](/csharp/2021/Solvers/Day10.cs) | | [C#](/csharp/2023/Solvers/Day10.cs) | |
| 11 | [C#](/csharp/2015/Solvers/Day11.cs) \| [F#](/fsharp/2015/Solutions/Day11.fs) | [C#](/csharp/2016/Solvers/Day11.cs) \| [F#](/fsharp/2016/Solutions/Day11.fs) | [F#](/fsharp/2017/Solutions/Day11.fs) | [F#](/fsharp/2018/Solutions/Day11.fs) | [F#](/fsharp/2019/Solutions/Day11.fs) | [C#](/csharp/2020/Solvers/Day11.cs) | [C#](/csharp/2021/Solvers/Day11.cs) | | [C#](/csharp/2023/Solvers/Day11.cs) | |
| 12 | [C#](/csharp/2015/Solvers/Day12.cs) \| [F#](/fsharp/2015/Solutions/Day12.fs) | [C#](/csharp/2016/Solvers/Day12.cs) \| [F#](/fsharp/2016/Solutions/Day12.fs) | [F#](/fsharp/2017/Solutions/Day12.fs) | [F#](/fsharp/2018/Solutions/Day12.fs) | [F#](/fsharp/2019/Solutions/Day12.fs) | [C#](/csharp/2020/Solvers/Day12.cs) | [C#](/csharp/2021/Solvers/Day12.cs) | | [C#](/csharp/2023/Solvers/Day12.cs) | |
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
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4541/23H2/2023Update/SunValley3)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
```

### 2024
| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **--** | **--** | **--** | **--** |

### 2023

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **41.51 μs** | **41.67 μs** | **41.71 μs** | **--** |
| **2** | **2.86 μs** | **2.87 μs** | **2.87 μs** | **--** |
| **3** | **13.06 μs** | **13.38 μs** | **13.66 μs** | **584 B** |
| **4** | **10.25 μs** | **10.26 μs** | **10.27 μs** | **--** |
| **5** | **6.83 μs** | **7.03 μs** | **7.39 μs** | **7.6 KB** |
| **6** | **0.11 μs** | **0.11 μs** | **0.11 μs** | **--** |
| **7** | **76.58 μs** | **76.76 μs** | **77.03 μs** | **16.0 KB** |
| **8** | **76.21 μs** | **76.42 μs** | **76.95 μs** | **22.4 KB** |
| **9** | **20.81 μs** | **20.99 μs** | **21.44 μs** | **--** |
| **10** | **43.80 μs** | **44.01 μs** | **44.25 μs** | **--** |
| **11** | **2.17 μs** | **2.17 μs** | **2.18 μs** | **584 B** |
| **12** | **1,302.61 μs** | **1,308.88 μs** | **1,311.92 μs** | **162.0 KB** |
| **13** | **15.39 μs** | **15.52 μs** | **15.62 μs** | **--** |
| **14** | **1,637.88 μs** | **1,640.87 μs** | **1,642.65 μs** | **11.8 KB** |
| **15** | **64.75 μs** | **64.98 μs** | **65.36 μs** | **65.6 KB** |
| **16** | **262.04 μs** | **266.84 μs** | **268.18 μs** | **851.7 KB** |
| **17** | **3,025.09 μs** | **3,046.43 μs** | **3,066.36 μs** | **411.7 KB** |
| **18** | **4.49 μs** | **4.53 μs** | **4.55 μs** | **--** |
| **19** | **44.56 μs** | **44.66 μs** | **44.92 μs** | **--** |
| **20** | **2.87 μs** | **2.89 μs** | **2.91 μs** | **15.8 KB** |
| **21** | **13.81 μs** | **13.87 μs** | **13.94 μs** | **--** |
| **22** | **56.64 μs** | **56.96 μs** | **57.05 μs** | **17.8 KB** |
| **23** | **231.29 μs** | **232.31 μs** | **233.53 μs** | **23.4 KB** |
| **24** | **223.07 μs** | **223.63 μs** | **225.06 μs** | **13.3 KB** |
| **25** | **211.92 μs** | **212.24 μs** | **213.05 μs** | **282.2 KB** |
| **All** | **7,725.78 μs** | **7,751.16 μs** | **7,810.15 μs** | **1.9 MB** |

2023 benchmarks are a result of measuring my solver on 5 separate inputs in a row.
This means the results should be more representative and less susceptible to branch prediction biases.

### 2022

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **5.27 μs** | **5.32 μs** | **5.92 μs** | **--** |
| **2** | **5.11 μs** | **5.13 μs** | **5.13 μs** | **--** |
| **3** | **4.66 μs** | **4.67 μs** | **4.68 μs** | **--** |
| **4** | **4.14 μs** | **4.15 μs** | **4.16 μs** | **--** |
| **5** | **6.55 μs** | **6.76 μs** | **7.18 μs** | **1.8 KB** |
| **6** | **5.87 μs** | **5.88 μs** | **5.89 μs** | **--** |
| **7** | **3.46 μs** | **3.47 μs** | **3.48 μs** | **1.1 KB** |
| **19** | **87.50 μs** | **88.03 μs** | **88.38 μs** | **177.5 KB** |
| **20** | **5,042.26 μs** | **5,077.92 μs** | **5,133.89 μs** | **105.9 KB** |

### 2021

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **3.20 μs** | **3.22 μs** | **3.24 μs** | **--** |
| **2** | **1.08 μs** | **1.09 μs** | **1.09 μs** | **--** |
| **3** | **6.78 μs** | **6.85 μs** | **6.93 μs** | **--** |
| **4** | **5.42 μs** | **5.45 μs** | **5.49 μs** | **--** |
| **5** | **172.38 μs** | **173.24 μs** | **173.78 μs** | **23.2 KB** |
| **6** | **0.32 μs** | **0.32 μs** | **0.32 μs** | **--** |
| **7** | **9.81 μs** | **9.87 μs** | **9.89 μs** | **8.4 KB** |
| **8** | **8.56 μs** | **8.59 μs** | **8.63 μs** | **--** |
| **9** | **18.08 μs** | **18.19 μs** | **18.71 μs** | **--** |
| **10** | **17.85 μs** | **17.89 μs** | **17.96 μs** | **--** |
| **11** | **17.02 μs** | **17.18 μs** | **17.22 μs** | **--** |
| **12** | **18.82 μs** | **18.86 μs** | **18.96 μs** | **50.8 KB** |
| **13** | **9.02 μs** | **9.06 μs** | **9.10 μs** | **27.7 KB** |
| **14** | **5.81 μs** | **5.82 μs** | **5.85 μs** | **--** |
| **15** | **3,458.43 μs** | **3,462.70 μs** | **3,526.60 μs** | **4 B** |
| **16** | **5.71 μs** | **5.72 μs** | **5.73 μs** | **--** |
| **17** | **1.21 μs** | **1.21 μs** | **1.22 μs** | **--** |
| **18** | **1,209.20 μs** | **1,217.59 μs** | **1,227.15 μs** | **3 B** |
| **19** | **2,241.85 μs** | **2,255.76 μs** | **2,261.11 μs** | **78.7 KB** |
| **20** | **2,030.63 μs** | **2,033.61 μs** | **2,038.71 μs** | **4 B** |
| **21** | **493.72 μs** | **496.33 μs** | **498.20 μs** | **1 B** |
| **22** | **1,145.53 μs** | **1,147.65 μs** | **1,148.82 μs** | **702.3 KB** |
| **23** | **1,126.61 μs** | **1,136.15 μs** | **1,145.00 μs** | **599.5 KB** |
| **24** | **0.06 μs** | **0.06 μs** | **0.06 μs** | **--** |
| **25** | **362.46 μs** | **363.19 μs** | **364.88 μs** | **1 B** |

### 2020

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **2.12 μs** | **2.13 μs** | **2.15 μs** | **4.1 KB** |
| **2** | **11.24 μs** | **11.26 μs** | **11.30 μs** | **--** |
| **3** | **1.19 μs** | **1.20 μs** | **1.20 μs** | **--** |
| **4** | **18.38 μs** | **18.57 μs** | **18.67 μs** | **--** |
| **5** | **4.88 μs** | **4.89 μs** | **4.90 μs** | **152 B** |
| **6** | **8.67 μs** | **8.75 μs** | **8.82 μs** | **--** |
| **7** | **148.19 μs** | **149.44 μs** | **150.95 μs** | **363.1 KB** |
| **8** | **2.15 μs** | **2.16 μs** | **2.17 μs** | **9.9 KB** |
| **9** | **16.45 μs** | **16.59 μs** | **16.69 μs** | **8.0 KB** |
| **10** | **0.62 μs** | **0.64 μs** | **0.64 μs** | **456 B** |
| **11** | **4,190.95 μs** | **4,218.85 μs** | **4,335.08 μs** | **878.6 KB** |
| **12** | **1.71 μs** | **1.73 μs** | **1.73 μs** | **--** |
| **13** | **1.06 μs** | **1.06 μs** | **1.07 μs** | **1.3 KB** |
| **14** | **2,950.63 μs** | **3,041.10 μs** | **3,159.88 μs** | **8.5 MB** |
| **15** | **183,638.00 μs** | **185,789.67 μs** | **187,861.67 μs** | **120.0 MB** |
| **16** | **22.78 μs** | **22.83 μs** | **22.86 μs** | **5.6 KB** |
| **17** | **109.21 μs** | **110.86 μs** | **112.76 μs** | **527.5 KB** |
| **18** | **46.06 μs** | **46.64 μs** | **47.22 μs** | **1.6 KB** |
| **19** | **335.94 μs** | **339.10 μs** | **342.21 μs** | **15.1 KB** |
| **20** | **467.20 μs** | **469.17 μs** | **472.20 μs** | **415.2 KB** |
| **21** | **132.54 μs** | **134.11 μs** | **134.87 μs** | **142.4 KB** |
| **22** | **10,971.06 μs** | **11,005.21 μs** | **11,082.06 μs** | **10.6 MB** |
| **23** | **145,303.90 μs** | **145,823.89 μs** | **146,152.25 μs** | **4.0 MB** |
| **24** | **2,791.56 μs** | **2,795.34 μs** | **2,806.31 μs** | **65.6 KB** |
| **25** | **1,012.28 μs** | **1,013.80 μs** | **1,018.53 μs** | **3 B** |

### 2019

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **0.84 μs** | **0.84 μs** | **0.84 μs** | **--** |
| **2** | **521.54 μs** | **522.40 μs** | **523.23 μs** | **1.6 KB** |

### 2018

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **32.33 μs** | **33.93 μs** | **35.33 μs** | **47.3 KB** |
| **2** | **19.91 μs** | **20.02 μs** | **20.08 μs** | **2.0 KB** |

### 2017

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **5.05 μs** | **5.08 μs** | **5.21 μs** | **--** |
| **2** | **2.87 μs** | **2.88 μs** | **2.90 μs** | **216 B** |

### 2016

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **5.17 μs** | **5.26 μs** | **5.33 μs** | **27.7 KB** |
| **2** | **6.80 μs** | **6.89 μs** | **7.16 μs** | **--** |
| **3** | **43.05 μs** | **43.45 μs** | **44.90 μs** | **--** |
| **4** | **68.74 μs** | **69.51 μs** | **70.80 μs** | **128 B** |
| **5** | **390,994.10 μs** | **406,054.25 μs** | **455,780.60 μs** | **2.1 KB** |
| **6** | **5.96 μs** | **6.22 μs** | **6.78 μs** | **1.1 KB** |
| **7** | **338.62 μs** | **339.68 μs** | **341.80 μs** | **1.4 KB** |
| **8** | **8.19 μs** | **8.22 μs** | **8.29 μs** | **456 B** |
| **9** | **11.17 μs** | **11.74 μs** | **12.96 μs** | **--** |
| **10** | **5.30 μs** | **5.37 μs** | **5.41 μs** | **30.6 KB** |
| **11** | **16,161.99 μs** | **16,232.72 μs** | **16,381.98 μs** | **11.0 MB** |
| **12** | **34,678.39 μs** | **38,115.56 μs** | **40,740.81 μs** | **585 B** |
| **13** | **28.44 μs** | **28.64 μs** | **28.82 μs** | **70.3 KB** |

### 2015

| Day | P0 | P50 | P100 | Allocations |
|--- |---:|---:|---:|---:|
| **1** | **4.78 μs** | **4.80 μs** | **4.81 μs** | **--** |
| **2** | **6.47 μs** | **6.63 μs** | **6.76 μs** | **--** |
| **3** | **145.06 μs** | **148.34 μs** | **151.03 μs** | **123.5 KB** |
| **4** | **144,905.23 μs** | **146,116.45 μs** | **149,333.83 μs** | **810 B** |
| **5** | **119.21 μs** | **120.04 μs** | **120.45 μs** | **--** |
| **6** | **21,292.17 μs** | **21,535.64 μs** | **21,618.04 μs** | **4.0 MB** |
| **7** | **51.66 μs** | **52.24 μs** | **53.06 μs** | **106.9 KB** |
| **8** | **8.07 μs** | **8.11 μs** | **8.15 μs** | **--** |
| **9** | **411.94 μs** | **414.51 μs** | **415.83 μs** | **9.3 KB** |
| **10** | **30,458.77 μs** | **30,519.44 μs** | **30,744.45 μs** | **46.1 MB** |
| **11** | **419.18 μs** | **420.82 μs** | **422.16 μs** | **1 B** |
| **12** | **93.10 μs** | **94.54 μs** | **95.70 μs** | **--** |
| **13** | **5,341.66 μs** | **5,449.46 μs** | **5,482.60 μs** | **11.1 KB** |
| **14** | **37.10 μs** | **37.42 μs** | **38.20 μs** | **1.1 KB** |
| **15** | **1,374.33 μs** | **1,377.84 μs** | **1,385.00 μs** | **346 B** |
| **16** | **37.28 μs** | **37.87 μs** | **38.16 μs** | **58.0 KB** |
| **17** | **19.06 μs** | **19.34 μs** | **19.53 μs** | **74.2 KB** |
| **18** | **5,495.62 μs** | **5,529.42 μs** | **5,538.48 μs** | **83.4 KB** |
| **19** | **11.41 μs** | **11.52 μs** | **11.59 μs** | **24.6 KB** |
| **20** | **1,608.70 μs** | **1,611.82 μs** | **1,618.29 μs** | **181.7 KB** |
| **21** | **3.95 μs** | **3.99 μs** | **4.03 μs** | **21.1 KB** |
| **22** | **1,878.38 μs** | **1,881.95 μs** | **1,893.31 μs** | **3.0 MB** |
| **23** | **4.62 μs** | **4.88 μs** | **5.05 μs** | **3.2 KB** |
| **24** | **2,041.33 μs** | **2,054.24 μs** | **2,093.17 μs** | **16.6 KB** |
| **25** | **0.17 μs** | **0.17 μs** | **0.17 μs** | **32 B** |
