using System;
using dotNetAPi.Data;
using dotNetAPi.Enum;
using Microsoft.EntityFrameworkCore;
using Action = dotNetAPi.Enum.Action;

namespace dotNetAPi.Services
{
	public class PermissionService
	{
		private readonly AppDbContext _appDbContext;
		public PermissionService(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public async Task<bool> hasPermission(int roleId , Feature feature , Action action)
		{
			var hasPermission = await (
				from usrm in _appDbContext.UserRoleMappings
				join rafm in _appDbContext.RoleActionFeatureMappings
				on usrm.RoleId equals rafm.RoleId

				join act in _appDbContext.Action
				on rafm.ActionId equals act.Id

				join f in _appDbContext.Features
				on rafm.FeatureId equals f.Id

				where usrm.RoleId == roleId
				&& f.ActionName == feature.ToString()
				&& act.ActionName == action.ToString()

				select usrm
				).AnyAsync();//AnyAsync is used to check if at least one record exists in the database. It returns a boolean

            return hasPermission;
		}
	}
}

