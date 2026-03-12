using EarClipperLib;
using Svg;
using SVG2Poly.SWGPath;
using SVG2Poly.SWGPathArranger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Helpers
{
	internal class SWGShapeParser
	{

		public static List<SWGPolygon.ISwgPolygon> ParsePolygons(SvgDocument svgdoc, int outerShellIndex = 0, bool checkOrientation = true, bool checkends = true)
		{

			//В копиляторе задаём дискретность кривых (Сколько будет промежуточных точек)
			var comp = new SwgPathCompiler(1);

			var result = new List<SWGPolygon.ISwgPolygon>();

			int Pathsiterator = 0;

			//В файле модет быть нескольько объектов внутри
			foreach (var data in svgdoc.Children)
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
								if (iterator++ == outerShellIndex)
								{
									if (checkOrientation) { if (pi.IsClockwise) pi.Reverse(); }
									Externals.Add(pi);

								}
								//Остальные - дыры - по часовой стрелке
								else
								{
									if (checkOrientation) { if (!pi.IsClockwise) pi.Reverse(); }
									Holes.Add(pi);
								}





							}
						}

						if (Externals.Count != 1) Console.WriteLine("Не одна внешняя");

						foreach (var item in Externals)
						{
							if (item is SwgPathBase swgitem)
							{
								if(checkends) swgitem.RemoveSameEnd();
								polygon = new SWGPolygon.SWGPolygon(swgitem);
							}




						}

						

						//для триангуляции не должно быть одинаковых точек на концах входных интервалов
						foreach (var item in Holes)
						{
							if (item is SwgPathBase swgitem)
							{
								if (checkends) swgitem.RemoveSameEnd();
								polygon.AddHole(swgitem);
							}


						}

						result.Add(polygon);

					}




				}
			}



				return result;
		}

		

	}
}
