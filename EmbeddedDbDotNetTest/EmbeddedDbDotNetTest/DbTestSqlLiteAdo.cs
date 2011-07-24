using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using EmbeddedDbDotNetTest.Exceptions;

namespace EmbeddedDbDotNetTest
{
	class DbTestSqlLiteAdo : DbTestBase
	{
		private const string PathToDll = @"SqlLiteAdo";
		private const string PathToDb = @"SqlLiteAdoDb\dbtest.db";
		private const string PathToSchema = @"sqlite_dbtest.sql";

		private readonly string pConnectionString;
		private readonly string pRandomString;

		public DbTestSqlLiteAdo()
		{
			SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder {DataSource = PathToDb};
			pConnectionString = builder.ToString();
			Random r = new Random();
			pRandomString = r.NextDouble().ToString();
			bool ok = SetDllDirectory(PathToDll);
			Debug.Assert(ok);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetDllDirectory(string lpPathName);


		private int CreateRecord(SQLiteConnection connection, int commentId, bool commentCreated, bool full)
		{
			string sqlt = full ?
				"INSERT INTO MAIN (A, B, C, D, E, F, G, H, COMMENT_ID) VALUES ({0}, {1}, '{2}', '{3}', {0}, {1}, '{2}', '{3}', {4}); SELECT last_insert_rowid();" :
				"INSERT INTO MAIN (E, F, G, H, COMMENT_ID) VALUES ({0}, {1}, '{2}', '{3}', {4}); SELECT last_insert_rowid();";
			string sql = string.Format(sqlt, 666, 0, 'W', pRandomString, commentCreated ? commentId.ToString() : "NULL");
			SQLiteCommand insert = new SQLiteCommand(sql, connection);
			return Convert.ToInt32(insert.ExecuteScalar());
		}

		private int CreateTag(SQLiteConnection connection, int commentId)
		{
			string sql = string.Format("INSERT INTO TAGS (NAME, COMMENT_ID) VALUES ('{0}', {1}); SELECT last_insert_rowid();", pRandomString, commentId);
			SQLiteCommand insert = new SQLiteCommand(sql, connection);
			return Convert.ToInt32(insert.ExecuteScalar());
		}

		private int CreateComment(SQLiteConnection connection)
		{
			string sql = string.Format("INSERT INTO COMMENTS (BODY) VALUES ('{0}'); SELECT last_insert_rowid();", pRandomString);
			SQLiteCommand insert = new SQLiteCommand(sql, connection);
			return Convert.ToInt32(insert.ExecuteScalar());
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
			SQLiteConnection.CreateFile(PathToDb);
			// create tables
			string script;
			using(StreamReader reader = new StreamReader(PathToSchema))
			{
				script = reader.ReadToEnd();
			}
			if(string.IsNullOrEmpty(script))
			{
				throw new DbException("No DB schema found.");
			}
			using(SQLiteConnection connection = new SQLiteConnection(pConnectionString))
			{
				connection.Open();
				SQLiteCommand create = new SQLiteCommand(script, connection);
				int count = create.ExecuteNonQuery();
			}
		}

		public override void CreateDb()
		{
			using(SQLiteConnection connection = new SQLiteConnection(pConnectionString))
			{
				connection.Open();
				int commentId = -1;
				bool commentCreated;
				for(int i = 0; i < RecordsCount; i++)
				{
					commentCreated = i % 2 == 0;
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
			using(SQLiteConnection connection = new SQLiteConnection(pConnectionString))
			{
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = i + 1;
					string sql = string.Format(sqlt, id);
					SQLiteCommand main = new SQLiteCommand(sql, connection);
					SQLiteDataReader mainresult = main.ExecuteReader();
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
			using(SQLiteConnection connection = new SQLiteConnection(pConnectionString))
			{
				Random r = new Random();
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = r.Next(RecordsCount) + 1;
					string sql = string.Format(sqlt, id);
					SQLiteCommand main = new SQLiteCommand(sql, connection);
					SQLiteDataReader mainresult = main.ExecuteReader();
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
			using(SQLiteConnection connection = new SQLiteConnection(pConnectionString))
			{
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				string sqlt1 = "UPDATE MAIN SET A = {0} WHERE ID = {1}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = i + 1;
					string sql = string.Format(sqlt, id);
					SQLiteCommand main = new SQLiteCommand(sql, connection);
					SQLiteDataReader mainresult = main.ExecuteReader();
					while(mainresult.Read())
					{
						int a = mainresult.GetInt32(1);
						string sql1 = string.Format(sqlt1, ++a, id);
						SQLiteCommand upd = new SQLiteCommand(sql1, connection);
						upd.ExecuteNonQuery();
					}
				}
			}
		}

		public override void ReadWriteRandom()
		{
			using(SQLiteConnection connection = new SQLiteConnection(pConnectionString))
			{
				Random r = new Random();
				connection.Open();
				string sqlt = "SELECT * FROM MAIN WHERE ID = {0}";
				string sqlt1 = "UPDATE MAIN SET A = {0} WHERE ID = {1}";
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = r.Next(RecordsCount) + 1;
					string sql = string.Format(sqlt, id);
					SQLiteCommand main = new SQLiteCommand(sql, connection);
					SQLiteDataReader mainresult = main.ExecuteReader();
					while(mainresult.Read())
					{
						int a = mainresult.GetInt32(1);
						string sql1 = string.Format(sqlt1, ++a, id);
						SQLiteCommand upd = new SQLiteCommand(sql1, connection);
						upd.ExecuteNonQuery();
					}
				}
			}
		}

		#endregion
	}
}
