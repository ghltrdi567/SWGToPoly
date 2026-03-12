using SVG2Poly.SWGPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGPolygon
{
	/// <summary>
	/// Совокупность внешнего контура и внутренних отверстий
	/// </summary>
	internal interface ISwgPolygon
	{
		public ISwgPath ExternalContour { get; }

		public IEnumerable<ISwgPath> InternalHoles { get; }

		public void AddHole(ISwgPath hole);


	}
}
