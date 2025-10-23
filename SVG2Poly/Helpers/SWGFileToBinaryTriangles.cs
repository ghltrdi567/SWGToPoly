using EarClipperLib;
using Svg;
using Svg.Pathing;
using SVG2Poly.SWGPath;
using SVG2Poly.SWGPathArranger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Helpers
{
	public class SWGFileToBinaryTriangles
	{

		public static void SvgFileToBinaryTriangles(string FullPathToInputSWG, string PathToOutputBinaryDirectory)
		{

			//В копиляторе задаём дискретность кривых
			var comp = new SwgPathCompiler(1);

			//программа не знает. какая часть пути будет внешней границей полигона  - получаем из файла

			int ExternalsIndex = 0;

			string EnternalFilePath = FullPathToInputSWG + ".external.txt";

			if (File.Exists(EnternalFilePath))
			{
				var strings =  File.ReadLines(EnternalFilePath);

				if (strings.Count() > 0)
				{

					int yui = -1;

					if (int.TryParse(strings.FirstOrDefault("-1"), out yui)){

						if(yui>=0) ExternalsIndex = yui;


					}


				}

			}
			



			var svgDoc = SvgDocument.Open<SvgDocument>(FullPathToInputSWG, new SvgOptions());



			int Pathsiterator = 0;

			List<Vector3m> AllTrianles = new List<Vector3m>();

			//В файле модет быть нескольько объектов внутри
			foreach (var data in svgDoc.Children)
			{

				//По путям
				foreach (var element in data.Children)
				{
					//Один путь - один полигон с внешней облочкой и дырами
					if (element is Svg.SvgPath path)
					{


						var elements = path.PathData;


						var paths = comp.CompilePaths(elements);


						List<ISwgPath> Externals = new List<ISwgPath>();

						List<ISwgPath> Holes = new List<ISwgPath>();




						int iterator = 0;
						foreach (var pathu in paths)
						{
							if (pathu is SwgPathBase pi)
							{


								string str = pi.ToJTSLineString();

								if (!pi.IsClosed) Console.WriteLine("Не замкнута!");

								//добавляем внешнюю - нам надо потив часовой стрелке
								if (iterator++ == ExternalsIndex)
								{
									if (pi.IsClockwise) pi.Reverse();
									Externals.Add(pi);

								}
								//Остальные - дыры - по часовой стрелке
								else
								{
									if (!pi.IsClockwise) pi.Reverse();
									Holes.Add(pi);
								}





							}
						}

						if (Externals.Count != 1) Console.WriteLine("Не одна внешняя");


						List<List<Vector3m>> holes = new List<List<Vector3m>>();

						//для триангуляции не должно быть одинаковых точек на концах входных интервалов
						foreach (var item in Holes)
						{
							//Удаляем из точек одинаковое с концом начало
							(item as SwgPathBase)?.RemoveSameEnd();

							holes.Add(PathHelpers.ToMVectors(item));
						}

						foreach (var item in Externals)
						{
							(item as SwgPathBase)?.RemoveSameEnd();

							
						}

						EarClipping earClipping = new EarClipping();
						earClipping.SetPoints(PathHelpers.ToMVectors(Externals[0]), holes);
						earClipping.Triangulate();
						var res = earClipping.Result;

						var result = PathHelpers.TrianglesDataToJTSPolygons(res);

						Console.WriteLine("Путь "+ Pathsiterator.ToString());


						Console.WriteLine(result + Environment.NewLine);

						AllTrianles.AddRange(res);

					}




				}






			}



			if(Pathsiterator > 1)
			{

				Console.WriteLine("Полный путь :");

				Console.WriteLine(PathHelpers.TrianglesDataToJTSPolygons(AllTrianles) + Environment.NewLine);





			}



			string filename =  Path.GetFileNameWithoutExtension(FullPathToInputSWG) ?? "[Unknown]";

			string outputFile = Path.Combine(PathToOutputBinaryDirectory, filename + ".triangles.bin");


			using (var stream = File.Open(outputFile, FileMode.Create))
			{
				using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
				{
					//Порядок серрилизации и десеррилизации должен быть одинаковым
					PathHelpers.WriteTrianglesDataToBinary(AllTrianles, writer);
				}
			}






		}





	}
}
