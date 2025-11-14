using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Helpers
{
	internal class MemoryTests
	{
		public static object testobj = null;



		public static void Test()
		{

			bool modernize = false;

			Random random = new Random();

			int CAPACITY = 10000000;




			if (modernize)
			{
				g3.DVectorArray3f vect = new g3.DVectorArray3f(CAPACITY);

				vect.Append(random.NextSingle(), random.NextSingle(), random.NextSingle());


				testobj = vect;



			}
			else
			{

				List<Vector3> res = new List<Vector3>(CAPACITY);

				for (int i = 0; i < CAPACITY; i++)
				{
					res.Add(new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle()));
				}


				testobj = res;


			}

			


			GCInfoReporter.ReportCurrentGCInfoToLog();




		}



	}
}
