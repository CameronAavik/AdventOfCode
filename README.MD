# Advent of Code 2017

These are my solutions in F#. If you were wanting to try it out, you should be able to open the .sln file in Visual Studio 2017 with Visual F# tools installed and it should all just work.

## Performance

Given that I am new to F#, I was interested in seeing if I could achieve C#-like performance while still following functional programming practices.

All days (except Day 22) are completely pure and do not do any mutations.

Below is the results of timing one run for each day/part so far:

	Day 01-1   13.37ms 1047
	Day 01-2   10.27ms 982
	Day 02-1    3.94ms 53460
	Day 02-2   14.32ms 282
	Day 03-1    1.05ms 326
	Day 03-2   12.22ms 363010
	Day 04-1    7.97ms 337
	Day 04-2    6.89ms 231
	Day 05-1  418.96ms 387096
	Day 05-2  904.09ms 28040648
	Day 06-1   66.89ms 14029
	Day 06-2   71.99ms 2765
	Day 07-1    8.68ms vvsvez
	Day 07-2   50.97ms 362
	Day 08-1   23.69ms 5946
	Day 08-2    4.76ms 6026
	Day 09-1    2.19ms 11846
	Day 09-2    0.46ms 6285
	Day 10-1    5.03ms 38415
	Day 10-2    7.89ms 9de8846431eef262be78f590e39a4848
	Day 11-1    2.82ms 715
	Day 11-2    0.61ms 1512
	Day 12-1    6.38ms 288
	Day 12-2   21.87ms 211
	Day 13-1    2.11ms 1840
	Day 13-2   59.95ms 3850260
	Day 14-1  163.70ms 8304
	Day 14-2  231.79ms 1018
	Day 15-1  811.55ms 600
	Day 15-2  678.48ms 313
	Day 16-1    9.64ms ceijbfoamgkdnlph
	Day 16-2  113.59ms pnhajoekigcbflmd
	Day 17-1    2.87ms 1561
	Day 17-2  393.55ms 33454823
	Day 18-1   12.80ms 7071
	Day 18-2   63.55ms 8001
	Day 19-1    3.40ms GSXDIPWTU
	Day 19-2    2.02ms 16100
	Day 20-1   23.64ms 300
	Day 20-2   62.41ms 502
	Day 21-1    7.28ms 142
	Day 21-2  995.58ms 1879071
	Day 22-1    2.14ms 5182
	Day 22-2  458.54ms 2512008
	Day 23-1    0.06ms 9409
	Day 23-2    2.23ms 913
	Day 24-1 1131.99ms 2006
	Day 24-2 1118.03ms 1994
	Day 25-1  583.21ms 4287
	Day 25-2    0.13ms Advent of Code Finished!