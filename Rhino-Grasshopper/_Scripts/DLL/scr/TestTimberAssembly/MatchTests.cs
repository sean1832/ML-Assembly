
namespace TestTimberAssembly
{
    public class MatchTests
    {
        private TimberAssembly.Match _sut;
        private List<Agent> _targetAgents;
        private List<Agent> _salvageAgents;

        private List<Agent> _remainTargetAgents;
        private List<Agent> _remainSalvageAgents;

        private Remain _remain;

        [SetUp]
        public void Setup()
        {
            _targetAgents = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(1, 2, 3)},
                new Agent(){Dimension = new Dimension(4, 5, 6)},
                new Agent(){Dimension = new Dimension(7, 8, 9)}
            };

            _salvageAgents = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(3, 2, 1)},
                new Agent(){Dimension = new Dimension(4.01, 5.05, 6)},
                new Agent(){Dimension = new Dimension(7, 8, 9)}
            };

            
            // for second match
            _remainTargetAgents = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(5, 2, 3)},
                new Agent(){Dimension = new Dimension(4, 5, 6)},
            };

            _remainSalvageAgents = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(3, 2, 3)},
                new Agent(){Dimension = new Dimension(2, 2, 3)},
                new Agent(){Dimension = new Dimension(2, 2, 4)},
                new Agent(){Dimension = new Dimension(2, 3, 2)}
            };

            _remain = new Remain() { Targets = _remainTargetAgents, Subjects = _remainSalvageAgents };

            _sut = new TimberAssembly.Match(_targetAgents, _salvageAgents, 0.1);
        }

        [Test]
        public void ExactMatch_ReturnsMatchedAgents_NoSharedDimension()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            // override the default salvage agents
            List<Agent> salvages = new List<Agent>() { _salvageAgents[0] };
            _sut.SalvageAgents = salvages;

            // Act
            List<MatchPair> pairs = _sut.ExactMatch(out dummyRemain);

            // Assert
            Assert.That(pairs.Count == 0); // there should be no matches
        }

        [Test]
        public void ExactMatch_ReturnsMatchedAgents_ShareDimensionWithTolerance()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> salvages = new List<Agent>() { _salvageAgents[1] };
            _sut.SalvageAgents = salvages;

            // Act
            List<MatchPair> pairs = _sut.ExactMatch(out dummyRemain);

            // Assert
            Assert.That(pairs.Count == 1);
            Assert.That(pairs[0].Target == _targetAgents[1]);
            Assert.That(pairs[0].Subjects[0] == _salvageAgents[1]);
        }

        [Test]
        public void ExactMatch_ReturnsMatchedAgents_ShareDimensionWithoutTolerance()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> salvages = new List<Agent>() { _salvageAgents[2] };
            _sut.SalvageAgents = salvages;

            // Act
            List<MatchPair> pairs = _sut.ExactMatch(out dummyRemain);

            // Assert
            Assert.That(pairs.Count == 1);
            Assert.That(pairs[0].Target == _targetAgents[2]);
            Assert.That(pairs[0].Subjects[0] == _salvageAgents[2]);
        }


        [Test]
        public void ExactMatch_RemainedAgents()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> salvages = new List<Agent>() { _salvageAgents[0], _salvageAgents[1] };
            _sut.SalvageAgents = salvages;

            // Act
            List<MatchPair> pairs = _sut.ExactMatch(out dummyRemain);

            // Assert
            Assert.That(dummyRemain.Targets.Count == 2); // two target agents are not matched and remained
            Assert.That(dummyRemain.Targets[0] == _targetAgents[0]);
            Assert.That(dummyRemain.Targets[1] == _targetAgents[2]);

            Assert.That(dummyRemain.Subjects.Count == 1); // one salvage agent is not matched and remained
            Assert.That(dummyRemain.Subjects[0] == _salvageAgents[0]);
        }

        [Test]
        public void SecondMatch_ReturnMatchedNoPairs_NoShareDimension()
        { 
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> targets = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(5, 2, 3)}
            };
            List<Agent> salvages = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(3, 2, 3)}, 
                new Agent(){Dimension = new Dimension(2, 2, 4)}
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            // Act
            List<MatchPair> pairs = _sut.SecondMatch(remain, out dummyRemain);

            // Assert
            Assert.That(pairs.Count == 0); // there should be no matches
        }

        [Test]
        public void SecondMatch_ReturnMatchedOnePairs_1DifferentDimensionMatch()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> targets = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(5, 2, 3)}
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(3, 2, 3)},
                new Agent(){Dimension = new Dimension(2, 2, 3)}
            };
            
            Remain remain =  new Remain(){ Targets = targets, Subjects = salvages };

            // Act
            List<MatchPair> pairs = _sut.SecondMatch(remain, out dummyRemain);

            // Assert
            Assert.That(pairs.Count == 1);
        }

        [Test]
        public void SecondMatch_RemainAgents()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> targets = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(5, 2, 3)},
                new Agent(){Dimension = new Dimension(6, 9, 8)},
                new Agent(){Dimension = new Dimension(7, 8, 9)},
                new Agent(){Dimension = new Dimension(8, 9, 10)}
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(3, 2, 3)},
                new Agent(){Dimension = new Dimension(2, 2, 3)},
                new Agent(){Dimension = new Dimension(5, 5, 4)},
                new Agent(){Dimension = new Dimension(2, 3, 5)},
                new Agent(){Dimension = new Dimension(9, 10, 15)},
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            // Act
            List<MatchPair> pairs = _sut.SecondMatchSlow(remain, out dummyRemain);

            // Assert
            Assert.That(dummyRemain.Subjects.Count == 2);
            Assert.That(dummyRemain.Targets.Count == 1);
        }

        //[Test]
        //public void RemainMatch_ShouldReturnEmptyList_WhenPreviousRemainsAreNull()
        //{
        //    // Arrange
        //    Remain previousRemains = null;
        //    var match = new TimberAssembly.Match(_targetAgents, _salvageAgents, 0.01);

        //    // Act
        //    List<MatchPair> result = match.RemainMatch(previousRemains);

        //    // Assert
        //    Assert.Fail();
        //}

        [Test]
        public void RemainMatch_ReturnListOfMatchPairs_WhenPreviousRemainsAreNotNull()
        {
            // Arrange
            var match = new TimberAssembly.Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<MatchPair> result = match.RemainMatch(_remain);

            // Assert
            Assert.IsInstanceOf<List<MatchPair>>(result);
        }

        [Test]
        public void RemainMatch_ReturnMatchPair_WhenSubjectEqualsTarget()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(5, 5, 5)},
                new Agent(){Dimension = new Dimension(7, 9, 4)}
            };
            
            List<Agent> salvages = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(4, 4, 4)},
                new Agent(){Dimension = new Dimension(6, 8, 2)}
            };
            Remain previousRemains = new Remain() { Targets = targets, Subjects = salvages };
            var match = new TimberAssembly.Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<MatchPair> result = match.RemainMatch(previousRemains);

            // Assert
            Assert.IsNotNull(result[0].OffcutsAgent);
            Assert.AreEqual(1, result[0].OffcutsAgent.Dimension.Length);
            Assert.AreEqual(1, result[0].OffcutsAgent.Dimension.Width);
            Assert.AreEqual(1, result[0].OffcutsAgent.Dimension.Height);

            Assert.AreEqual(1, result[1].OffcutsAgent.Dimension.Length);
            Assert.AreEqual(1, result[1].OffcutsAgent.Dimension.Width);
            Assert.AreEqual(2, result[1].OffcutsAgent.Dimension.Height);
        }

        [Test]
        public void RemainMatch_ReturnMatchPair_WhenSubjectsIsMoreThanTarget()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(5, 5, 5)},
                new Agent(){Dimension = new Dimension(7, 9, 4)}
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(){Dimension = new Dimension(4, 4, 4)},
                new Agent(){Dimension = new Dimension(6, 8, 2)},
                new Agent(){Dimension = new Dimension(10, 4, 7)}
            };
            Remain previousRemains = new Remain() { Targets = targets, Subjects = salvages };


            var match = new TimberAssembly.Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<MatchPair> result = match.RemainMatch(previousRemains);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].OffcutsAgent.Dimension.Length);
            Assert.AreEqual(1, result[0].OffcutsAgent.Dimension.Width);
            Assert.AreEqual(1, result[0].OffcutsAgent.Dimension.Height);

            Assert.AreEqual(1, result[1].OffcutsAgent.Dimension.Length);
            Assert.AreEqual(1, result[1].OffcutsAgent.Dimension.Width);
            Assert.AreEqual(2, result[1].OffcutsAgent.Dimension.Height);
        }
    }
}
