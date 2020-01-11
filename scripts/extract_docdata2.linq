<Query Kind="Program">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

void Main()
{
	var watch = System.Diagnostics.Stopwatch.StartNew();

	using (var connection = new SqlConnection(@"Data Source=192.168.1.13;Initial Catalog=Demo;User Id=sa;Password=sa;"))
	{
		connection.Open();

		using (var command = connection.CreateCommand())
		{
			command.CommandText = @"select
										cast(d.pid as varchar(2000)) pid,
										d.sdid,
										cast(d.clinicaldate as varchar(2000)) clinicaldate,
										case d.xid
											when 100000000000000000000000000000000000 then cast(d.sdid as varchar(2000))
											when 1000000000000000000 then cast(d.sdid as varchar(2000))
											else cast(d.xid as varchar(2000))
										end as parent,
										dd.data as data,
										coalesce(dd.linkid, 0) linkid,
										x.data as stylesheet
									from
										document d
										inner join docdata2 dd on d.sdid = dd.sdid
										inner join xsldata x on d.xslid = x.xslid
									where 
										d.mimetype = 'text/xml'
										and d.finalsign = 1
										and d.status = 'S'
										and d.doctype != 24
										--and d.PID in (select PID from ##cus_mdemr_test_pats)
									order by
										parent, d.clinicaldate, dd.ddid";

			SqlDataReader rdr = null;
			String pid;
			String sdid;
			String clindate;
			String parent;
			Byte[] data;
			Byte[] stylesheet;
			String linkid;
			String path;
			String path_template;
			String html_path;

			rdr = command.ExecuteReader();

			while (rdr.Read())
			{
				pid = rdr["pid"].ToString();
				sdid = rdr["sdid"].ToString();
				clindate = rdr["clinicaldate"].ToString();
				parent = rdr["parent"].ToString();
				data = (byte[])rdr["data"];
				linkid = rdr["linkid"].ToString();
				stylesheet = (byte[])rdr["stylesheet"];

				path = $"D:\\sandbox\\netdoc\\01_docs_extracted_xml\\{pid}\\{parent}\\{clindate}\\{linkid}\\data.xml";
				path_template = $"D:\\sandbox\\netdoc\\01_docs_extracted_xml\\{pid}\\{parent}\\{clindate}\\{linkid}\\template.xml";
				html_path = $"D:\\sandbox\\netdoc\\01_docs_extracted\\{pid}\\{parent}\\{clindate}\\{linkid}\\data.html";

				//Console.WriteLine(path);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.WriteAllBytes(path, data);
				File.WriteAllBytes(path_template, stylesheet);

				if (!File.Exists(html_path))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(html_path));
					var xsl = new System.Xml.Xsl.XslCompiledTransform();
					xsl.Load(path_template);
					xsl.Transform(path, html_path);
					//count_created++;
					//Console.WriteLine(count_created + " - " + Path.Combine(htmlPath, "data.html"));
				}
				else
				{
					//count_exists++;
				}
			}
		}
	}

	watch.Stop();
	var elapsedMs = milliReadable(watch.ElapsedMilliseconds);
	Console.WriteLine("DocData Elapsed time - {0}", elapsedMs);
}

private static string milliReadable(long ms)
{
	TimeSpan t = TimeSpan.FromMilliseconds(ms);
	string answer = TimeSpan.FromMilliseconds(ms).ToString(@"d\ \d\a\y\s\ h\ \h\o\u\r\s\ mm\ \m\i\n\u\t\e\s\ ss\ \s\e\c\o\n\d\s\ fff\ \m\i\l\l\i\s\e\c\o\n\d\s");
	return answer;
}