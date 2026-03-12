using EarClipperLib;
using SVG2Poly.SWGPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Helpers
{
	internal class SWGPolygonTriangulator
	{
		/// <summary>
		/// переводит полигоны в набор треуголььников
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="checkOrientation">проверяет (и исправляет)то, что внешняя граница доллжна быть против часовой стрелке. а дыры по часовой стрелке</param>
		/// <param name="checkends">проверяет (и исправляет)то, что не должны началььные и конечные точки бытьт одинаковыми</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static List<SWGShape.SWGTriangle> Triangulate(SWGPolygon.ISwgPolygon polygon, bool checkOrientation = true, bool checkends = true)
		{

			//точки на концах не должныюыть одинаковыми
			if(checkends) (polygon.ExternalContour as SwgPathBase)?.RemoveSameEnd();

			if(checkOrientation) { if (polygon.ExternalContour.IsClockwise) (polygon.ExternalContour as SwgPathBase)?.Reverse(); }

			List<List<Vector3m>> holes = new List<List<Vector3m>>();

			foreach(ISwgPath hole in polygon.InternalHoles)
			{
				if (checkends) (hole as SwgPathBase)?.RemoveSameEnd();
				if (checkOrientation) { if (!hole.IsClockwise) (hole as SwgPathBase)?.Reverse(); }

				holes.Add(PathHelpers.ToMVectors(hole));

			}

			EarClipping earClipping = new EarClipping();
			earClipping.SetPoints(PathHelpers.ToMVectors(polygon.ExternalContour), holes);
			earClipping.Triangulate();
			List<Vector3m> res = earClipping.Result;

			if (res.Count % 3 != 0) throw new Exception("Исходное не делится на 3");

			List<SWGShape.SWGTriangle> result = new List<SWGShape.SWGTriangle>(res.Count/3);

			int counter = 0;
			for (int i = 0; i < res.Count/3; i++)
			{
				result.Add(new SWGShape.SWGTriangle(
					new System.Numerics.Vector2(res[counter].X.ToSingle(), res[counter++].Y.ToSingle()),
					new System.Numerics.Vector2(res[counter].X.ToSingle(), res[counter++].Y.ToSingle()),
					new System.Numerics.Vector2(res[counter].X.ToSingle(), res[counter++].Y.ToSingle())
					));
			}


			return result;
		}

		

	}
}
