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

			//В копиляторе задаём дискретность кривых (Сколько будет промежуточных точек)
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


			List<SWGPolygon.SWGPolygon> AllPolygons = new List<SWGPolygon.SWGPolygon>();
			//В файле модет быть нескольько объектов внутри
			foreach (var data in svgDoc.Children)
			{

				//По путям
				foreach (var element in data.Children)
				{

					var polygon = new SWGPolygon.SWGPolygon();



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

						foreach (var item in Externals)
						{
							if(item is SwgPathBase swgitem)
							{
								swgitem.RemoveSameEnd();
								polygon = new SWGPolygon.SWGPolygon(swgitem);
							}


							

						}

						List<List<Vector3m>> holes = new List<List<Vector3m>>();

						//для триангуляции не должно быть одинаковых точек на концах входных интервалов
						foreach (var item in Holes)
						{
							if (item is SwgPathBase swgitem)
							{
								swgitem.RemoveSameEnd();
								holes.Add(PathHelpers.ToMVectors(item));
								polygon.AddHole(swgitem);
							}

							
						}

						AllPolygons.Add(polygon);

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

		public static void SvgFileToBinaryShapes(string FullPathToInputSWG, string PathToOutputBinaryDirectory)
		{
			//В копиляторе задаём дискретность кривых (Сколько будет промежуточных точек)
			var comp = new SwgPathCompiler(1);

			//программа не знает. какая часть пути будет внешней границей полигона  - получаем из файла

			int? ExternalsIndex = null;

			string EnternalFilePath = FullPathToInputSWG + ".external.txt";

			if (File.Exists(EnternalFilePath))
			{
				var strings = File.ReadLines(EnternalFilePath);

				if (strings.Count() > 0)
				{

					int yui = -1;

					if (int.TryParse(strings.FirstOrDefault("-1"), out yui))
					{

						if (yui >= 0) ExternalsIndex = yui;


					}


				}

			}

			

			//парсим полигоны из Svg пути
		 	List<SWGPolygon.ISwgPolygon> parsedPOlygons = Helpers.SWGShapeParser.ParsePolygons(SvgDocument.Open<SvgDocument>(FullPathToInputSWG, new SvgOptions()),true, true, ExternalsIndex);


			List<SWGShape.SWGTriangle> Triangles = new List<SWGShape.SWGTriangle>();

			foreach(var polygon in parsedPOlygons)
			{

				if (polygon == null) continue;
				//каждый полигон триангулируем (получаем треугольники, которые внутри полигона)
				Triangles.AddRange(Helpers.SWGPolygonTriangulator.Triangulate(polygon, false, false));
			}

			//Все данные (треугольники и ограничивающие пути) записываем в один объект для сохранения
			SWGShape.SWGShapeBase shape = new SWGShape.SWGShapeBase();

			shape.AddBoundaries(parsedPOlygons);
			shape.AddTriangles(Triangles);



			string filename = Path.GetFileNameWithoutExtension(FullPathToInputSWG) ?? "[Unknown]";

			string outputFile = Path.Combine(PathToOutputBinaryDirectory, filename + ".shapes.bin");

			Transporter.SWGShapeBinaryTransporter tr = new Transporter.SWGShapeBinaryTransporter();

			//тест
			if (true)
			{

				var testfilename = Path.GetTempFileName();


				using (var stream = File.Open(testfilename, FileMode.Create))
				{
					using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
					{

						tr.Write(writer, shape);

					}

					stream.Position = 0;
					using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
					{


						var done = tr.Read(reader);

						if (!SWGShape.SWGShapeBase.IsSame(shape, done)) throw new Exception("Ошибка в записи/чтении");

					}



				}




				File.Delete(testfilename);






			}





			using (var stream = File.Open(outputFile, FileMode.Create))
			{
				using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
				{
					
					tr.Write(writer, shape);

				}
			}




		}



	}
}
