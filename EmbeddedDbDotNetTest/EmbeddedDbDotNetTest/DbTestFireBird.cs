using System;
using System.IO;
using System.Linq;
using EmbeddedDbDotNetTest.Exceptions;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;

namespace EmbeddedDbDotNetTest
{
	class DbTestFireBird : DbTestBase
	{
		private const string Login = "dbtest";
		private const string Password = "dbtest";
		private const int Dialect = 3;
		private const string Charset = "UTF8";
		private const string PathToDll = @"FireBird\fbembed.dll";
		private const string PathToDb = @"FireBirdDb\dbtest.fdb";
		private const string PathToSchema = @"fb_dbtest.sql";

		private readonly string pConnectionString;
		private readonly string pRandomString;

		public DbTestFireBird()
		{
			FbConnectionStringBuilder builder = new FbConnectionStringBuilder
			                                    {
			                                    	ServerType = FbServerType.Embedded,
			                                    	UserID = Login,
			                                    	Password = Password,
			                                    	Dialect = Dialect,
			                                    	Charset = Charset,
			                                    	ClientLibrary = PathToDll,
			                                    	Database = PathToDb
			                                    };
			pConnectionString = builder.ToString();
			Random r = new Random();
			pRandomString = r.NextDouble().ToString();
		}

		private int CreateRecord(FbConnection connection, int commentId, bool commentCreated, bool full)
		{
			string sqlt = full ?
				"INSERT INTO MAIN (A, B, C, D, E, F, G, H, COMMENT_ID) VALUES ({0}, {1}, '{2}', '{3}', {0}, {1}, '{2}', '{3}', {4}) RETURNING ID" :
				"INSERT INTO MAIN (E, F, G, H, COMMENT_ID) VALUES ({0}, {1}, '{2}', '{3}', {4}) RETURNING ID";
			string sql = string.Format(sqlt, 666, 0, 'W', pRandomString, commentCreated ? commentId.ToString() : "NULL");
			FbCommand insert = new FbCommand(sql, connection);
			return (int)insert.ExecuteScalar();
		}

		private int CreateTag(FbConnection connection, int commentId)
		{
			string sql = string.Format("INSERT INTO TAGS (NAME, COMMENT_ID) VALUES ('{0}', {1}) RETURNING ID", pRandomString, commentId);
			FbCommand insert = new FbCommand(sql, connection);
			return (int)insert.ExecuteScalar();
		}

		private int CreateComment(FbConnection connection)
		{
			string sql = string.Format("INSERT INTO COMMENTS (BODY) VALUES ('{0}') RETURNING ID", pRandomString);
			FbCommand insert = new FbCommand(sql, connection);
			return (int)insert.ExecuteScalar();
		}

		#region Overrides of DbTestBase

		public override void PrepareDb()
		{
			// clean
			if(File.Exists(PathToDb))
			{
				File.Delete(PathToDb);
			}
			// create db
			FbConnection.CreateDatabase(pConnectionString);
			// create tables
			string script;
			using(StreamReader reader = new StreamReader(PathToSchema))
			{
				script = reader.ReadToEnd();
			}
			FbScript sqls = new FbScript(script);
			if(sqls.Parse() <= 0)
			{
				throw new DbException("No DB schema found.");
			}
			using(FbConnection connection = new FbConnection(pConnectionString))
			{
				connection.Open();
				foreach(FbCommand create in sqls.Results.Select(sql => new FbCommand(sql, connection)))
				{
					create.ExecuteNonQuery();
				}
			}
		}

		public override void CreateDb()
		{
			using(FbConnection connection = new FbConnection(pConnectionString))
			{
				connection.Open();
				int commentId = -1;
				bool commentCreated;
				for(int i = 0; i < RecordsCount; i++)
				{
					commentCreated = i%2 == 0;
					if(commentCreated)	// create comment
					{
						commentId = CreateComment(connection);
						if(i % 4 == 0)	// create tag
						{
							int tagId = CreateTag(connection, commentId);
						}
					}
					int recId = CreateRecord(connection, commentId, commentCreated, i % 3 == 0);
				}
			}
		}

		public override void ReadAll()
		{
			using(FbConnection connection = new FbConnection(pConnectionString))
			{
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = i + 1;
					string sql = string.Format(sqlt, id);
					FbCommand main = new FbCommand(sql, connection);
					FbDataReader mainresult = main.ExecuteReader();
					while(mainresult.Read())
					{
						for(int c = 0; c < mainresult.FieldCount; c++)
						{
							string name = mainresult.GetName(c);
							object o = mainresult.GetValue(c);
						}
					}
				}
			}
		}

		public override void ReadRandom()
		{
			using(FbConnection connection = new FbConnection(pConnectionString))
			{
				Random r = new Random();
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = r.Next(RecordsCount) + 1;
					string sql = string.Format(sqlt, id);
					FbCommand main = new FbCommand(sql, connection);
					FbDataReader mainresult = main.ExecuteReader();
					while(mainresult.Read())
					{
						for(int c = 0; c < mainresult.FieldCount; c++)
						{
							string name = mainresult.GetName(c);
							object o = mainresult.GetValue(c);
						}
					}
				}
			}
		}

		public override void ReadWriteAll()
		{
			using(FbConnection connection = new FbConnection(pConnectionString))
			{
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				string sqlt1 = "UPDATE MAIN SET A = {0} WHERE ID = {1}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = i + 1;
					string sql = string.Format(sqlt, id);
					FbCommand main = new FbCommand(sql, connection);
					FbDataReader mainresult = main.ExecuteReader();
					while(mainresult.Read())
					{
						int a = mainresult.GetInt32(1);
						string sql1 = string.Format(sqlt1, ++a, id);
						FbCommand upd = new FbCommand(sql1, connection);
						upd.ExecuteNonQuery();
					}
				}
			}
		}

		public override void ReadWriteRandom()
		{
			using(FbConnection connection = new FbConnection(pConnectionString))
			{
				Random r = new Random();
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				string sqlt1 = "UPDATE MAIN SET A = {0} WHERE ID = {1}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = r.Next(RecordsCount) + 1;
					string sql = string.Format(sqlt, id);
					FbCommand main = new FbCommand(sql, connection);
					FbDataReader mainresult = main.ExecuteReader();
					while(mainresult.Read())
					{
						int a = mainresult.GetInt32(1);
						string sql1 = string.Format(sqlt1, ++a, id);
						FbCommand upd = new FbCommand(sql1, connection);
						upd.ExecuteNonQuery();
					}
				}
			}
		}

		#endregion
	}
}
