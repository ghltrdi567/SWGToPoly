using Svg;
using Svg.Pathing;
using SVG2Poly.SWGPath;
using SVG2Poly.SWGPathArranger;



string path = "M339 0h-63v622l-130 -83l-32 47l175 114h50v-700z";


var pty = new SvgPathBuilder().ConvertFromString(path);


var comp = new SwgPathCompiler(3);


var paths = comp.CompilePaths(pty as SvgPathSegmentList);



foreach(var pathu in paths)
{
	if(pathu is SwgPathBase pi)
	{


		string str = pi.ToJTSLineString();

		var ui = pi.IsClosed;

		var u = pi.IsClockwise;

	}
}
