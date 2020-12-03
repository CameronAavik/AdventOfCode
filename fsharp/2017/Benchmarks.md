## Performance

Benchmarks are performed using [BenchmarkDotNet](https://benchmarkdotnet.org/) on the following specs:

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i5-7600K CPU 3.80GHz (Kaby Lake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=5.0.100-rc.2.20479.15
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.47505, CoreFX 5.0.20.47505), X64 RyuJIT DEBUG
  Job-OZAOPF : .NET Core 5.0.0 (CoreCLR 5.0.20.47505, CoreFX 5.0.20.47505), X64 RyuJIT

PowerPlanMode=00000000-0000-0000-0000-000000000000  IterationTime=250.0000 ms  MaxIterationCount=15  
MinIterationCount=10  WarmupCount=1  

```
|  Year | Day | Part |     Mean [ns] |    Error [ns] |
|-------- |---- |----- |--------------:|--------------:|
| **2017** |   **1** |    **1** |     **321,436.2** |      **2,532.34** |
| **2017** |   **1** |    **2** |     **717,693.2** |     **13,743.80** |
| **2017** |   **2** |    **1** |     **112,016.7** |      **1,485.23** |
| **2017** |   **2** |    **2** |     **992,133.5** |      **7,122.80** |
| **2017** |   **3** |    **1** |      **86,305.7** |      **1,177.61** |
| **2017** |   **3** |    **2** |     **393,849.5** |      **7,675.08** |
| **2017** |   **4** |    **1** |     **846,531.0** |      **5,098.72** |
| **2017** |   **4** |    **2** |   **1,776,610.8** |     **16,947.64** |
| **2017** |   **5** |    **1** | **373,771,950.0** |  **6,007,262.18** |
| **2017** |   **5** |    **2** | **796,474,440.0** | **14,278,879.26** |
| **2017** |   **6** |    **1** |  **36,331,759.1** |    **722,321.84** |
| **2017** |   **6** |    **2** |  **35,823,344.4** |    **468,863.74** |
| **2017** |   **7** |    **1** |   **2,816,686.0** |     **46,923.52** |
| **2017** |   **7** |    **2** |   **5,087,926.3** |     **88,056.43** |
| **2017** |   **8** |    **1** |   **3,535,305.5** |      **8,727.88** |
| **2017** |   **8** |    **2** |   **3,457,286.8** |     **67,501.21** |
| **2017** |   **9** |    **1** |     **415,216.6** |      **7,893.76** |
| **2017** |   **9** |    **2** |     **421,184.8** |      **4,676.62** |
| **2017** |  **10** |    **1** |      **97,900.0** |      **1,881.87** |
| **2017** |  **10** |    **2** |   **1,693,200.6** |     **31,371.45** |
| **2017** |  **11** |    **1** |     **849,926.5** |      **8,798.71** |
| **2017** |  **11** |    **2** |     **873,736.1** |      **7,382.48** |
| **2017** |  **12** |    **1** |   **2,411,883.9** |     **42,111.49** |
| **2017** |  **12** |    **2** |  **13,168,557.0** |    **325,964.74** |
| **2017** |  **13** |    **1** |      **95,798.0** |        **846.14** |
| **2017** |  **13** |    **2** |  **55,072,055.0** |    **928,318.81** |
| **2017** |  **14** |    **1** |  **95,495,105.0** |  **1,806,987.71** |
| **2017** |  **14** |    **2** | **152,807,954.5** |  **2,835,308.98** |
| **2017** |  **15** |    **1** | **338,758,770.0** |  **4,615,687.04** |
| **2017** |  **15** |    **2** | **317,626,620.0** |  **1,417,816.00** |
| **2017** |  **16** |    **1** |   **4,363,878.8** |     **55,358.53** |
| **2017** |  **16** |    **2** | **101,876,844.4** |  **1,149,353.42** |
| **2017** |  **17** |    **1** |     **127,379.3** |      **1,739.34** |
| **2017** |  **17** |    **2** | **365,099,033.3** |  **3,162,604.07** |
| **2017** |  **18** |    **1** |     **426,853.6** |      **5,854.83** |
| **2017** |  **18** |    **2** |  **45,163,807.5** |    **778,971.29** |
| **2017** |  **19** |    **1** |   **2,007,347.5** |     **21,554.76** |
| **2017** |  **19** |    **2** |   **1,981,695.7** |     **10,462.84** |
| **2017** |  **20** |    **1** |  **14,746,892.2** |    **270,983.41** |
| **2017** |  **20** |    **2** |  **39,604,566.7** |    **537,207.53** |
| **2017** |  **21** |    **1** |   **1,781,018.7** |     **24,955.87** |
| **2017** |  **21** |    **2** | **836,432,333.3** |  **9,950,683.34** |
| **2017** |  **22** |    **1** |     **496,887.9** |      **5,481.98** |
| **2017** |  **22** |    **2** | **360,172,460.0** |  **6,274,556.02** |
| **2017** |  **23** |    **1** |      **79,195.2** |      **1,065.38** |
| **2017** |  **23** |    **2** |     **188,126.1** |      **2,046.88** |
| **2017** |  **24** |    **1** | **971,471,310.0** |  **6,189,228.09** |
| **2017** |  **24** |    **2** | **998,199,650.0** | **11,580,180.94** |
| **2017** |  **25** |    **1** | **608,154,550.0** |  **8,740,075.90** |
| **2017** |  **25** |    **2** |         **252.0** |          **3.71** |