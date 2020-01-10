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
										D.PID,
										D.SDID,
										PP.PatientID,
										PP.LAST,
										PP.FIRST,
										convert(varchar(255), PP.BIRTHDATE, 110) dob,
										PP.SEX,
										isnull(PP.Address1, '') + ' ' + isnull(PP.Address2, '') + ' ' + isnull(PP.City, '') + ' ' + isnull(PP.State, '') as address,
										case when PP.Phone1 not like '' and PP.Phone1 is not null then PP.Phone1 + ' (' + PP.Phone1Type + ') ' else '' end
										+ case when PP.Phone2 not like '' and PP.Phone2 is not null then ' ' + PP.Phone2 + ' (' + PP.Phone2Type + ') ' else '' end
										+ case when PP.Phone3 not like '' and PP.Phone3 is not null then ' ' + PP.Phone3 + ' (' + PP.Phone3Type + ') ' else '' end
										as phone,
										isnull(pp.emailaddress, '') email,
										d.clinicaldate clindate,
										isnull(d.summary, '') summary
									from
										document d
										inner	join doctypes dt on d.doctype = dt.dtid
										inner join patientprofile pp on pp.pid = d.pid
									where
										d.xid in (
										100000000000000000000000000000000000,
										1000000000000000000
										)
										and d.finalsign = 1
										and d.status = 'S'
										and d.doctype != 24";
			
			SqlDataReader rdr = null;
			String pid;
			String sdid;
			String patientid;
			String last;
			String first;
			String dob;
			String sex;
			String address;
			String phone;
			String email;
			String clindate;
			String summary;
			String path;

			rdr = command.ExecuteReader();
			
			while(rdr.Read()){
				pid = rdr["pid"].ToString();
				sdid = rdr["sdid"].ToString();
				patientid = rdr["patientid"].ToString();
				last = rdr["last"].ToString();
				first = rdr["first"].ToString();
				dob = rdr["dob"].ToString();
				sex = rdr["sex"].ToString();
				address = rdr["address"].ToString();
				phone = rdr["phone"].ToString();
				email = rdr["email"].ToString();
				clindate = rdr["clindate"].ToString();
				summary = rdr["summary"].ToString();
				//Console.WriteLine(email);

				String data = $"Patient : {first} {last}\n"
								+ $"DOB: {sex}    Sex: {sex}\n"
								+ $"PatientID: {patientid}\n"
								+ $"Phone: {phone}\n"
								+ $"E - Mail : {email}\n"
								+ $"Address: {address}\n"
								+ $"Document Date : {dob}\n"
								+ $"Document Summary : {summary}";
			
			path = $"D:\\sandbox\\netdoc\\01_docs_extracted\\{sdid}\\{sdid}\\summary.txt";
			
			//Console.WriteLine(data);
			Console.WriteLine(path);
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, data);
			}
		}
	}
	
	watch.Stop();
	var elapsedMs = milliReadable(watch.ElapsedMilliseconds);
	Console.WriteLine("Elapsed time - {0}", elapsedMs);
}

private static string milliReadable(long ms)
{
	TimeSpan t = TimeSpan.FromMilliseconds(ms);
	string answer = t.ToString(@"d\ \d\a\y\s\ h\ \h\o\u\r\s\ mm\ \m\i\n\u\t\e\s\ ss\ \s\e\c\o\n\d\s\ fff\ \m\i\l\l\i\s\e\c\o\n\d\s");
	return answer;
}