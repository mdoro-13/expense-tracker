using Api.Controllers;
using Api.Infrastructure.Data;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UnitTests.Utils;

internal static class ControllerTestUtils
{
    internal static T InitializeController<T>(DataContext dbContext, ClaimsPrincipal user, IExpenseManager expenseManager = null) where T : BaseApiController
    {
        var controller = Activator.CreateInstance(typeof(T), new object[] {dbContext, expenseManager}) as T;
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
