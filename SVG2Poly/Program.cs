using EarClipperLib;
using Svg;
using Svg.Pathing;
using SVG2Poly.Helpers;
using SVG2Poly.SWGPath;
using SVG2Poly.SWGPathArranger;
using System.Numerics;
using System.Text;



string inputDirectory = Path.Combine(Environment.CurrentDirectory, "IN"); 

string outDirectory = Path.Combine(Environment.CurrentDirectory, "OUT");


string[] Files = Directory.GetFiles(inputDirectory, "*.svg");

for (int i = 0; i < Files.Length; i++)
{
	Console.WriteLine("Файл: " + Files[i] + Environment.NewLine);


	SWGFileToBinaryTriangles.SvgFileToBinaryTriangles(Files[i], outDirectory);
}


