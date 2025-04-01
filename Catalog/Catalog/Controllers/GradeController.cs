using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly GradeService _gradeService;

        public GradeController(GradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpPost("POST")]
        public async Task<IActionResult> AddGrade([FromBody] GradeRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            try
            {
                var grade = await _gradeService.AddGradeAsync(
                    request.TeacherId, request.StudentId, request.ClassId, request.Value, request.Date);

                return Ok(new { Message = "Grade added successfully", Grade = grade });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("update/{gradeId}")]
        public async Task<IActionResult> UpdateGrade(int gradeId, [FromBody] UpdateGradeRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            try
            {
                var updatedGrade = await _gradeService.UpdateGradeAsync(gradeId, request.Value, request.Date);

                if (updatedGrade == null)
                    return NotFound("Grade not found");

                return Ok(new { Message = "Grade updated successfully", Grade = updatedGrade });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetGradesByTeacher(int teacherId)
        {
            try
            {
                var grades = await _gradeService.GetGradesByTeacherAsync(teacherId);

                if (grades == null || grades.Count == 0)
                    return NotFound("No grades found for this teacher");

                return Ok(grades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{gradeId}")]
        public async Task<IActionResult> DeleteGrade(int gradeId)
        {
            try
            {
                var success = await _gradeService.DeleteGradeAsync(gradeId);

                if (!success)
                    return NotFound("Grade not found");

                return Ok(new { Message = "Grade deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }

    public class GradeRequest
    {
        public int TeacherId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
    }

    public class UpdateGradeRequest
    {
        public double Value { get; set; }
        public DateTime Date { get; set; }
    }
}
