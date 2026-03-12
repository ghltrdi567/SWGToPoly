using SVG2Poly.SWGPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGPolygon
{
	internal class SWGPolygon : ISwgPolygon
	{
		public ISwgPath ExternalContour => _external;

		public IEnumerable<ISwgPath> InternalHoles => _holes;


		protected SWGPath.SwgPathBase _external;

		protected List<SWGPath.SwgPathBase> _holes;


		public SWGPolygon()
		{
			_external = new SwgPathBase();

			_holes = new List<SwgPathBase>(0);
		}

		public SWGPolygon(SwgPathBase external)
		{
			_external = external;

			_holes = new List<SwgPathBase>(0);
		}

		public void AddHole(ISwgPath hole)
		{
			if (hole is SWGPath.SwgPathBase swghole)
			{
				_holes.Add(swghole);
			}
			else throw new ArgumentException("Неверный формат пути для добавления");
		}
	}
}
