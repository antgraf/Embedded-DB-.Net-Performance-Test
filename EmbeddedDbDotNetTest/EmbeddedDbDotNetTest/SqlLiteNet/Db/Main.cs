using SQLite;

namespace EmbeddedDbDotNetTest.SqlLiteNet.Db
{
	public class Main
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int A { get; set; }
		public bool B { get; set; }
		[MaxLength(1)]
		public string C { get; set; }
		[MaxLength(64)]
		public string D { get; set; }
		public int E { get; set; }
		public bool F { get; set; }
		[MaxLength(1)]
		public string G { get; set; }
		[MaxLength(64)]
		public string H { get; set; }
		[Indexed]
		public int CommentId { get; set; }
	}
}
