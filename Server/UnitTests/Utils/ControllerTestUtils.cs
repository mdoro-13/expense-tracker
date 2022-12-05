using Api.Controllers;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UnitTests.Utils;

internal static class ControllerTestUtils
{
    internal static ExpenseController InitializeController(DataContext dbContext, ClaimsPrincipal user)
    {
        var controller = new ExpenseController(dbContext);
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
