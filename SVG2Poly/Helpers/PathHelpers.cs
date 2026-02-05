using EarClipperLib;
using SVG2Poly.SWGPath;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVG2Poly.Helpers
{
	internal class PathHelpers
	{

		public static List<Vector3m> ToMVectors(ISwgPath path)
		{

			List<Vector3m> result = new List<Vector3m>();

			foreach(var point in path.Points)
			{

				result.Add(new Vector3m(point.X, point.Y, 0));

			}

			return result;

		}



		public static List<Vector2> ToVectors3(List<Vector3m> source)
		{

			List<Vector2> result = new List<Vector2>();

			foreach (var point in source)
			{

				result.Add(new Vector2(point.X.ToSingle(), point.Y.ToSingle()));

			}

			return result;

		}


		public static string TrianglesDataToJTSPolygons(List<Vector3m> source)
		{

			if (source.Count % 3 != 0) throw new Exception("Исходное не делится на 3");


			var sb = new StringBuilder();

			sb.Append("MULTIPOLYGON (");



			for (int i = 0; i < source.Count/3; i++) {


				sb.Append("((");

				sb.Append(source[i*3].X.ToSingle().ToString(CultureInfo.InvariantCulture) + " ");
				sb.Append(source[i*3].Y.ToSingle().ToString(CultureInfo.InvariantCulture));
				sb.Append(", ");

				sb.Append(source[i * 3+1].X.ToSingle().ToString(CultureInfo.InvariantCulture) + " ");
				sb.Append(source[i * 3+1].Y.ToSingle().ToString(CultureInfo.InvariantCulture));
				sb.Append(", ");

				sb.Append(source[i * 3+2].X.ToSingle().ToString(CultureInfo.InvariantCulture) + " ");
				sb.Append(source[i * 3+2].Y.ToSingle().ToString(CultureInfo.InvariantCulture));
				sb.Append(", ");

				sb.Append(source[i * 3].X.ToSingle().ToString(CultureInfo.InvariantCulture) + " ");
				sb.Append(source[i * 3].Y.ToSingle().ToString(CultureInfo.InvariantCulture));




				sb.Append("))");


				if (i != source.Count / 3 - 1) sb.Append(", ");


			}



			sb.Append(")");


			return sb.ToString();

		}


		public static void WriteTrianglesDataToBinary(List<Vector3m> source, BinaryWriter writer)
		{
			if (source.Count % 3 != 0) throw new Exception("Исходное не делится на 3");

			writer.Write(source.Count);

			//по количеству треугольников
			for (int i = 0; i < source.Count/3; i++)
			{
				writer.Write(source[i * 3].X.ToSingle());
				writer.Write(source[i * 3].Y.ToSingle());


				writer.Write(source[i * 3 + 1].X.ToSingle());
				writer.Write(source[i * 3 + 1].Y.ToSingle());


				writer.Write(source[i * 3 + 2].X.ToSingle());
				writer.Write(source[i * 3 + 2].Y.ToSingle());


			}

		}

		public List<Vector3m> ReadTrianglesDataFromBinary(BinaryReader reader)
		{


			int VericeCount = reader.ReadInt32();

			if(VericeCount % 3 != 0) throw new Exception("количество вершин должно делиться на 3");

			List<Vector3m> result = new List<Vector3m>(VericeCount * 3);


			for (int i = 0; i < VericeCount; i++)
			{
				result.Add(new Vector3m(reader.ReadSingle(), reader.ReadSingle(), 0)); 

			}

			return result;
		}


	}
}
