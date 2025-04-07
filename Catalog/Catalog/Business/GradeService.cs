﻿using Catalog.Dtos;
using Catalog.Models;
using Catalog.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Services
{
    public class GradeService
    {
        private readonly IGradeRepository _gradeRepository;

        public GradeService(IGradeRepository gradeRepository)
        {
            _gradeRepository = gradeRepository;
        }

        public async Task<List<Grade>> GetGradesByTeacherAsync(int teacherId)
        {
            return await _gradeRepository.GetGradesByTeacherAsync(teacherId);
        }

        public async Task<List<Grade>> AddMultipleGradesAsync(int teacherId, int studentId, int classId, List<GradeEntry> grades)
        {
            var newGrades = grades.Select(g => new Grade
            {
                Value = g.Value,
                Date = g.Date,
                Assignments = g.Assignments,
                StudentId = studentId,
                ClassId = classId,
                TeacherId = teacherId
            }).ToList();

            return await _gradeRepository.AddGradesAsync(newGrades);
        }

        public async Task<Grade> AddGradeAsync(int teacherId, int studentId, int classId, double value, DateTime date, string? assignments)
        {
            var grade = new Grade
            {
                Value = value,
                Date = date,
                StudentId = studentId,
                ClassId = classId,
                TeacherId = teacherId,
                Assignments = assignments
            };

            return await _gradeRepository.AddGradeAsync(grade);
        }

        public async Task<Grade?> UpdateGradeAsync(int gradeId, double value, DateTime date, string assignments)
        {
            var grade = await _gradeRepository.GetGradeByIdAsync(gradeId);
            if (grade == null)
                return null;

            grade.Value = value;
            grade.Date = date;
            grade.Assignments = assignments;

            await _gradeRepository.SaveAsync();
            return grade;
        }

        public async Task<bool> DeleteGradeAsync(int gradeId)
        {
            return await _gradeRepository.DeleteGradeAsync(gradeId);
        }
    }
}
