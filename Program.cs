using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructOrClassReturn
{
	class Program
	{
		static void Main(string[] args)
		{
			for (int i = 0; i < 4; i++)
			{
				new speedTester().compare(100);
			}
			
			Console.Read();
		}
	}

	struct heavyStruct
	{
		public long one;
		public long two;
		public long thr;
		public long fou;
		public long fiv;
		public long six;
		public long sev;
		public long eig;
		public long nin;
		public long ten;
	}

	struct heavyClass
	{
		public long one;
		public long two;
		public long thr;
		public long fou;
		public long fiv;
		public long six;
		public long sev;
		public long eig;
		public long nin;
		public long ten;
	}

	class speedTester
	{
		public void compare(int times)
		{
			int x = 10000;
			int y = 100;
			var b = new builder();
			var o = new objClass();

			var a = new double[times];
			var mem = new List<long>(times);

			Stopwatch s = new Stopwatch();

			Console.WriteLine("Working...");

			for (int j = 0; j < 6; j++)
			{
				GC.WaitForFullGCComplete();
				switch (j)
				{
					case 0:
						Console.WriteLine("Structs...");
						for (int i = 0; i < times; i++)
						{
							s.Start();
							o.getStructs(x * 100);
							s.Stop();
							a[i] = s.Elapsed.TotalMilliseconds;
							s.Reset();
							mem.Add(GC.GetTotalMemory(false));
						}
						break;
					case 1:
						Console.WriteLine("Classes...");
						for (int i = 0; i < times; i++)
						{
							s.Start();
							o.getClasses(x * 100);
							s.Stop();
							a[i] = s.Elapsed.TotalMilliseconds;
							s.Reset();
							mem.Add(GC.GetTotalMemory(false));
						}
						break;
					case 2:
						Console.WriteLine("Structs + concat...");
						for (int i = 0; i < times; i++)
						{
							s.Start();
							b.getMultiCallerStructs(x,y);
							s.Stop();
							a[i] = s.Elapsed.TotalMilliseconds;
							s.Reset();
							mem.Add(GC.GetTotalMemory(false));
						}
						break;
					case 3:
						Console.WriteLine("Classes + concat...");
						for (int i = 0; i < times; i++)
						{
							s.Start();
							b.getMultiCallerClasses(x,y);
							s.Stop();
							a[i] = s.Elapsed.TotalMilliseconds;
							s.Reset();
							mem.Add(GC.GetTotalMemory(false));
						}
						break;
					case 4:
						Console.WriteLine("Structs + Recursive + concat...");
						for (int i = 0; i < times; i++)
						{
							s.Start();
							b.getStructRecursive(10, x, y);
							s.Stop();
							a[i] = s.Elapsed.TotalMilliseconds;
							s.Reset();
							mem.Add(GC.GetTotalMemory(false));
						}
						break;
					case 5:
						Console.WriteLine("Classes + Recursive + concat...");
						for (int i = 0; i < times; i++)
						{
							s.Start();
							b.getClassRecursive(10, x, y);
							s.Stop();
							a[i] = s.Elapsed.TotalMilliseconds;
							s.Reset();
							mem.Add(GC.GetTotalMemory(false));
						}
						break;
					default:
						Console.WriteLine("huh?");
						break;
				}


				double max = a.Max();
				Console.WriteLine("Max: {0}", max);

				double min = a.Min();
				Console.WriteLine("Min: {0}", min);

				double mean = a.Average();
				Console.WriteLine("Mean: {0}", mean);

				var deviations = new double[a.Length];
				for (int i = 0; i < a.Length; i++)
				{
					deviations[i] = Math.Pow((a[i] - mean), 2.0);
				}

				double averageVariance = deviations.Average();
				Console.WriteLine("Variance: {0}", averageVariance);

				//The higher this is, the more GC spikes we had
				double standardDeviation = Math.Sqrt(averageVariance);
				Console.WriteLine("Population Standard Deviation: {0}", standardDeviation);

				var adjusted = new List<double>();

				for (int i = 0; i < a.Length; i++)
				{
					if (deviations[i] < averageVariance)
					{
						adjusted.Add(a[i]);
					}
				}

				//This is to remove the spikes from GC
				var adjustedMean = adjusted.Average();
				Console.WriteLine("Adjusted Mean: {0}", adjustedMean);

				var avgMem = mem.Average();
				Console.WriteLine("Average Memory: {0}", avgMem/1000000.0);
				Console.WriteLine();
				mem.Clear();
			}
		}
	}

	class builder
	{
		/// <summary>
		/// each objClass each gets numOfStructs number of structs
		/// </summary>
		public heavyStruct[] getMultiCallerStructs(int numOfStructs, int numOfCallers)
		{
			var o = new objClass();

			var a = new heavyStruct[numOfStructs * numOfCallers];
			int count = 0;
			for (int i = 0; i < numOfCallers; i++)
			{
				var ia = o.getStructs(numOfStructs);
				foreach (var s in ia)
				{
					a[count++] = s;
				}
			}

			return a;
		}

		/// <summary>
		/// each objClass each gets numOfClasses number of structs
		/// </summary>
		public heavyClass[] getMultiCallerClasses(int numOfClasses, int numOfCallers)
		{
			var o = new objClass();

			var a = new heavyClass[numOfClasses * numOfCallers];
			int count = 0;
			for (int i = 0; i < numOfCallers; i++)
			{
				var ia = o.getClasses(numOfClasses);
				foreach (var c in ia)
				{
					a[count++] = c;
				}
			}

			return a;
		}

		public heavyStruct[] getStructRecursive(int times, int numOfStructs, int numOfCallers)
		{
			if (times == 0)
				return getMultiCallerStructs(numOfStructs, numOfCallers);
			return getStructRecursive(times - 1, numOfStructs, numOfCallers);
		}

		public heavyClass[] getClassRecursive(int times, int numOfStructs, int numOfCallers)
		{
			if (times == 0)
				return getMultiCallerClasses(numOfStructs, numOfCallers);
			return getClassRecursive(times - 1, numOfStructs, numOfCallers);
		}

	}

	class objClass
	{
		public heavyStruct[] getStructs(int num)
		{
			var a = new heavyStruct[num];
			for (int i = 0; i < num; i++)
			{
				a[i] = new heavyStruct()
				{
					one = 1,
					two = 2,
					thr = 3,
					fou = 4,
					fiv = 5,
					six = 6,
					sev = 7,
					eig = 8,
					nin = 9,
					ten = 10
				};
			}
			return a;
		}

		public heavyClass[] getClasses(int num)
		{
			var a = new heavyClass[num];
			for (int i = 0; i < num; i++)
			{
				a[i] = new heavyClass()
				{
					one = 1,
					two = 2,
					thr = 3,
					fou = 4,
					fiv = 5,
					six = 6,
					sev = 7,
					eig = 8,
					nin = 9,
					ten = 10
				};
			}
			return a;
		}
	}
}
