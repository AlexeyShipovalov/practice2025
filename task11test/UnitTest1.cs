using task11;
namespace task11test
{
    public class CalculatorTests
    {
        [Fact]
        public void TestCalculatorOperations()
        {
            dynamic calculator = DynamicClassCreator.CreateCalculator();
             if (calculator == null)
            {
                throw new Exception("Не удалось создать экземпляр Calculator");
            }
            Assert.Equal(5, calculator.Add(2, 3));
            Assert.Equal(-1, calculator.Minus(2, 3));
            Assert.Equal(6, calculator.Mul(2, 3));
            Assert.Equal(2, calculator.Div(6, 3));
            Assert.Throws<DivideByZeroException>(() => calculator.Div(6, 0));
        }
    }
}
