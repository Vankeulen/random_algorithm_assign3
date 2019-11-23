using System;

namespace random_ass4 {
	class MainClass {

		public static void Main(string[] args) {
			ushort seed = 0;
			Func<int> sm64rng = () => {
				seed = rng(seed);
				return seed;
			};
			RngDistribution(sm64rng, ushort.MaxValue, 10, "Mario: sm64rng");

			Random rand = new Random();
			Func<int> csharpRng = () => {
				return rand.Next(int.MaxValue);
			};
			RngDistribution(csharpRng, int.MaxValue, 10, "C#: csharpRng");

			Console.Read();
		}

		private static void RngDistribution(Func<int> rngFunc, int maxValue, int reps, string rngType) {
			Console.WriteLine(rngType);

			long totalReps = ((long)maxValue) * reps;

			// In order to get around memory limits, we split the full results array 
			// into this many pieces
			int pieces = 4;
			// Each array is the same size
			int size = 1 + (maxValue / pieces);
			// And we create a 2d array to hold our smaller arrays
			byte[][] results = new byte[pieces][];
			for (int i = 0; i < results.Length; i++) {
				results[i] = new byte[size];
			}

			int maxRepeat = 1;
			int percent = 0;
			for (long i = 0; i < totalReps; i++) {
				int next = rngFunc();

				int piece = next / size;
				int part = next % size;

				results[piece][part] += 1;

				int cnt = results[piece][part];
				if (cnt > maxRepeat) {
					// Console.WriteLine($"New record! {next} is {cnt}!");
					maxRepeat = cnt;
				}

				int p = ((int)((i * 100.0) / maxValue)) / reps;
				if ((p > percent) && (p % 10 == 0)) {
					Console.WriteLine(p + "% done...");
					percent = p;

				}
			}
			Console.WriteLine("Done\n\n");

			int[] frequencies = new int[maxRepeat + 1];

			for (int i = 0; i < maxValue; i++) {
				int piece = i / size;
				int part = i % size;

				int occurrences = results[piece][part];

				frequencies[occurrences] += 1;
			}

			for (int i = 0; i < frequencies.Length; i++) {
				Console.WriteLine($"{frequencies[i]} x {i} occurrences");
			}
		}

		// Mario 64 RNG
		static ushort rng(ushort input) {
			if (input == 0x560A) { input = 0; }
			ushort s0 = (ushort)(((byte)input) << 8);
			s0 ^= input;
			input = (ushort)(((s0 & 0xFF) << 8) | ((s0 & 0xFF00) >> 8));
			s0 = (ushort)(((byte)s0) << 1 ^ input);

			ushort s1 = (ushort)((s0 >> 1) ^ 0xFF80);
			if ((s0 & 1) == 0) {
				input = (ushort)(s1 ^ 0x1ff4);
			} else {
				input = (ushort)(s1 ^ 0x8180);
			}
			return input;
		}
	}
}

