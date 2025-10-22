using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Curves
{
	internal struct ArcCurve : IParametricCurveBase
	{
		public ArcCurve(Vector2 start, Vector2 end, float rotationRad, Vector2 radiuses, bool isBigArc, bool isArcSweep)
		{
			Start = start;
			End = end;
			RotationRad = rotationRad;
			Radiuses = radiuses;
			IsBigArc = isBigArc;
			IsArcSweep = isArcSweep;


			//нам нужны только положительные радиусы
			radiuses.X = float.Abs(radiuses.X);
			radiuses.Y = float.Abs(radiuses.Y);

			if (radiuses.X <= 0 || radiuses.Y <= 0) throw new ArgumentException("Zero radius");

			//https://www.w3.org/TR/SVG/implnote.html#ArcConversionEndpointToCenter
			//B.2.4

			float middle_x = (Start.X - End.X) / 2;
			float middle_y = (Start.Y - End.Y) / 2;

			float Cos_R = MathF.Cos(rotationRad);
			float Sin_R = MathF.Sin(rotationRad);


			// r1`
			Vector2 middleEnds = new Vector2(

				Cos_R * middle_x + Sin_R * middle_y,

				-Sin_R * middle_x + Cos_R * middle_y

			);


			//квадраты полученных значений
			float RxSq = radiuses.X * radiuses.X;
			float RySq = radiuses.Y * radiuses.Y;

			float x1Sq = middleEnds.X * middleEnds.X;
			float y1Sq = middleEnds.Y * middleEnds.Y;

			//проверка, что радиусы достаточно большие

			float Lamda = x1Sq / RxSq + y1Sq / RySq;

			//Радиусы слишком маленкие, нужно их увеличить
			if (Lamda > 1)
			{
				float sqrt = MathF.Sqrt(Lamda);

				radiuses.X *= sqrt;
				radiuses.Y *= sqrt;

				RxSq *= Lamda;
				RySq *= Lamda;

			}


			//c`

			float work1 = radiuses.X * middleEnds.Y / radiuses.Y;
			float work2 = radiuses.Y * middleEnds.X / radiuses.X;



			float bigWork = (RxSq * RySq - RxSq * y1Sq - RySq * x1Sq) / (RxSq * y1Sq + RySq * x1Sq);

			bigWork = isBigArc == isArcSweep ? -MathF.Sqrt(bigWork) : MathF.Sqrt(bigWork);

			Vector2 Cmedi = new Vector2(


				work1 * bigWork,

				-work2 * bigWork

			);


			//c

			this.C = new Vector2(

				Cmedi.X * Cos_R - Cmedi.Y * Sin_R + middle_x + End.X,      //последние 2 слагаемых = (Start.X + End.X) / 2;


				Cmedi.X * Sin_R + Cmedi.Y * Cos_R + middle_y + End.Y      //последние 2 слагаемых = (Start.Y + End.Y) / 2;


			);


			Vector2 theta1Angle = new Vector2(

				(middleEnds.X - Cmedi.X) / radiuses.X,

				(middleEnds.Y - Cmedi.Y) / radiuses.Y

			);

			Vector2 theta2Angle = new Vector2(

				(-middleEnds.X - Cmedi.X) / radiuses.X,

				(-middleEnds.Y - Cmedi.Y) / radiuses.Y

			);


			float Theta1 = AngleBetween(new Vector2(1, 0), theta1Angle);

			float DeltaThet = AngleBetween(theta1Angle, theta2Angle) % (MathF.PI * 2);

			if (DeltaThet > 0)
			{
				if (!isArcSweep) DeltaThet = -DeltaThet;
			}
			else
			{
				if (isArcSweep) DeltaThet = -DeltaThet;
			}

			this.Theta_0 = Theta1;
			this.DetlaTheta = DeltaThet;

		}

		public Vector2 Start { get; init; }

		public Vector2 End { get; init; }

		public float RotationRad { get; init; }

		public Vector2 Radiuses { get; init; }

		public bool IsBigArc { get; init; }

		public bool IsArcSweep { get; init; }

		public Vector2 C {  get; init; }

		public float Theta_0 { get; init; }
		
		public float DetlaTheta { get; init; }

		public Vector2 ComputePoint(float t)
		{

			float ThetaRadians = Theta_0 + DetlaTheta * t;


			float cosF = MathF.Cos(RotationRad);
			float sinF = MathF.Sin(RotationRad);

			float cosT = MathF.Cos(ThetaRadians);
			float sinT = MathF.Sin(ThetaRadians);



			return new Vector2(

				C.X + Radiuses.X * cosT * cosF - Radiuses.Y * sinT * sinF,

				C.Y + Radiuses.X * cosT * sinF + Radiuses.Y * sinT * cosF

			);
		}


		public static float AngleBetween(Vector2 One, Vector2 Two)
		{

			float result = MathF.Acos(Vector2.Dot(One, Two) / (One.Length() * Two.Length()));


			//С каким знаком будем угол между векторами
			float signDescriminator = One.X * Two.Y - One.Y * Two.X;


			if (signDescriminator < 0) result = -result;


			return result;
		}


	}
}
