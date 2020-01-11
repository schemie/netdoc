<Query Kind="Expression" />

void Main()
{
	var xmlPath = @"X:\projects\PPNL\document\documents_extracted_xml";
	var extractedPath = @"X:\projects\PPNL\document\documents_extracted";
	var count_created = 0;
	var count_exists = 0;

	foreach (var patientFolder in Directory.EnumerateDirectories(xmlPath))
	{
		foreach (var documentFolder in Directory.EnumerateDirectories(patientFolder))
		{
			foreach (var dateFolder in Directory.EnumerateDirectories(documentFolder))
			{
				//Console.WriteLine(dateFolder);
				System.Threading.Tasks.Parallel.ForEach(Directory.EnumerateDirectories(dateFolder), new System.Threading.Tasks.ParallelOptions() { MaxDegreeOfParallelism = 4 }, (linkFolder) =>
			   {
				   var htmlPath = linkFolder.Replace(xmlPath, extractedPath);
				   Directory.CreateDirectory(htmlPath);

				   if (!File.Exists(Path.Combine(htmlPath, "data.html")))
				   {
					   var xsl = new System.Xml.Xsl.XslCompiledTransform();
					   xsl.Load(Path.Combine(linkFolder, "template.xsl"));
					   xsl.Transform(Path.Combine(linkFolder, "data.xml"), Path.Combine(htmlPath, "data.html"));
					   count_created++;
					   Console.WriteLine(count_created + " - " + Path.Combine(htmlPath, "data.html"));
				   }
				   else
				   {
					   count_exists++;
				   }
					//Console.WriteLine(Path.Combine(htmlPath, "data.html"));
				});
			}
		}
	}
	Console.WriteLine("Created : {0}, Already Existed: {1}", count_created, count_exists);
}