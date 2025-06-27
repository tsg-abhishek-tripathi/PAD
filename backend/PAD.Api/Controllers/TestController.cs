
using Microsoft.AspNetCore.Mvc;

namespace PAD.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new { 
            message = "PAD API is working!", 
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        });
    }

    [HttpGet("employees-test")]
    public ActionResult<object> GetEmployeesTest()
    {
        return Ok(new
        {
            message = "Employees endpoint test",
            sampleEmployees = new[]
            {
                new { id = 1, name = "John Doe", email = "john.doe@company.com" },
                new { id = 2, name = "Jane Smith", email = "jane.smith@company.com" }
            }
        });
    }
}
