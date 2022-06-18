using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public static class MockAwsServiceFactory
    {
        /// <summary>Creates a mock AWS service.</summary>
        public static Natural.Aws.MockAwsService CreateMockAwsService()
        {
            Natural.Aws.MockAwsService mockAwsService = new Natural.Aws.MockAwsService();
            mockAwsService.CreateTable("Actions");
            mockAwsService.CreateTable("Items");
            return mockAwsService;
        }
    }
}
