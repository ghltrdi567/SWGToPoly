using Svg.Pathing;
using SVG2Poly.SWGPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGPathArranger
{
	internal interface ISwgPathCompiler
	{

		IEnumerable<ISwgPath> CompilePaths(SvgPathSegmentList segments);


	}
}
