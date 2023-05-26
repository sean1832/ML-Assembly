using Moq;
using Newtonsoft.Json;
using TimberAssembly.Entities;

namespace TestTimberAssembly
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void DeserializeToAgents_ReturnsEmptyList_WhenGivenEmptyList()
        {
            // Arrange
            var jsonList = new List<string>();
            
            // Act
            var actual = Parser.DeserializeToAgents(jsonList);
            
            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void DeserializeToAgents_ReturnsCorrectNumberOfAgents()
        {
            // Arrange
            var jsonList = new List<string>{
                "{\"Name\":\"T01\",\"Dimension\":{\"Length\":1,\"Width\":2,\"Height\":3}}",
                "{\"Name\":\"T02\",\"Dimension\":{\"Length\":4,\"Width\":5,\"Height\":6}}"
            };
            
            // Act
            var actual = Parser.DeserializeToAgents(jsonList);
            
            // Assert
            Assert.That(actual, Has.Exactly(2).Items);
        }

        [Test]
        public void DeserializeToAgents_CallsDeserializeObject_ForEachJsonString()
        {
            // Arrange
            var jsonList = new List<string> {
                "{\"Name\":\"T01\",\"Dimension\":{\"Length\":1,\"Width\":2,\"Height\":3}}",
                "{\"Name\":\"T02\",\"Dimension\":{\"Length\":4,\"Width\":5,\"Height\":6}}"
            };

            // Act
            var actual = jsonList.Select(JsonConvert.DeserializeObject<Agent>).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual.Capacity, Is.EqualTo(2));
                Assert.That(actual[0].Name == "T01");
                Assert.That(actual[1].Name == "T02");

                Assert.That(actual[0].Dimension.Length, Is.EqualTo(1));
                Assert.That(actual[0].Dimension.Width, Is.EqualTo(2));
                Assert.That(actual[0].Dimension.Height, Is.EqualTo(3));

                Assert.That(actual[1].Dimension.Length, Is.EqualTo(4));
                Assert.That(actual[1].Dimension.Width, Is.EqualTo(5));
                Assert.That(actual[1].Dimension.Height, Is.EqualTo(6));
            });
        }

        [Test]
        public void SerializeAgentPairs_JsonString()
        {
            // Arrange
            List<Pair> pairs = new List<Pair>()
            {
                new Pair()
                {
                    Target = new Agent()
                    {
                        Name = "T01",
                        Dimension = new Dimension(1, 2, 3)
                    },
                    Subjects = new List<Agent>()
                    {
                        new Agent()
                        {
                            Name = "S01",
                            Dimension = new Dimension(1, 2, 3)
                        },
                        new Agent()
                        {
                            Name = "S02",
                            Dimension = new Dimension(1, 2, 3)
                        }
                    }
                },  
            };

            // Act
            var jsonString = Parser.SerializeAgentPairs(pairs);

            // Assert
            Assert.That(jsonString.Count == 1);
        }
    }
}
