
using SVG2Poly.Helpers;

//Input directory with swg files 
string inputDirectory = Path.Combine(Environment.CurrentDirectory, "IN");

//Output directory with files with polygons
string outDirectory = Path.Combine(Environment.CurrentDirectory, "OUT");

if(!Directory.Exists(inputDirectory)) Directory.CreateDirectory(inputDirectory);
if(!Directory.Exists(outDirectory)) Directory.CreateDirectory(outDirectory);

string[] Files = Directory.GetFiles(inputDirectory, "*.svg");

for (int i = 0; i < Files.Length; i++)
{
	Console.WriteLine("Файл: " + Files[i] + Environment.NewLine);


	//SWGFileToBinaryTriangles.SvgFileToBinaryTriangles(Files[i], outDirectory);
	SWGFileToBinaryTriangles.SvgFileToBinaryShapes(Files[i], outDirectory);
}