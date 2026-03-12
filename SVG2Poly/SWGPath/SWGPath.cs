using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGPath
{
	/// <summary>
	/// последовательность точек, образующих путь
	/// </summary>
	internal class SwgPathBase : ISwgPath
	{
		private List<Vector2> _points;

		public SwgPathBase()
		{
			_points = new List<Vector2>(0);
		}
		public bool IsClockwise => CheckClockwise();

		public bool IsClosed => CheckClosed();

		public IEnumerable<Vector2> Points => _points;

		public Vector2 LastPoint => _points.Count == 0? Vector2.Zero: _points[_points.Count - 1];

		public int PointsCount => _points.Count;

		public void ClosePath()
		{
			if (!IsClosed) AddPoint(_points[0]);

		}

		public void AddPoint(Vector2 source)
		{

			_points.Add(source);

		}

		public void Reverse() => _points.Reverse();


		protected virtual bool CheckClosed()
		{

			if(_points.Count == 0) throw new InvalidOperationException("У пустой линии невозможно определить замкнута она или нет");

			return _points[0] == _points[_points.Count - 1];

		}

		protected virtual bool CheckClockwise()
		{
			if (_points.Count <3 ) throw new InvalidOperationException("Для определения направления вращения линии нужно как минимум 3 точки");


			float sum = 0;

			for (int i = 0; i < _points.Count-1; i++)
			{

				sum += _points[i].X * _points[i + 1].Y - _points[i + 1].X * _points[i].Y;

			}

			if (sum < 0) return true;
			return false;

		}


		

		/// <summary>
		/// Удаляет соседние одинаковые точки
		/// </summary>
		public virtual void ErodeDoublings()
		{

			int end = _points.Count - 1;

			for (int i = 0; i < end; i++)
			{
				if (_points[i] == _points[i + 1])
				{
					end--;

					_points.RemoveAt(i);
					i--;
				}

			}



		}

		/// <summary>
		/// Если начало и конец одинаковые -удаляет одно из
		/// </summary>
		public virtual void RemoveSameEnd()
		{
			if(_points.Count >0) {

				if (_points[0] == _points[_points.Count - 1])
				{

					_points.RemoveAt(0);
				}
			
			
			
			
			
			}




		}

		

		public virtual string ToJTSLineString()
		{
			var sb = new StringBuilder();

			sb.Append("LINESTRING (");


			for (int i = 0; i < _points.Count; i++)
			{
				sb.Append(_points[i].X.ToString(CultureInfo.InvariantCulture) + " " +
						(_points[i].Y).ToString(CultureInfo.InvariantCulture));

				if (i != _points.Count - 1) sb.Append(", ");

			}

			sb.Append(")");


			return sb.ToString();




		}


		public static bool IsSame(ISwgPath one, ISwgPath two)
		{

			var oneenum = one.Points.GetEnumerator();

			var twoenum = two.Points.GetEnumerator();

			while(oneenum.MoveNext() && twoenum.MoveNext())
			{
				if (oneenum.Current != twoenum.Current) return false;
			}

			//один закончился, другой нет
			if (oneenum.MoveNext() || twoenum.MoveNext()) return false;
			else return true;
			


		}


	}
}
