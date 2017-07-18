using System.Data;
using CustomAttributeGenerator;
using Moq;
using Xunit;

namespace Tests.Unit.EntityDocExtension
{
	public class DatabaseCustomAttributeSourceTests
	{
		public DatabaseCustomAttributeSourceTests()
		{
		    _command.SetupGet(c => c.Parameters)
		            .Returns(new Mock<IDataParameterCollection> { DefaultValue = DefaultValue.Mock }.Object);

		    _connection.Setup(c => c.CreateCommand())
		               .Returns(_command.Object);

			_underTest = new DatabaseCustomAttributeSource("connectionString", _ => _connection.Object);
		}

		[Fact]
		public void Test_Connection_Disposed()
		{
			// Act.
			_underTest.Dispose();

			// Assert.
			_connection.Verify(c => c.Dispose(), Times.Once());
		}

		private readonly DatabaseCustomAttributeSource _underTest;

		private readonly Mock<IDbCommand> _command = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };
		private readonly Mock<IDbConnection> _connection = new Mock<IDbConnection> { DefaultValue = DefaultValue.Mock };
	}
}