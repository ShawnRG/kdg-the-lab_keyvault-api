/*
 *This class is solely for the purpose of testing the frameworks Nsubstitute and FluentAssertions,
 * it has no further use and can be deleted once we have data to test with.
 *
 * A test for the testing.
 */

using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace KeyvaultApi.Testing
{
    
    [TestFixture]  
    public class TestingTest
    {
        private ITestInterface _testInterface = Substitute.For<ITestInterface>();
        
        [Test]
        public void TestAdd()
        {
            _testInterface.Add(1, 2).Returns(3);
            _testInterface.Add(1, 2).Should().Be(3);

            

        }

        [Test]
        public void TestMode()
        {
            _testInterface.Mode.Returns("DEC");
            _testInterface.Mode.Should().Be("DEC");
        }

    }
}