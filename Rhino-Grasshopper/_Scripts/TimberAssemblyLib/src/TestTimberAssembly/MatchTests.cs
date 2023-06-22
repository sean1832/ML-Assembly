
using TimberAssembly.Entities;
using TimberAssembly.Operation;

namespace TestTimberAssembly
{
    public class MatchTests
    {
        private Match _sut;
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
                new Agent(dimension : new Vector3D(1, 2, 3)),
                new Agent(dimension : new Vector3D(4, 5, 6)),
                new Agent(dimension : new Vector3D(7, 8, 9))
            };

            _salvageAgents = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(3, 2, 1)),
                new Agent(dimension : new Vector3D(4.01, 5.05, 6)),
                new Agent(dimension : new Vector3D(7, 8, 9))
            };

            
            // for second match
            _remainTargetAgents = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(5, 2, 3)),
                new Agent(dimension : new Vector3D(4, 5, 6)),
            };

            _remainSalvageAgents = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(3, 2, 3)),
                new Agent(dimension : new Vector3D(2, 2, 3)),
                new Agent(dimension : new Vector3D(2, 2, 4)),
                new Agent(dimension : new Vector3D(2, 3, 2))
            };

            _remain = new Remain() { Targets = _remainTargetAgents, Subjects = _remainSalvageAgents };

            _sut = new Match(_targetAgents, _salvageAgents, 0.1);
        }

        #region ExactMatch

        [Test]
        public void ExactMatch_ReturnsMatchedAgents_NoSharedDimension()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            // override the default salvage agents
            List<Agent> salvages = new List<Agent>() { _salvageAgents[0] };
            _sut.SubjectAgents = salvages;

            // Act
            List<Pair> pairs = _sut.ExactMatch(ref dummyRemain);

            // Assert
            Assert.That(pairs.Count == 0); // there should be no matches
        }

        [Test]
        public void ExactMatch_ReturnsMatchedAgents_ShareDimensionWithTolerance()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> salvages = new List<Agent>() { _salvageAgents[1] };
            _sut.SubjectAgents = salvages;

            // Act
            List<Pair> pairs = _sut.ExactMatch(ref dummyRemain);

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
            _sut.SubjectAgents = salvages;

            // Act
            List<Pair> pairs = _sut.ExactMatch(ref dummyRemain);

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
            _sut.SubjectAgents = salvages;

            // Act
            List<Pair> pairs = _sut.ExactMatch(ref dummyRemain);

            // Assert
            Assert.That(dummyRemain.Targets.Count == 2); // two target agents are not matched and remained
            Assert.That(dummyRemain.Targets[0] == _targetAgents[0]);
            Assert.That(dummyRemain.Targets[1] == _targetAgents[2]);

            Assert.That(dummyRemain.Subjects.Count == 1); // one salvage agent is not matched and remained
            Assert.That(dummyRemain.Subjects[0] == _salvageAgents[0]);
        }

        #endregion

        #region SecondMatch

        [Test]
        public void DoubleMatch_ReturnMatchedNoPairs_NoShareDimension()
        { 
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(5, 2, 3))
            };
            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension:new Vector3D(3, 2, 3)), 
                new Agent(dimension : new Vector3D(2, 2, 4))
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            // Act
            List<Pair> pairs = _sut.DoubleMatch(ref remain);

            // Assert
            Assert.That(pairs.Count == 0); // there should be no matches
        }

        [Test]
        public void DoubleMatch_ReturnMatchedOnePairs_1DifferentDimensionMatch()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension:new Vector3D(5, 2, 3))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(3, 2, 3)),
                new Agent(dimension:new Vector3D(2, 2, 3))
            };
            
            Remain remain =  new Remain(){ Targets = targets, Subjects = salvages };

            // Act
            List<Pair> pairs = _sut.DoubleMatch(ref remain);

            // Assert
            Assert.That(pairs.Count == 1);
        }

        [Test]
        public void DoubleMatch_ReturnMatchedOnePairs_WhenOneOfSubjectEqualsTarget()
        {
            // Arrange
            Remain dummyRemain = new Remain();
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension:new Vector3D(5, 2, 3))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(5, 2, 3)),
                new Agent(dimension:new Vector3D(5, 2, 3))
            };

            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            // Act
            List<Pair> pairs = _sut.DoubleMatch(ref remain);

            // Assert
            Assert.AreEqual(0, pairs.Count);
        }

        [Test]
        public void DoubleMatch_RemainAgents()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(7, 8, 9)),
                new Agent(dimension : new Vector3D(6, 9, 8)),
                new Agent(dimension : new Vector3D(5, 2, 3)),
                new Agent(dimension : new Vector3D(8, 9, 10))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(5, 8, 9)),
                new Agent(dimension:new Vector3D(2, 8, 9)),
                new Agent(dimension : new Vector3D(3, 2, 3)),
                new Agent(dimension : new Vector3D(2, 2, 3)),
                new Agent(dimension : new Vector3D(9, 10, 15))
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            // Act
            List<Pair> pairs = _sut.DoubleMatch(ref remain);
            
            // Assert
            Assert.That(remain.Subjects.Count == 1);
            Assert.That(remain.Targets.Count == 2);
            Assert.That(remain.Subjects[0] == salvages[4]);
            Assert.That(remain.Targets[0] == targets[1]);
            Assert.That(remain.Targets[1] == targets[3]);
        }

        #endregion

        #region UniMatch

        [Test]
        public void UniMatch_FourSubjectOneRemain_TargetLargerThanSubject()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(6, 7, 10))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(2, 3, 4)),
                new Agent(dimension : new Vector3D(7, 4, 10)),
                new Agent(dimension : new Vector3D(2, 4, 10)),
                new Agent(dimension : new Vector3D(2, 3, 6)),
                new Agent(dimension : new Vector3D(7, 8, 9)) // this will be remain
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.UniMatch(ref remain);

            var test = result;

            // Assert
            Assert.AreEqual(1, remain.Subjects.Count);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(4, result[0].Subjects.Count);
        }

        [Test]
        public void UniMatch_ThreeSubjectThreeRemain_TwoSubjectFit()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(7, 5, 4))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(2.5, 7, 4)),
                new Agent(dimension : new Vector3D(2.5, 7, 2)),
                new Agent(dimension : new Vector3D(2.5, 7, 2)),
                new Agent(dimension : new Vector3D(2, 3, 6)), // this will be remain
                new Agent(dimension : new Vector3D(7, 8, 9)) // this will be remain
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.UniMatch(ref remain);

            var test = result;

            // Assert
            Assert.AreEqual(2, remain.Subjects.Count);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(3, result[0].Subjects.Count);
        }

        [Test]
        public void UniMatch_TwoSubjectThreeRemain_TwoSubjectFit()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(7, 5, 4))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(2, 5, 4)),
                new Agent(dimension : new Vector3D(5, 5, 4)),
                new Agent(dimension : new Vector3D(2, 4, 10)),
                new Agent(dimension : new Vector3D(2, 3, 6)),
                new Agent(dimension : new Vector3D(7, 8, 9)) // this will be remain
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.UniMatch(ref remain);

            var test = result;

            // Assert
            Assert.AreEqual(3, remain.Subjects.Count);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].Subjects.Count);
        }
        #endregion

        #region QuadrupleMatch



        #endregion

        #region CutToTarget

        [Test]
        public void CutToTarget_OffcutsRemain_WhenSubjectIsLargerThanTargets()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(2, 2, 4))
            };

            List<Agent> subjects = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(10, 4, 8))
            };
            Remain remain = new Remain() { Targets = targets, Subjects = subjects };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.CutToTarget(ref remain);

            // Assert
            Assert.That(result.Count == 1);
            Assert.AreEqual(3, remain.Subjects.Count);

            Assert.AreEqual(8, remain.Subjects[0].Dimension.X);
            Assert.AreEqual(4, remain.Subjects[0].Dimension.Y);
            Assert.AreEqual(8, remain.Subjects[0].Dimension.Z);

            Assert.AreEqual(2, remain.Subjects[1].Dimension.X);
            Assert.AreEqual(2, remain.Subjects[1].Dimension.Y);
            Assert.AreEqual(8, remain.Subjects[1].Dimension.Z);

            Assert.AreEqual(2, remain.Subjects[2].Dimension.X);
            Assert.AreEqual(2, remain.Subjects[2].Dimension.Y);
            Assert.AreEqual(4, remain.Subjects[2].Dimension.Z);
        }

        [Test]
        public void CutToTarget_ReturnMatchedPairs_MultipleSubjectsAndTargets()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(2, 2, 6)),
                new Agent(dimension : new Vector3D(3, 5, 2))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(4, 4, 8)),
                new Agent(dimension : new Vector3D(2, 5, 6)),
                new Agent(dimension : new Vector3D(7, 4, 7))
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };
            List<double> expectedTargets1 = new List<double>() { 2, 2, 6 };
            List<double> expectedTargets2 = new List<double>() { 3, 5, 2 };

            List<double> expectedSubjects1 = new List<double>() { 2, 2, 6 };
            List<double> expectedSubjects2 = new List<double>() { 3, 5, 2 };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.CutToTarget(ref remain);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(expectedTargets1, result[0].Target.Dimension.ToList());
            Assert.AreEqual(expectedSubjects1, result[0].Subjects[0].Dimension.ToList());

            Assert.AreEqual(expectedTargets2, result[1].Target.Dimension.ToList());
            Assert.AreEqual(expectedSubjects2, result[1].Subjects[0].Dimension.ToList());
        }

        [Test]
        public void CutToTarget_OffcutsRemain_MultipleSubjectsAndTargets()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(2, 2, 6)),
                new Agent(dimension : new Vector3D(3, 5, 2))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(4, 4, 8)),
                new Agent(dimension : new Vector3D(2, 5, 6)),
                new Agent(dimension : new Vector3D(7, 4, 7))
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.CutToTarget(ref remain);

            var test = result;

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(4, remain.Subjects.Count);
        }

        [Test]
        public void CutToTarget_ReturnEmpty_WhenSubjectIsSmallerThanTargets()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(8, 8, 9))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(4, 4, 7))
            };
            Remain remain = new Remain() { Targets = targets, Subjects = salvages };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.CutToTarget(ref remain);

            // Assert
            Assert.IsEmpty(result);
        }


        #endregion

        #region ExtendToTarget

        [Test]
        public void ExtendToTarget_ReturnEmpty_WhenSubjectsLargerThanTargets()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(3, 7, 8))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(4, 4, 4))
            };
            Remain previousRemains = new Remain() { Targets = targets, Subjects = salvages };
            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.ExtendToTarget(ref previousRemains);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void ExtendToTarget_ReturnMatchPair_WhenSubjectsIsMoreThanTarget()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(7, 9, 4))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(10, 4, 7))
            };
            Remain previousRemains = new Remain() { Targets = targets, Subjects = salvages };
            List<double> expected1 = new List<double>() { 7, 1, 4 };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.ExtendToTarget(ref previousRemains);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expected1, result[0].Subjects[1].Dimension.ToList());
        }

        [Test]
        public void ExtendToTarget_ReturnMatchPair_WhenSubjectsIsLessThanTarget()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(5, 5, 5)),
                new Agent(dimension : new Vector3D(7, 9, 4)),
                new Agent(dimension : new Vector3D(10, 4, 7)),
                new Agent(dimension : new Vector3D(8, 15, 3))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(4, 4, 7)),
                new Agent(dimension : new Vector3D(7, 9, 2)),
                new Agent(dimension : new Vector3D(4, 5, 5))
            };
            Remain previousRemains = new Remain() { Targets = targets, Subjects = salvages };
            List<double> expected1 = new List<double>() { 5, 5, 1 };
            List<double> expected2 = new List<double>() { 7, 9, 2 };

            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.ExtendToTarget(ref previousRemains);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(expected1, result[0].Subjects[1].Dimension.ToList());
            Assert.AreEqual(expected2, result[1].Subjects[1].Dimension.ToList());
        }

        [Test]
        public void ExtendToTarget_ReturnListOfMatchPairs_WhenPreviousRemainsAreNotNull()
        {
            // Arrange
            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            // Act
            List<Pair> result = match.ExtendToTarget(ref _remain);

            // Assert
            Assert.IsInstanceOf<List<Pair>>(result);
        }

        [Test]
        public void ExtendToTarget_ReturnOneMatch_WhenSubjectOneDimensionDifferent()
        {
            // Arrange
            List<Agent> targets = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(3, 7, 8))
            };

            List<Agent> salvages = new List<Agent>()
            {
                new Agent(dimension : new Vector3D(3, 5, 8))
            };
            Remain previousRemains = new Remain() { Targets = targets, Subjects = salvages };
            var match = new Match(_targetAgents, _salvageAgents, 0.01);

            List<double> expected = new List<double>() { 3, 2, 8 };

            // Act
            List<Pair> result = match.ExtendToTarget(ref previousRemains);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expected, result[0].Subjects[1].Dimension.ToList());
        }
        #endregion
    }
}
