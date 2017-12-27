# Advent of Code 2017

These are my solutions in F#. If you were wanting to try it out, you should be able to open the .sln file in Visual Studio 2017 with Visual F# tools installed and it should all just work.

If you were wanting to look at the code, all solutions are in the one file: [Program.fs](Program.fs)

## Performance

Given that I am new to F#, I was interested in seeing if I could achieve C#-like performance while still following functional programming practices.

All solutions (except Day 22) are completely pure and do not do any mutations.

Below is the results of timing one run for each day/part:

	Day 01-1   15.62ms 1047
	Day 01-2   11.47ms 982
	Day 02-1    4.06ms 53460
	Day 02-2   34.57ms 282
	Day 03-1    1.35ms 326
	Day 03-2   17.33ms 363010
	Day 04-1   26.10ms 337
	Day 04-2    7.23ms 231
	Day 05-1  538.28ms 387096
	Day 05-2  924.74ms 28040648
	Day 06-1   62.63ms 14029
	Day 06-2   60.23ms 2765
	Day 07-1   10.24ms vvsvez
	Day 07-2   24.47ms 362
	Day 08-1   15.77ms 5946
	Day 08-2    5.11ms 6026
	Day 09-1    1.86ms 11846
	Day 09-2    0.39ms 6285
	Day 10-1    5.07ms 38415
	Day 10-2    7.41ms 9de8846431eef262be78f590e39a4848
	Day 11-1    2.80ms 715
	Day 11-2    0.81ms 1512
	Day 12-1    5.62ms 288
	Day 12-2   19.99ms 211
	Day 13-1    1.73ms 1840
	Day 13-2   57.88ms 3850260
	Day 14-1  128.49ms 8304
	Day 14-2  194.74ms 1018
	Day 15-1  793.65ms 600
	Day 15-2  657.63ms 313
	Day 16-1    9.35ms ceijbfoamgkdnlph
	Day 16-2  103.03ms pnhajoekigcbflmd
	Day 17-1    3.13ms 1561
	Day 17-2  370.26ms 33454823
	Day 18-1   11.28ms 7071
	Day 18-2   67.63ms 8001
	Day 19-1    3.38ms GSXDIPWTU
	Day 19-2    2.11ms 16100
	Day 20-1   20.92ms 300
	Day 20-2   56.86ms 502
	Day 21-1    7.00ms 142
	Day 21-2  965.88ms 1879071
	Day 22-1    1.96ms 5182
	Day 22-2  485.85ms 2512008
	Day 23-1    0.08ms 9409
	Day 23-2    2.40ms 913
	Day 24-1 1089.72ms 2006
	Day 24-2 1090.36ms 1994
	Day 25-1  657.14ms 4287
	Day 25-2    0.13ms Advent of Code Finished!