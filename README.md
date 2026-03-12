## Парсит фигуру в формате SWG и преобразует в набор треугольников

## Пример использования

```csharp
//Input directory with swg files 
string inputDirectory = Path.Combine(Environment.CurrentDirectory, "IN");

//Output directory with files with polygons
string outDirectory = Path.Combine(Environment.CurrentDirectory, "OUT");

//Если входные и выходные директории не сущеуствуют - создаём
if(!Directory.Exists(inputDirectory)) Directory.CreateDirectory(inputDirectory);
if(!Directory.Exists(outDirectory)) Directory.CreateDirectory(outDirectory);

//Получаем файлы svg во входной директории
string[] Files = Directory.GetFiles(inputDirectory, "*.svg");

//ОБрабатываем каждый файл по отдельности
for (int i = 0; i < Files.Length; i++)
{
	Console.WriteLine("Файл: " + Files[i] + Environment.NewLine);

    //Парсит фигуру в полигоны (внешнюю границу и внутренние дыры), 
    триангулирует их (превращает в набор треугольников внутри полигона) 
    //и сохраняет всё это в бинарых файлах в выходной директории
	SWGFileToBinaryTriangles.SvgFileToBinaryShapes(Files[i], outDirectory);
}

```