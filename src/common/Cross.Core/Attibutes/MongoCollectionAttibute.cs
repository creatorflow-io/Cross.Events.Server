using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Attibutes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class MongoCollectionAttribute : Attribute
	{
		public MongoCollectionAttribute(string collectionName)
		{
			CollectionName = collectionName;
		}

		public string CollectionName { get; }
	}
}
