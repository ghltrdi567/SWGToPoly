using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGPath
{
	internal interface ISwgPath
	{
		/// <summary>
		/// направление (по часовой стрелке или нет)
		/// </summary>
		public bool IsClockwise { get; }

		/// <summary>
		/// Замкнутая ли
		/// </summary>
		public bool IsClosed { get; }

		/// <summary>
		/// Количество точек пути
		/// </summary>
		public int PointsCount { get; }

		/// <summary>
		/// точки
		/// </summary>
		public IEnumerable<Vector2> Points { get; }


		public void AddPoint(Vector2 source);


	}
}
