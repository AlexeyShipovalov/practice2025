using Xunit;
using task04;

namespace task04Tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }
        [Fact]
        public void Cruiser_Fire_ShouldDeal100Damage()
        {
            var cruiser = new Cruiser();
            Assert.Equal(100, cruiser.FirePower);
        }
        [Fact]
        public void Fighter_Fire_ShouldDeal50Damage()
        {
            var fighter = new Fighter();
            Assert.Equal(50, fighter.FirePower);
        }
        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            var fighter = new Fighter();
            Assert.Equal(100, fighter.Speed);
            Assert.Equal(50, fighter.FirePower);
        }
        [Fact]
        public void Cruiser_FirePower_ShouldBeGreaterThanFighter()
        {
            var cruiser = new Cruiser();
            var fighter = new Fighter();
            Assert.True(cruiser.FirePower > fighter.FirePower);
        }
    }
}
