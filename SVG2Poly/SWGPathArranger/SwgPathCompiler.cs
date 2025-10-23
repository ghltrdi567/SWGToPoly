using Svg.Pathing;
using SVG2Poly.SWGPath;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.SWGPathArranger
{
	internal class SwgPathCompiler : ISwgPathCompiler
	{
		/// <summary>
		/// НА сколько точек будет разбиваться кривая
		/// </summary>
		public int CurveDiscreteCount { get; protected set; }

		public SwgPathCompiler(int discreteCount)
		{
			CurveDiscreteCount = int.Abs(discreteCount);
		}

		public IEnumerable<ISwgPath> CompilePaths(SvgPathSegmentList segments)
		{
			List<SwgPathBase> result = new List<SwgPathBase>();

			if (segments == null) return result;

			SwgPathBase currentPath = new SwgPathBase();



			Vector2? LastControl = null;

			foreach (SvgPathSegment path in segments) 
			{

				
				//начало нового пути
				if(path is SvgMoveToSegment moveSeg)
				{
					//отправляем путь в результирующий список, начинаем новый путь
					if(currentPath.PointsCount != 0)
					{
						result.Add(currentPath);

						currentPath = new SwgPathBase();



					}

					currentPath.AddPoint(path.IsRelative? currentPath.LastPoint + GetNumber(path.End): GetNumber(path.End));


					LastControl = null;
					continue;

				}

				if (path is SvgClosePathSegment closeSeg)
				{
					//отправляем путь в результирующий список, начинаем новый путь
					if (currentPath.PointsCount != 0)
					{
						

						currentPath.ClosePath();



					}
					
					continue;
				}


				if (path is SvgLineSegment lineSeg)
				{
					

					currentPath.AddPoint(path.IsRelative ? currentPath.LastPoint + GetNumber(path.End) : GetNumber(path.End));

					LastControl = null;
					continue;

				}


				if (path is SvgArcSegment arcSeg)
				{
					//Конечная точка кривой в абсолютных координатах
					Vector2 endPoint = path.IsRelative ? currentPath.LastPoint + GetNumber(path.End) : GetNumber(path.End);


					//Создаём кривую в абсолютных координатах
					Curves.ArcCurve curve = new Curves.ArcCurve(
						currentPath.LastPoint,
						endPoint, 
						arcSeg.Angle, 
						new Vector2(arcSeg.RadiusX, arcSeg.RadiusY), 
						arcSeg.Size == SvgArcSize.Large, 
						arcSeg.Sweep == SvgArcSweep.Positive);




					for (int i = 0; i < CurveDiscreteCount; i++)
					{
						float t = (float)i / CurveDiscreteCount;

						currentPath.AddPoint(curve.ComputePoint(t));

					}




					currentPath.AddPoint(endPoint);

					LastControl = null;
					continue;

				}


				if (path is SvgCubicCurveSegment cubSeg)
				{
					//Конечная точка кривой в абсолютных координатах
					Vector2 endPoint = path.IsRelative ? currentPath.LastPoint + GetNumber(path.End) : GetNumber(path.End);

					Vector2 Control1 = path.IsRelative ? currentPath.LastPoint + GetNumber(cubSeg.FirstControlPoint): GetNumber(cubSeg.FirstControlPoint);
					Vector2 Control2 = path.IsRelative ? currentPath.LastPoint + GetNumber(cubSeg.SecondControlPoint): GetNumber(cubSeg.SecondControlPoint);

					//Создаём кривую в абсолютных координатах
					Curves.CubicCurve curve = new Curves.CubicCurve(currentPath.LastPoint, endPoint, Control1, Control2);




					for (int i = 0; i < CurveDiscreteCount; i++)
					{
						float t = (float)i / CurveDiscreteCount;

						currentPath.AddPoint(curve.ComputePoint(t));

					}




					currentPath.AddPoint(endPoint);

					LastControl = Control2;
					continue;

				}



				if (path is SvgQuadraticCurveSegment quaSeg)
				{
					//Конечная точка кривой в абсолютных координатах
					Vector2 endPoint = path.IsRelative ? currentPath.LastPoint + GetNumber(path.End) : GetNumber(path.End);


					bool hasControl = IsLimited(quaSeg.ControlPoint.X) && IsLimited(quaSeg.ControlPoint.Y);

					if (!hasControl)
					{
						if (LastControl == null) throw new Exception("не удалось найти точку контроля предыдущего сегмента");
						else {

							if (path.IsRelative)
							{
								quaSeg.ControlPoint = new PointF(LastControl.Value.X - currentPath.LastPoint.X, LastControl.Value.Y - currentPath.LastPoint.Y);
							}
							else
							{
								quaSeg.ControlPoint = new PointF(LastControl.Value.X, LastControl.Value.Y);
							}
						
						} 
					}

					Vector2 Control = path.IsRelative ? currentPath.LastPoint + GetNumber(quaSeg.ControlPoint) : GetNumber(quaSeg.ControlPoint);


					//Создаём кривую в абсолютных координатах
					Curves.QuadraticCurve curve = new Curves.QuadraticCurve(currentPath.LastPoint, endPoint, Control, !hasControl);




					for (int i = 0; i < CurveDiscreteCount; i++)
					{
						float t = (float)i / CurveDiscreteCount;

						currentPath.AddPoint(curve.ComputePoint(t));

					}




					currentPath.AddPoint(endPoint);

					LastControl = curve.LastControlPoint;
					continue;

				}





				LastControl = null;
			}

			result.Add(currentPath);


			
			foreach(var path in result)
			{
				//Убирем соседние одинаковые значения
				path?.ErodeDoublings();


			}




			return result;
		}



		public static Vector2 GetNumber(PointF point) => new Vector2(EnsureLimited(point.X), EnsureLimited(point.Y));


		public static float EnsureLimited(float source) => float.IsNaN(source) || float.IsInfinity(source) ? 0: source;

		public static bool IsLimited(float source) => !(float.IsNaN(source) || float.IsInfinity(source));



	}
}
