using SQLite;

namespace EmbeddedDbDotNetTest.SqlLiteNet.Db
{
	public class Comments
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Body { get; set; }
	}
}
