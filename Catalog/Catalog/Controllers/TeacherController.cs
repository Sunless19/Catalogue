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
}