using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/student")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    [HttpGet("notes")]
    public IActionResult GetNotes()
    {
        return Ok("Acces doar pentru STUDENT");
    }
}
