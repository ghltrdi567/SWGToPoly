using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Curves
{
	/// <summary>
	/// Вычислитель параметрической кривой
	/// </summary>
	internal interface IParametricCurveComputer
	{
		/// <summary>
		/// ВЫчисляет точку на кривой 
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Vector2 ComputePoint(float t);
	}


	internal interface IParametricCurveBase : IParametricCurveComputer
	{
		public Vector2 Start {  get; }

		public Vector2 End {  get; }

	}

	internal interface IControlledCurveBase : IParametricCurveBase
	{

		public Vector2 LastControlPoint { get; }



	}

}
