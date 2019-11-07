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

			long lseed = DateTime.Now.Ticks;
			Func<int> srng = () => {
				lseed = SRNG.hash(lseed);
				int val = (int)lseed;
				if (val == int.MinValue) { return 0; }
				return (val < 0) ? -1 * val : val;
			};
			RngDistribution(srng, int.MaxValue, 10, "srng");


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

	/// <summary> Crude, Simple RNG </summary>
	/// <remarks> 
	///        <para> Provides both static and instance methods for random number generation. </para>
	///        <para> Static methods can be used to generate random numbers from given information. </para>
	///        <para> An instance of this class represents a stateful sequence of numbers. </para>
	///        <para> Instances can be constructed with a given seed, so they generate the same sequence of numbers. </para>
	///        <para> Otherwise, if not given a seed, they will use the current <see cref="System.DateTime.Now.Ticks"/> as the seed. </para>
	/// </remarks>
	public class SRNG {

		/// <summary> BAAAARF </summary>
		private const long c1 = 0xFFFF8000;
		/// <summary> BRRAAAAAP </summary>
		private const long c2 = 0x1F2FF3F4;//0x8BADF00D;
										   /// <summary> PFFFFFFFT </summary>
		private const long c3 = 0x83828180;//0x501D1F1D;

		/// <summary> Hashes a <see cref="System.DateTime"/> by its Ticks </summary>
		/// <param name="seed"> <see cref="System.DateTime"/> to hash </param>
		/// <returns> Hash of <paramref name="seed"/> seed's Ticks </returns>
		public static long hash(DateTime seed) { return hash(seed.Ticks); }

		/// <summary> Gets the hash of a given <paramref name="seed"/>. </summary>
		/// <param name="seed"> Value to use as seed </param>
		/// <returns> Mostly randomly distributed value based on the input hash </returns>
		public static long hash(long seed) {
			long a1 = seed << 32;
			long s0 = seed ^ a1;

			long left = (int)s0;
			long right = (int)(s0 >> 32);
			long join = (left << 32) | right;

			s0 = join ^ (s0 << 1);
			long s1 = c1 ^ (s0 >> 1);

			bool w = ((byte)s0) % 2 == 0;
			long value = w ? c2 : c3;
			value = s1 ^ value;

			return value;
		}
	}
}

