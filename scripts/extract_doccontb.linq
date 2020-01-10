<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>System.Data.SqlClient</NuGetReference>
  <NuGetReference>System.Globalization</NuGetReference>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	using (var connection = new SqlConnection(@"Data Source=192.168.1.13;Initial Catalog=Demo;User Id=sa;Password=sa;"))
	{
		connection.Open();

		using (var command = connection.CreateCommand())
		{
			command.CommandText = @"select top 10
										d.pid,
										d.sdid,
										d.clinicaldate,
										case d.xid
											when 100000000000000000000000000000000000 then d.sdid
											when 1000000000000000000 then d.sdid
											else d.xid
										end as parent,
										dc.contb_time as date,
										coalesce(
											dc.username,
											ltrim(
										        
										            case coalesce(dcu.firstname, '') when '' then '' else dcu.firstname end +
														(
										                case coalesce(dcu.middlename, '') when '' then '' else (' ' + dcu.middlename) end +
										                case coalesce(dcu.lastname, '') when '' then '' else (' ' + dcu.lastname) end
										                )
										        )
										) as signer,
										(select text from systemtemp where type = 4 and name = 'Signature Line') as format,
										coalesce(dc.linkid, 0) as linkid
									from
										document d
										inner join doccontb dc on d.sdid = dc.sdid
										left join usr dcu on dc.usrid = dcu.pvid
									where
										dc.contb_action in (3, 7, 8)
										and d.finalsign = 1
										and d.status = 'S'
										and d.doctype != 24
										order by d.sdid, dc.contb_time";

			SqlDataReader rdr = null;
			String pid;
			String sdid;
			String clindate;
			String parent;
			String linkid;
			String path;
			String format;
			String signer;
			DateTime contbtime;

			rdr = command.ExecuteReader();

			while (rdr.Read())
			{
				pid = rdr["pid"].ToString();
				sdid = rdr["sdid"].ToString();
				clindate = rdr["clinicaldate"].ToString();
				parent = rdr["parent"].ToString();
				linkid = rdr["linkid"].ToString();
				format = rdr["format"].ToString();
				signer = rdr["signer"].ToString();
				contbtime = (DateTime) rdr["date"];
				
				int firstPlaceholder = format.IndexOf("%s");
				int secondPlaceholder = format.IndexOf("%s", firstPlaceholder + 1);
				int thirdPlaceholder = format.IndexOf("%s", secondPlaceholder + 1);
				String date = contbtime.ToString("MM/dd/yyyy");
				String time = contbtime.ToString("h:mm a");
				
				String signature = format.Substring(0, firstPlaceholder) + signer + format.Substring(firstPlaceholder + 2, secondPlaceholder - (firstPlaceholder + 2)) + date + format.Substring(secondPlaceholder + 2, thirdPlaceholder - (secondPlaceholder + 2)) + time + format.Substring(thirdPlaceholder + 2);			

				Console.WriteLine(signature);

				path = $"D:\\sandbox\\netdoc\\01_docs_extracted\\{sdid}\\{parent}\\{clindate}\\{linkid}\\signature_{linkid}.rtf";

				Console.WriteLine(path);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.AppendAllText(path, signature);
			}
		}
	}
}