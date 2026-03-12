using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGShape
{
	internal struct SWGTriangle
	{

		public Vector2 P1, P2, P3;

		public SWGTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			P1 = p1;
			P2 = p2;
			P3 = p3;
		}


		public static bool IsSame(SWGTriangle one, SWGTriangle two) => one.P1 == two.P1 && one.P2 == two.P2 && one.P3 == two.P3;
	}
}
