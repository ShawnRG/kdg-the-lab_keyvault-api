using System;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions
{
    [SetUpFixture]
    public class TestsSetupClass
    {
        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            Environment.SetEnvironmentVariable("keyvaultHostname", "INSERT HOSTNAME");
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "CHANGE THIS");
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", "CHANGE THIS");
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "CHANGE THIS");
        }
    }
}
