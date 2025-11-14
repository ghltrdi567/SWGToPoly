using Clipper2Lib;
using EarClipperLib;
using Svg;
using Svg.Pathing;
using SVG2Poly.Helpers;
using SVG2Poly.SWGPath;
using SVG2Poly.SWGPathArranger;
using System.Numerics;
using System.Text;


Paths64 subj = new Paths64();
Paths64 clip = new Paths64();
subj.Add(Clipper.MakePath(new int[] { 100, 50, 10, 79, 65, 2, 65, 98, 10, 21 }));
clip.Add(Clipper.MakePath(new int[] { 98, 63, 4, 68, 77, 8, 52, 100, 19, 12 }));
Paths64 solution = Clipper.Intersect(subj, clip, FillRule.NonZero);





string inputDirectory = Path.Combine(Environment.CurrentDirectory, "IN"); 

string outDirectory = Path.Combine(Environment.CurrentDirectory, "OUT");


string[] Files = Directory.GetFiles(inputDirectory, "*.svg");

for (int i = 0; i < Files.Length; i++)
{
	Console.WriteLine("Файл: " + Files[i] + Environment.NewLine);


	SWGFileToBinaryTriangles.SvgFileToBinaryTriangles(Files[i], outDirectory);
}


