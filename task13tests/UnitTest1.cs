using System;
using System.Collections.Generic;
using System.IO;
using task13;
namespace task13tests
{
    public class StudentTests
    {
        private readonly Student _testStudent = new()
        {
            FirstName = "Яков",
            LastName = "Погорелкин",
            BirthDate = new DateTime(1999, 1, 17),
            Grades = new List<Subject>
            {
                new Subject { Name = "Русский язык", Grade = 5 },
                new Subject { Name = "Физика", Grade = 4 }
            }
        };
        [Fact]
        public void Serialize_ShouldIncludeAllProperties()
        {
            var json = JsonService.Serialize(_testStudent);
            Assert.Contains(_testStudent.FirstName, json);
            Assert.Contains(_testStudent.LastName, json);
            Assert.Contains("1999-01-17", json);
            Assert.Contains("Русский язык", json);
        }
        [Fact]
        public void Deserialize_ShouldReturnValidObject()
        {
            var json = JsonService.Serialize(_testStudent);
            var deserialized = JsonService.Deserialize(json);
            Assert.Equal(_testStudent.FirstName, deserialized.FirstName);
            Assert.Equal(_testStudent.BirthDate, deserialized.BirthDate);
            Assert.Equal(_testStudent.Grades.Count, deserialized.Grades.Count);
        }
        [Fact]
        public void SaveAndLoad_ShouldWorkCorrectly()
        {
            string testFile = "test_student.json";
            JsonService.SaveToFile(_testStudent, testFile);
            var loaded = JsonService.LoadFromFile(testFile);
            Assert.Equal(_testStudent.FirstName, loaded.FirstName);
            Assert.Equal(_testStudent.Grades[0].Name, loaded.Grades[0].Name);
            File.Delete(testFile);
        }
        [Fact]
        public void Deserialize_InvalidJson_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => 
                JsonService.Deserialize("{неверный json}"));
        }
    }
}
