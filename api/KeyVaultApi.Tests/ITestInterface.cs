/*
 *This interface is solely for the purpose of testing the frameworks Nsubstitute and FluentAssertions,
 * it has no further use and can be deleted once we have data to test with.
 *
 * A test for the testing.
 */


using System;

namespace KeyvaultApi.Testing
{
    
    public interface ITestInterface
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event EventHandler PoweringUp;
    }
}