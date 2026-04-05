using System;
using dotNetAPi.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Action = dotNetAPi.Enum.Action;

namespace dotNetAPi.Services
{
	public class PermissionFilter:IAsyncAuthorizationFilter 
	{
		private readonly PermissionService _permissionService;
		private readonly Feature _feature;
		private readonly Action _action;

		public PermissionFilter(PermissionService permissionService, Feature feature , Action action)
		{
			_permissionService = permissionService;

			_feature = feature;
			_action = action;

        }

		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user =context.HttpContext.User;

			if(user?.Identity==null || !user.Identity.IsAuthenticated)
			{
				context.Result = new UnauthorizedObjectResult(new
				{
					message = "UnAuthorized"
				});

				return;
			}

			var claimValue = user.FindFirst("roleId")?.Value;

			if(!int.TryParse(claimValue , out int roleId))
			{
				context.Result = new UnauthorizedObjectResult(new
				{
					message = "Invalid Token"
				});

				return;
			}


			var hasPermission = await _permissionService.hasPermission(roleId, _feature, _action);

			if (!hasPermission)
			{
				context.Result = new UnauthorizedObjectResult(new
				{
					message = "Forbidden-Resource"
				})
				{
					StatusCode = 403
				};

				return;
			}
        }
    }
}

