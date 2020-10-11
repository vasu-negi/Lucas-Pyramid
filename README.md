# Lucas-Pyramid

In order to run app on *nix based/MAC system, follow the instructions below

•   Go to project directory in terminal
•   Type the command in following way: dotnet fsi --langversion:preview proj1.fsx N k
•    This program finds all consecutive numbers starting from 1 to N

1.  The size of the work unit is determined using the number of cores present in a system. This could be proven both intuitively and mathematically.
By intuition, the best possible way to divide your work among n workers is work / n, this makes sure that all the cores are working equally and running at 100% efficiency.

In order to prove, this intuition, we ran this test for N = 1000000 and k = 24, on different no of worker and following were results, the ratio peaks around when no of worker = number of cores

Worker  Real    CPU Ratio
2    9.771   25.205  2.5795722
4    7.51    28.329  3.77217044
8    7.17    40.21   5.60808926
20   7.507   34.5    4.59571067
50   6.76    33.26   4.92011834
100  7.11    33.84   4.75949367
500  9.3     39.4    4.23655914
1000 10.57   38.845  3.67502365



2.  Following is the output result for N = 1000000 & k = 4 Result for running the program: dotnet fsi --langversion:preview proj1.fsx 1000000 4

No number was printed, that means there are 4 consecutive number who sum of square is a perfect square.

3.  The Ratio of CPU is 4.5 (Real: 00:00:05.250 CPU: 00:00:23.763)

4.  The largest problem we managed to solve was for N = 10^8 and k = 24

(base) vasus-MacBook-Pro:Lucas-Pyramid vasunegi$  dotnet fsi --langversion:preview lucas-pyramid.fsx 100000000 24
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
1
9
20
25
44
76
121
197
304
353
540
856
1301
2053
3112
3597
5448
8576
12981
20425
30908
35709
54032
84996
128601
12602701
202289
306060
353585
534964
841476
1273121
19823373
82457176
2002557
8329856
52422128
3029784
3500233
34648837
5295700
29991872
Real: 00:08:34.246, CPU: 01:20:45.102, GC gen0: 268826, gen1: 1499, gen2: 6
(base) vasus-MacBook-Pro:Lucas-Pyramid vasunegi$

Output format:

After running the program with the given command, all the start indices are printed on each line.

Note: List of numbers may or may not be in sorted order.

