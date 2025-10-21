using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Curves
{
	/// <summary>
	/// Кубическая кривая Безье
	/// </summary>
	internal struct CubicCurve : IControlledCurveBase
	{
		public CubicCurve(Vector2 start, Vector2 end, Vector2 control1, Vector2 control2)
		{
			Start = start;
			End = end;
			Control1 = control1;
			Control2 = control2;
		}

		public Vector2 LastControlPoint => Control2;

		public Vector2 Start { get; init; }

		public Vector2 End { get; init; }

		public Vector2 Control1 { get; init; }

		public Vector2 Control2 { get; init; }

		public Vector2 ComputePoint(float t)
		{
			t = float.Clamp(t, 0, 1);

			float minT = 1 - t;

			return Start * minT * minT * minT +
				Control1 * 3 * t * minT * minT +
				Control2 * 3 * t * t * minT +
				End * t * t * t;
		}
	}
}
