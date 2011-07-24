using System;

namespace EmbeddedDbDotNetTest.Exceptions
{
	public class DbException : ApplicationException
	{
		public DbException()
		{}

		public DbException(string msg)
			: base(msg)
		{}
	}
}
