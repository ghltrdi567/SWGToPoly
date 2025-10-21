using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Curves
{
	/// <summary>
	/// Квадратичная кривая Безье
	/// </summary>
	internal struct QuadraticCurve : IControlledCurveBase
	{
		public QuadraticCurve(Vector2 start, Vector2 end, Vector2 controlPoint, bool IsPreviousControlPoint = false)
		{

			Start = start;
			End = end;
			
			if (IsPreviousControlPoint)
			{
				ControlPoint = Start + (Start - controlPoint);
			}
			else
			{
				ControlPoint = controlPoint;
			}

		}

		public Vector2 Start { get; init; }

		public Vector2 End { get; init; }

		public Vector2 ControlPoint { get; init; }

		public Vector2 LastControlPoint => ControlPoint;

		public Vector2 ComputePoint(float t)
		{
			t = float.Clamp(t, 0, 1);

			float minT = 1 - t;

			return Start * minT * minT +
				ControlPoint * 2 * t * minT +
				End * t * t;

		}
	}
}
