using System;

namespace _Game.Scripts.Balance.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
	public class TableSheetNameAttribute : Attribute
	{
		public string TableName { get; }

		public TableSheetNameAttribute(string tableName) => TableName = tableName;
	}
}