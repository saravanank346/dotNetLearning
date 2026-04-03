using System;
namespace dotNetAPi.Models.RBAC
{
	public class RoleActionFeatureMapping
	{
		public int Id { get; set; }
		public int RoleId { get; set; }
		public int ActionId { get; set;}
		public int FeatureId { get; set; }

		public Role ?Role { get; set; }
		public Action ?Action { get; set; }
		public Feature? Feature { get; set; }
	}
}

