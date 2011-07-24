namespace EmbeddedDbDotNetTest
{
	abstract class DbTestBase
	{
		protected const int RecordsCount = 400;

		public abstract void PrepareDb();
		public abstract void CreateDb();
		public abstract void ReadAll();
		public abstract void ReadRandom();
		public abstract void ReadWriteAll();
		public abstract void ReadWriteRandom();
	}
}
