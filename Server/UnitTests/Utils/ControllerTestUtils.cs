using Api.Controllers;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UnitTests.Utils;

internal static class ControllerTestUtils
{
    internal static T InitializeController<T>(DataContext dbContext, ClaimsPrincipal user) where T : BaseApiController
    {
        var controller = Activator.CreateInstance(typeof(T), new object[] {dbContext}) as T;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };
        return controller;
    }
}
