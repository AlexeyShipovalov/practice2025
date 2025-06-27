using System;
using System.Collections.Generic;
using System.Linq;
namespace task02
{
    public class Student
    {
        public string Name { get; set; }
        public string Faculty { get; set; }
        public List<int> Grades { get; set; }
    }
    public class StudentService
    {
        private readonly List<Student> students;

        public StudentService(List<Student> studentList)
        {
            if (studentList == null)
            {
                students = new List<Student>();
            }
            else
            {
                students = studentList;
            }
        }

        public List<Student> GetStudentsByFaculty(string faculty)
        {
            var result = new List<Student>();
            foreach (var student in students)
            {
                if (string.Equals(student.Faculty, faculty, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(student);
                }
            }
            return result;
        }

        public List<Student> GetStudentsWithMinAverageGrade(double minGrade)
        {
            var result = new List<Student>();
            foreach (var student in students)
            {
                if (student.Grades.Count > 0)
                {
                    double sum = 0;
                    foreach (var grade in student.Grades)
                    {
                        sum += grade;
                    }
                    double average = sum / student.Grades.Count;
                    if (average >= minGrade)
                    {
                        result.Add(student);
                    }
                }
            }
            return result;
        }

        public List<Student> GetStudentsOrderedByName()
        {
            var sortedStudents = new List<Student>(students);
            sortedStudents.Sort(delegate(Student x, Student y) {
                return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            });
            return sortedStudents;
        }

        public Dictionary<string, List<Student>> GroupStudentsByFaculty()
        {
            var groups = new Dictionary<string, List<Student>>();
            foreach (var student in students)
            {
                if (!groups.ContainsKey(student.Faculty))
                {
                    groups[student.Faculty] = new List<Student>();
                }
                groups[student.Faculty].Add(student);
            }
            return groups;
        }

        public string GetFacultyWithHighestAverageGrade()
        {
            var facultyGroups = new Dictionary<string, List<double>>();
            
            foreach (var student in students)
            {
                if (!facultyGroups.ContainsKey(student.Faculty))
                {
                    facultyGroups[student.Faculty] = new List<double>();
                }
                
                if (student.Grades.Count > 0)
                {
                    double sum = 0;
                    foreach (var grade in student.Grades)
                    {
                        sum += grade;
                    }
                    facultyGroups[student.Faculty].Add(sum / student.Grades.Count);
                }
            }

            string bestFaculty = null;
            double highestAverage = 0;

            foreach (var group in facultyGroups)
            {
                double facultyAverage = group.Value.Count > 0 ? group.Value.Average() : 0;
                if (bestFaculty == null || facultyAverage > highestAverage)
                {
                    highestAverage = facultyAverage;
                    bestFaculty = group.Key;
                }
            }

            return bestFaculty;
        }
    }
}
