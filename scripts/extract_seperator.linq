<Query Kind="Program">
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <NuGetReference>System.Globalization</NuGetReference>
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
								        d.pid,
								        d.sdid,
								        d.clinicaldate,
								        case d.xid
								            when 100000000000000000000000000000000000 then d.sdid
								            when 1000000000000000000 then d.sdid
								            else d.xid
								        end as parent,
								        coalesce(dd.linkid, 0) linkid,
								        (select text from systemtemp where type = 4 and name = 'Separator Line') as separator
								    from docdata dd
								    inner join document d on dd.sdid = d.sdid
								    where d.finalsign = 1
								    and d.status = 'S'
								    and d.doctype != 24
								    --and d.sdid in (select sdid from ${table})
								
								    UNION
								
								    select
								        d.pid,
								        d.sdid,
								        d.clinicaldate,
								        case d.xid
								            when 100000000000000000000000000000000000 then d.sdid
								            when 1000000000000000000 then d.sdid
								            else d.xid
								        end as parent,
								        coalesce(dd.linkid, 0) linkid,
								        (select text from systemtemp where type = 4 and name = 'Separator Line') as separator
								    from docdata2 dd
								    inner join document d on dd.sdid = d.sdid
								    where d.finalsign = 1
								    and d.status = 'S'
								    and d.doctype != 24
								    --and d.sdid in (select sdid from ${table})";

			SqlDataReader rdr = null;
			String pid;
			String sdid;
			String clindate;
			String parent;
			String data;
			String linkid;
			String path;

			rdr = command.ExecuteReader();

			while (rdr.Read())
			{
				pid = rdr["pid"].ToString();
				sdid = rdr["sdid"].ToString();
				clindate = rdr["clinicaldate"].ToString();
				parent = rdr["parent"].ToString();
				data = rdr["separator"].ToString();
				linkid = rdr["linkid"].ToString();

				path = $"D:\\sandbox\\netdoc\\01_docs_extracted\\{pid}\\{parent}\\{clindate}\\{linkid}\\zseparator.rtf";

				//Console.WriteLine(path);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.WriteAllText(path, data);
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