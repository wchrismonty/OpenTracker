using NSubstitute;
using NSubstitute.ReturnsExtensions;
using OpenTracker.Models.Items;
using OpenTracker.Models.PrizePlacements;
using OpenTracker.Models.UndoRedo;
using Xunit;

namespace OpenTracker.UnitTests.Models.UndoRedo
{
    public class ChangePrizeTests
    {
        private readonly IPrizePlacement _prizePlacement = Substitute.For<IPrizePlacement>();
        private readonly ChangePrize _sut;
        private readonly IItem _previousValue = Substitute.For<IItem>();

        public ChangePrizeTests()
        {
            _sut = new ChangePrize(_prizePlacement);
        }

        [Fact]
        public void CanExecute_ShouldCallCanCycle()
        {
            _ = _sut.CanExecute();

            _prizePlacement.Received().CanCycle();
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void CanExecute_ShouldReturnTrue_WhenCanCycleReturnsTrue(bool expected, bool canCycle)
        {
            _prizePlacement.CanCycle().Returns(canCycle);
            
            Assert.Equal(expected, _sut.CanExecute());
        }

        [Fact]
        public void ExecuteDo_ShouldCallCycle()
        {
            _sut.ExecuteDo();
            
            _prizePlacement.Received().Cycle();
        }

        [Fact]
        public void ExecuteUndo_ShouldSetPrizeToPreviousValue()
        {
            _prizePlacement.Prize.Returns(_previousValue);
            _sut.ExecuteDo();
            _prizePlacement.Prize.ReturnsNull();
            _sut.ExecuteUndo();
            
            Assert.Equal(_previousValue, _prizePlacement.Prize);
        }
    }
}