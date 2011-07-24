using System;

namespace EmbeddedDbDotNetTest
{
	static class Program
	{
		static void Main()
		{
			try
			{
				Console.WriteLine("DbTestSqlLiteAdo:");
				DbTestSqlLiteAdo dbSqlLiteAdo = new DbTestSqlLiteAdo();
				Test(dbSqlLiteAdo);
				Console.WriteLine("DbTestSqlLiteNet:");
				DbTestSqlLiteNet dbSqlLiteNet = new DbTestSqlLiteNet();
				Test(dbSqlLiteNet);
				Console.WriteLine("DbTestFireBird:");
				DbTestFireBird dbFireBird = new DbTestFireBird();
				Test(dbFireBird);
				Console.WriteLine("===");
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		private static void Test(DbTestBase db)
		{
			try
			{
				TestTimer time = new TestTimer(null);
				Console.WriteLine("PrepareDb");
				time.Start();
				PrepareDb(db);
				LogTime(time.Stop());
				Console.WriteLine("CreateDb");
				time.Start();
				CreateDb(db);
				LogTime(time.Stop());
				Console.WriteLine("ReadAll");
				time.Start();
				ReadAll(db);
				LogTime(time.Stop());
				Console.WriteLine("ReadRandom");
				time.Start();
				ReadRandom(db);
				LogTime(time.Stop());
				Console.WriteLine("ReadWriteAll");
				time.Start();
				ReadWriteAll(db);
				LogTime(time.Stop());
				Console.WriteLine("ReadWriteRandom");
				time.Start();
				ReadWriteRandom(db);
				LogTime(time.Stop());
				Console.WriteLine(".");
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		private static void LogTime(double sec)
		{
			Console.WriteLine("Time: {0:0.##} sec", sec);
		}

		private static void LogException(Exception exception)
		{
			Console.WriteLine("Error: {0}", exception.GetType());
		}

		private static void ReadWriteRandom(DbTestBase db)
		{
			try
			{
				db.ReadWriteRandom();
			}
			catch(Exception e)
			{
				LogException(e);
			}
		}

		private static void ReadWriteAll(DbTestBase db)
		{
			try
			{
				db.ReadWriteAll();
			}
			catch(Exception e)
			{
				LogException(e);
			}
		}

		private static void ReadRandom(DbTestBase db)
		{
			try
			{
				db.ReadRandom();
			}
			catch(Exception e)
			{
				LogException(e);
			}
		}

		private static void ReadAll(DbTestBase db)
		{
			try
			{
				db.ReadAll();
			}
			catch(Exception e)
			{
				LogException(e);
			}
		}

		static void PrepareDb(DbTestBase db)
		{
			try
			{
				db.PrepareDb();
			}
			catch(Exception e)
			{
				LogException(e);
			}
		}

		static void CreateDb(DbTestBase db)
		{
			try
			{
				db.CreateDb();
			}
			catch(Exception e)
			{
				LogException(e);
			}
		}
	}
}
