using Catalog.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Catalog.Repository;

[ApiController]
[Route("api/class")]
public class ClassController : ControllerBase
{
    private readonly IClassRepository _classService;

    public ClassController(IClassRepository classService)
    {
        _classService = classService;
    }

    [HttpGet("show-classes")]
    public IActionResult GetClasses()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { Message = "Invalid token." });

        int teacherId = int.Parse(userIdClaim.Value);
        var classes = _classService.GetClassesByTeacherId(teacherId);
        return Ok(classes);
    }

    [HttpPost("add-student")]
    public IActionResult AddStudentToClass([FromBody] AddStudentRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.ClassName) || string.IsNullOrWhiteSpace(request.StudentName))
        {
            return BadRequest(new { Message = "Invalid request data." });
        }

        string errorMessage;
        bool success = _classService.AddStudentToClass(request.ClassName, request.StudentName, out errorMessage);

        if (!success)
        {
            return BadRequest(new { Message = errorMessage });
        }

        return Ok(new { Message = "Student added successfully." });
    }

    public class AddStudentRequest
    {
        public string ClassName { get; set; }
        public string StudentName { get; set; }
    }
}
