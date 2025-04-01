using Catalog.AppDBContext;
using Catalog.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Catalog.Repository;

[ApiController]
[Route("api/teacher")]
[Authorize(Roles = "Teacher")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherRepository _teacherService;

    public TeacherController(ITeacherRepository teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet("classes")]
    public IActionResult GetClasses()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { Message = "Invalid token." });

        int teacherId = int.Parse(userIdClaim.Value);
        var classes = _teacherService.GetClassesByTeacherId(teacherId);
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
        bool success = _teacherService.AddStudentToClass(request.ClassName, request.StudentName, out errorMessage);

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