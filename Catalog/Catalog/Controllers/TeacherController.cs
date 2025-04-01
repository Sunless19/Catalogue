using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/teacher")]
[Authorize(Roles = "Teacher")]
public class TeacherController : ControllerBase
{
    [HttpGet("elevi")]
    public IActionResult GetStudents()
    {
        return Ok("Acces doar pentru TEACHER");
    }
}