using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;
using Persistence;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ModelTest
{
    [TestClass]
    public class MotorDuelModelTests
    {
        private MotorDuelModel _model;
        private Mock<IDataAccess> _mock;
        private SaveState _mockedState = new SaveState()
        {
            FieldSize = FieldSize.Size12x12,
            Time = System.TimeSpan.FromSeconds(12),
            PlayerA = new PlayerState()
            {
                Location = new Point(2,3),
                Direction = Direction.Up,
                Trail = new List<Point>() { new Point(1,3), new Point(1,2) }
            },
            PlayerB = new PlayerState()
            {
                Location = new Point(4, 5),
                Direction = Direction.Up,
                Trail = new List<Point>() { new Point(6, 5), new Point(7, 5) }
            }
        };

        [TestInitialize]
        public void Initialize()
        {
            
            _mock = new Mock<IDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(_mockedState));
            _model = new MotorDuelModel(_mock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(PlayerCollidedException),"Didn't collide with side")]
        public void PlayerCollidesWithSide()
        {
            _model.NewGame(FieldSize.Size12x12);
            _model.PlayerB.TurnRight();
            _model.PlayerB.MoveForward(_model);
            _model.PlayerB.MoveForward(_model);
            _model.PlayerB.MoveForward(_model);
        }


        [TestMethod]
        [ExpectedException(typeof(PlayerCollidedException), "Didn't collide with own trail")]
        public void PlayerCollidesWithOwnTrail()
        {
            _model.NewGame(FieldSize.Size36x36);
            _model.PlayerA.MoveForward(_model);
            _model.PlayerA.TurnRight();
            _model.PlayerA.MoveForward(_model);
            _model.PlayerA.TurnRight();
            _model.PlayerA.MoveForward(_model);
            _model.PlayerA.TurnRight();
            _model.PlayerA.MoveForward(_model);
            _model.PlayerA.TurnRight();
        }

        [TestMethod]
        [ExpectedException(typeof(PlayerCollidedException), "Didn't collide with other's trail")]
        public void PlayerCollidesWithOthersTrail()
        {
            _model.NewGame(FieldSize.Size12x12);
            _model.PlayerB.MoveForward(_model);
            _model.PlayerA.TurnLeft();

            while(true)
            {
                _model.PlayerA.MoveForward(_model);
            }
        }

        [TestMethod]
        public void PlayerMovesWell()
        {

            _model.NewGame(FieldSize.Size36x36);
            var pl = _model.PlayerA;

            var originalLocation = pl.Location;

            pl.MoveForward(_model);
            pl.TurnLeft();
            pl.MoveForward(_model);
            pl.MoveForward(_model);
            pl.TurnRight();
            pl.MoveForward(_model);
            pl.MoveForward(_model);
            pl.TurnRight();
            pl.MoveForward(_model);
            pl.MoveForward(_model);

            var supposedResult = new Point(originalLocation.X, originalLocation.Y + 3);

            Assert.AreEqual(supposedResult, pl.Location);
        }

        [TestMethod]
        public void DataLoadsWellFromPersistence()
        {
            _model.OpenFileAsync("alma");
            Assert.AreEqual(_mockedState.FieldSize, _model.FieldSize);
            Assert.AreEqual(_mockedState.PlayerA.Location, _model.PlayerA.Location);
            Assert.AreEqual(_mockedState.PlayerA.Trail, _model.PlayerA.Trail);
            Assert.AreEqual(_mockedState.PlayerA.Direction, _model.PlayerA.Direction);
            Assert.AreEqual(_mockedState.PlayerB.Location, _model.PlayerB.Location);
            Assert.AreEqual(_mockedState.PlayerB.Trail, _model.PlayerB.Trail);
            Assert.AreEqual(_mockedState.PlayerB.Direction, _model.PlayerB.Direction);
        }
    }
}
