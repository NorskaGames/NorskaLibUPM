using System;

namespace NorskaLib.Spreadsheets
{
	public class PageNameAttribute : Attribute
	{
		public readonly string name;

		public PageNameAttribute(string name)
		{
			this.name = name;
		}
	}
}
