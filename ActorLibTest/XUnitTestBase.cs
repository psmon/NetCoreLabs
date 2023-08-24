using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ActorLib;

using Microsoft.Extensions.Configuration;

using Xunit.Abstractions;

namespace ActorLibTest
{
    public abstract class XUnitTestBase
    {
        public IConfiguration configuration;

        public readonly ITestOutputHelper output;


        public XUnitTestBase(ITestOutputHelper output)
        {
            this.output = output;
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configuration = configurationBuilder.Build();
        }

    }
}
