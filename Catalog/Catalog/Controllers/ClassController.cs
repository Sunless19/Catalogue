using Catalog.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Catalog.Repository;
using static ClassController;

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
        if (request == null || request.ClassId <= 0 || string.IsNullOrWhiteSpace(request.StudentName))
        {
            return BadRequest(new { Message = "Invalid request data." });
        }

        string errorMessage;
        int studentId = _classService.AddStudentToClass(request.ClassId, request.StudentName, out errorMessage);

        if (studentId == -1)
        {
            return BadRequest(new { Message = errorMessage });
        }

        return Ok(new { Message = "Student added successfully.", StudentId = studentId });
    }
    [HttpDelete("delete-student")]
    public IActionResult DeleteStudent([FromBody] DeleteStudentRequest request)
    {
        if (request.ClassId <= 0 || request.StudentId <= 0)
        {
            return BadRequest(new { Message = "Invalid request data." });
        }

        bool success = _classService.RemoveStudentFromClass(request.ClassId, request.StudentId, out string errorMessage);

        if (!success)
        {
            return BadRequest(new { Message = errorMessage });
        }

        return Ok(new { Message = "Student removed successfully." });
    }

    public class AddStudentRequest
    {
        public int ClassId { get; set; }
        public string StudentName { get; set; }
    }
    public class DeleteStudentRequest
    {
        public int ClassId { get; set; }
        public int StudentId { get; set; }
    }
}
