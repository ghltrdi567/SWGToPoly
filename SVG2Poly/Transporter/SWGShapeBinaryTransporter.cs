using SVG2Poly.SWGPath;
using SVG2Poly.SWGPathArranger;
using SVG2Poly.SWGShape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Transporter
{
	internal class SWGShapeBinaryTransporter : IBinarySWGTransporter<SWGShapeBase>
	{
		#region Чтение

		public SWGShapeBase Read(BinaryReader reader)
		{
			var result = new SWGShapeBase();

			int boundCount = reader.ReadInt32();
			int triangleCount = reader.ReadInt32();


			for (int i = 0; i < boundCount; i++)
			{
				result.AddBoundary(ReadPath(reader));
			}

			for (int i = 0; i < triangleCount; i++)
			{
				result.AddTriangle(ReadTriangle(reader));
			}




			return result;
		}




		public static Vector2 ReadVector(BinaryReader reader) => new Vector2(reader.ReadSingle(), reader.ReadSingle());

		public static SWGShape.SWGTriangle ReadTriangle(BinaryReader reader) => new SWGTriangle(ReadVector(reader), ReadVector(reader), ReadVector(reader));

		public static ISwgPath ReadPath(BinaryReader reader)
		{
			var result = new SwgPathBase();

			int pointCount = reader.ReadInt32();

			for (int i = 0; i < pointCount; i++)
			{
				result.AddPoint(ReadVector(reader));
			}

			return result;
		}



		#endregion

		 
		#region Запись




		public void Write(BinaryWriter writer, SWGShapeBase source)
		{

			writer.Write(source.PolygonBoundaries.Count());
			writer.Write(source.InternalTriangles.Count());

			
			int Counter = 0;
			foreach (var polygon in source.PolygonBoundaries) {
			
				if(polygon != null)
				{
					Write(writer, polygon);
					Counter++;

				}
			
			
			}
			if(Counter != source.PolygonBoundaries.Count()) throw  new Exception("ОШибка в записи swgShape");


			Counter = 0;
			foreach (var triangle in source.InternalTriangles)
			{

					Write(writer, triangle);
					Counter++;

			}
			if (Counter != source.InternalTriangles.Count()) 
				throw new Exception("ОШибка в записи swgShape");


		}

		protected static void Write(BinaryWriter writer, SWGPath.ISwgPath shape)
		{

			writer.Write(shape.PointsCount);


			int poiCounter = 0;
			foreach (var point in shape.Points) {
			
				Write(writer, point);
				poiCounter++;
			
			}

			if (poiCounter != shape.PointsCount) throw new Exception("ОШибка в записи swgPath");
		}

		protected static void Write(BinaryWriter writer, SWGShape.SWGTriangle triangle)
		{

				Write(writer, triangle.P1);
				Write(writer, triangle.P2);
				Write(writer, triangle.P3);

		}

		protected static void Write(BinaryWriter writer, Vector2 vec)
		{

			writer.Write(vec.X);
			writer.Write(vec.Y);


		}



		#endregion







	}
}
