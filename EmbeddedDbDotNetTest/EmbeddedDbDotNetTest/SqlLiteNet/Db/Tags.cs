using SQLite;

namespace EmbeddedDbDotNetTest.SqlLiteNet.Db
{
	public class Tags
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[MaxLength(32)]
		public string Name { get; set; }
		[Indexed]
		public int CommentId { get; set; }
	}
}
