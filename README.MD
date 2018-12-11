# Advent of Code

These are my solutions in F# for Advent of Code 2017 and 2018. If you were wanting to try it out, you should be able to open the .sln file in Visual Studio 2017 with Visual F# tools installed and it should all just work.

## Performance

All solutions (except 2017 Day 22) are completely pure and do not do any mutations.

Below is the results of timing one run for each day/part:

    Year 2017 Day 01-1   23.93ms 1047
    Year 2017 Day 01-2   17.09ms 982
    Year 2017 Day 02-1    4.71ms 53460
    Year 2017 Day 02-2   18.70ms 282
    Year 2017 Day 03-1    1.27ms 326
    Year 2017 Day 03-2   17.87ms 363010
    Year 2017 Day 04-1   13.28ms 337
    Year 2017 Day 04-2   18.04ms 231
    Year 2017 Day 05-1  492.44ms 387096
    Year 2017 Day 05-2  944.34ms 28040648
    Year 2017 Day 06-1   64.62ms 14029
    Year 2017 Day 06-2   58.85ms 2765
    Year 2017 Day 07-1   10.68ms vvsvez
    Year 2017 Day 07-2   25.11ms 362
    Year 2017 Day 08-1   16.87ms 5946
    Year 2017 Day 08-2    4.51ms 6026
    Year 2017 Day 09-1    2.18ms 11846
    Year 2017 Day 09-2    0.43ms 6285
    Year 2017 Day 10-1    5.10ms 38415
    Year 2017 Day 10-2    8.62ms 9de8846431eef262be78f590e39a4848
    Year 2017 Day 11-1    3.21ms 715
    Year 2017 Day 11-2    1.06ms 1512
    Year 2017 Day 12-1    7.37ms 288
    Year 2017 Day 12-2   18.10ms 211
    Year 2017 Day 13-1    1.84ms 1840
    Year 2017 Day 13-2   55.27ms 3850260
    Year 2017 Day 14-1  141.52ms 8304
    Year 2017 Day 14-2  193.18ms 1018
    Year 2017 Day 15-1  790.52ms 600
    Year 2017 Day 15-2  647.31ms 313
    Year 2017 Day 16-1   10.00ms ceijbfoamgkdnlph
    Year 2017 Day 16-2  123.56ms pnhajoekigcbflmd
    Year 2017 Day 17-1    2.91ms 1561
    Year 2017 Day 17-2  375.85ms 33454823
    Year 2017 Day 18-1   12.43ms 7071
    Year 2017 Day 18-2   67.55ms 8001
    Year 2017 Day 19-1    3.67ms GSXDIPWTU
    Year 2017 Day 19-2    2.03ms 16100
    Year 2017 Day 20-1   25.91ms 300
    Year 2017 Day 20-2   59.71ms 502
    Year 2017 Day 21-1    8.24ms 142
    Year 2017 Day 21-2  988.49ms 1879071
    Year 2017 Day 22-1    2.05ms 5182
    Year 2017 Day 22-2  473.90ms 2512008
    Year 2017 Day 23-1    0.07ms 9409
    Year 2017 Day 23-2    2.07ms 913
    Year 2017 Day 24-1 1095.06ms 2006
    Year 2017 Day 24-2 1118.39ms 1994
    Year 2017 Day 25-1  636.14ms 4287
    Year 2017 Day 25-2    0.20ms Advent of Code Finished!
    Year 2018 Day 01-1    0.46ms 516
    Year 2018 Day 01-2   10.91ms 71892
    Year 2018 Day 02-1   15.72ms 7470
    Year 2018 Day 02-2   17.19ms kqzxdenujwcstybmgvyiofrrd
    Year 2018 Day 03-1   25.23ms 110891
    Year 2018 Day 03-2   31.90ms 297
    Year 2018 Day 04-1   18.80ms 84636
    Year 2018 Day 04-2    4.09ms 91679
    Year 2018 Day 05-1    2.93ms 11310
    Year 2018 Day 05-2   17.34ms 6020
    Year 2018 Day 06-1  425.47ms 4342
    Year 2018 Day 06-2    6.50ms 42966
    Year 2018 Day 07-1   14.13ms BHRTWCYSELPUVZAOIJKGMFQDXN
    Year 2018 Day 07-2    1.39ms 959
    Year 2018 Day 08-1    4.53ms 41849
    Year 2018 Day 08-2    1.29ms 32487
    Year 2018 Day 09-1   27.16ms 423717
    Year 2018 Day 09-2 3007.83ms 3553108197
    Year 2018 Day 10-1   29.55ms <solution omitted since it's visual>
    Year 2018 Day 10-2    2.59ms 10656
    Year 2018 Day 11-1   15.62ms 20,41
    Year 2018 Day 11-2  321.33ms 236,270,11