using System;
using dotNetAPi.Enum;
using dotNetAPi.Services;
using Microsoft.AspNetCore.Mvc;
using Action = dotNetAPi.Enum.Action;

namespace dotNetAPi.RBAC
{
	public class PermissionAttribute:TypeFilterAttribute
	{

		public PermissionAttribute(Feature feature , Action action):base(typeof(PermissionFilter))
		{
			Arguments = new object[] { feature, action};
		}
	}
}

