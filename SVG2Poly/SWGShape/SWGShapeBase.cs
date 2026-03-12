using SVG2Poly.SWGPath;
using SVG2Poly.SWGPolygon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SVG2Poly.SWGShape
{
	internal class SWGShapeBase : ISWGShape
	{
		public IEnumerable<ISwgPath> PolygonBoundaries => _boundaries;

		public IEnumerable<SWGShape.SWGTriangle> InternalTriangles => _internalTriangles;


		protected List<ISwgPath> _boundaries;

		protected List<SWGShape.SWGTriangle> _internalTriangles;

		public SWGShapeBase()
		{
			_boundaries = new List<ISwgPath>();

			_internalTriangles = new List<SWGTriangle>();
		}

		public void AddBoundaries(IEnumerable<ISwgPath> path) => _boundaries.AddRange(path);

		public void AddBoundary(ISwgPath path) => _boundaries.Add(path);


		public void AddBoundaries(IEnumerable<SWGPolygon.ISwgPolygon> polys)
		{
			foreach (ISwgPolygon polygon in polys)
			{
				_boundaries.Add(polygon.ExternalContour);

				foreach(ISwgPath hole in polygon.InternalHoles) _boundaries.Add(hole);


			}

		}

		public void AddTriangle(SWGTriangle tri) => _internalTriangles.Add(tri);

		public void AddTriangles(IEnumerable<SWGTriangle> tries) => _internalTriangles.AddRange(tries);

		public static bool IsSame(SWGShapeBase one, SWGShapeBase two)
		{

			if(one._boundaries.Count != two._boundaries.Count) return false;
			if(one._internalTriangles.Count != two._internalTriangles.Count) return false;


			for (int i = 0; i < one._boundaries.Count; i++)
			{
				if (!SwgPathBase.IsSame(one._boundaries[i], two._boundaries[i])) return false;
			}

			for (int i = 0; i < two._internalTriangles.Count; i++)
			{
				if (!SWGShape.SWGTriangle.IsSame(one._internalTriangles[i], two._internalTriangles[i])) return false;
			}

			return true;



		}
	}
}
