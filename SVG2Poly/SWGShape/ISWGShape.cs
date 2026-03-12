using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGShape
{
	internal interface ISWGShape
	{
		public IEnumerable<SWGPath.ISwgPath> PolygonBoundaries { get; }

		public IEnumerable<SWGShape.SWGTriangle> InternalTriangles { get; }

	}


	


}
