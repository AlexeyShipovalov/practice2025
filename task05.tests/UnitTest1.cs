using Xunit;
using System;
namespace task05.tests
{
    public class TestClass
    {
        public int PublicField;
        private string _privateField;
        public int Property { get; set; }

        public void Method() { }
    }

    [Serializable]
    public class AttributedClass { }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
        }
        [Fact]
        public void GetProperties_ReturnsPropertyNames()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties();

            Assert.Contains("Property", properties);
        }
        [Fact]
        public void GetProperties_DoesNotIncludeFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties();

            Assert.DoesNotContain("PublicField", properties);
            Assert.DoesNotContain("_privateField", properties);
        }
        [Fact]
        public void HasAttribute_ReturnsTrueForClassWithAttribute()
        {
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));
            var result = analyzer.HasAttribute<SerializableAttribute>();

            Assert.True(result);
        }
        [Fact]
        public void HasAttribute_ReturnsFalseForClassWithoutAttribute()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var result = analyzer.HasAttribute<SerializableAttribute>();

            Assert.False(result);
        }
        [Fact]
        public void HasAttribute_ReturnsFalseForNonExistingAttribute()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var result = analyzer.HasAttribute<ObsoleteAttribute>();
            Assert.False(result);
        }
    }
}
