using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EmbeddedDbDotNetTest.SqlLiteNet.Db;
using SQLite;

namespace EmbeddedDbDotNetTest
{
	class DbTestSqlLiteNet : DbTestBase
	{
		private const string PathToDll = @"SqlLiteNet";
		private const string PathToDb = @"SqlLiteNetDb\dbtest.db";

		private readonly string pRandomString;
		private readonly Main pDefaultMain;
		private readonly Comments pDefaultComment;
		private readonly Tags pDefaultTag;

		public DbTestSqlLiteNet()
		{
			Random r = new Random();
			pRandomString = r.NextDouble().ToString();
			// blobs, chars are not supported
			pDefaultMain = new Main
			               {
			               	A = 666,
			               	B = false,
			               	C = null,
			               	D = null,
			               	E = 666,
			               	F = false,
			               	G = "W",
			               	H = pRandomString,
			               	CommentId = -1
			               };
			pDefaultComment = new Comments { Body = pRandomString };
			pDefaultTag = new Tags {Name = pRandomString, CommentId = -1};
			bool ok = SetDllDirectory(PathToDll);
			Debug.Assert(ok);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetDllDirectory(string lpPathName);

		private int CreateRecord(SQLiteConnection connection, int commentId, bool commentCreated, bool full)
		{
			// nulls are not supported
			pDefaultMain.C = full ? "W" : null;
			pDefaultMain.D = full ? pRandomString : null;
			pDefaultMain.CommentId = commentCreated ? commentId : -1;
			return connection.Insert(pDefaultMain);
		}

		private int CreateTag(SQLiteConnection connection, int commentId)
		{
			pDefaultTag.CommentId = commentId;
			return connection.Insert(pDefaultTag);
		}

		private int CreateComment(SQLiteConnection connection)
		{
			return connection.Insert(pDefaultComment);
		}

		#region Overrides of DbTestBase

		public override void PrepareDb()
		{
			// clean
			if(File.Exists(PathToDb))
			{
				File.Delete(PathToDb);
			}
			// create db & tables
			using(SQLiteConnection db = new SQLiteConnection(PathToDb))
			{
				db.CreateTable<Main>();
				db.CreateTable<Comments>();
				db.CreateTable<Tags>();
			}
		}

		public override void CreateDb()
		{
			using(SQLiteConnection connection = new SQLiteConnection(PathToDb))
			{
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
			using(SQLiteConnection connection = new SQLiteConnection(PathToDb))
			{
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = i+1;
					Main m = connection.Table<Main>().Where(r => r.Id == id).FirstOrDefault();
				}
			}
		}

		public override void ReadRandom()
		{
			using(SQLiteConnection connection = new SQLiteConnection(PathToDb))
			{
				Random rand = new Random();
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = rand.Next(RecordsCount) + 1;
					Main m = connection.Table<Main>().Where(r => r.Id == id).FirstOrDefault();
				}
			}
		}

		public override void ReadWriteAll()
		{
			using(SQLiteConnection connection = new SQLiteConnection(PathToDb))
			{
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = i+1;
					Main m = connection.Table<Main>().Where(r => r.Id == id).FirstOrDefault();
					//Debug.Assert(m != null);
					m.A++;
					connection.Update(m);
				}
			}
		}

		public override void ReadWriteRandom()
		{
			using(SQLiteConnection connection = new SQLiteConnection(PathToDb))
			{
				Random rand = new Random();
				for(int i = 0; i < RecordsCount; i++)
				{
					int id = rand.Next(RecordsCount) + 1;
					Main m = connection.Table<Main>().Where(r => r.Id == id).FirstOrDefault();
					//Debug.Assert(m != null);
					m.A++;
					connection.Update(m);
				}
			}
		}

		#endregion
	}
}
