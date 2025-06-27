using System;
using task02;
using System.Collections.Generic;
using System.Linq;
using Xunit;
namespace task02tests
{
    
    public class StudentServiceTests
    {
        private readonly List<Student> _testStudents;
        private readonly StudentService _service;
        public StudentServiceTests()
        {
            _testStudents = new List<Student>
            {
                new Student { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
                new Student { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
                new Student { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } }
            };
            
            _service = new StudentService(_testStudents);
        }

        [Fact]
        public void GetStudentsByFaculty_ShouldReturnCorrectStudents()
        {
            var result = _service.GetStudentsByFaculty("ФИТ").ToList();
            Assert.Equal(2, result.Count);
            Assert.True(result.All(s => s.Faculty == "ФИТ"));
        }

        [Fact]
        public void GetStudentsWithMinAverageGrade_ShouldFilterCorrectly()
        {
            var result = _service.GetStudentsWithMinAverageGrade(4.5).ToList();
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Иван");
            Assert.Contains(result, s => s.Name == "Петр");
        }

        [Fact]
        public void GetStudentsOrderedByName_ShouldSortAlphabetically()
        {
            var result = _service.GetStudentsOrderedByName().ToList();
            Assert.Equal("Анна", result[0].Name);
            Assert.Equal("Иван", result[1].Name);
            Assert.Equal("Петр", result[2].Name);
        }

        [Fact]
        public void GroupStudentsByFaculty_ShouldCreateCorrectGroups()
        {
            var result = _service.GroupStudentsByFaculty();
            Assert.Equal(2, result.Count);
            Assert.Equal(2, result["ФИТ"].Count());
            Assert.Equal(1, result["Экономика"].Count());
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_ShouldReturnCorrectFaculty()
        {
            var result = _service.GetFacultyWithHighestAverageGrade();
            Assert.Equal("Экономика", result); 
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_WithEmptyList_ShouldReturnNull()
        {
            var emptyService = new StudentService(new List<Student>());
            var result = emptyService.GetFacultyWithHighestAverageGrade();
            Assert.Null(result);
        }
    }
}

