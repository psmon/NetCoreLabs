using System.Diagnostics;

using ActorLib;

using Akka.Configuration;
using Akka.Event;
using Akka.TestKit.Xunit2;

using Microsoft.Extensions.Configuration;

using NBench;

using Pro.NBench.xUnit.XunitExtensions;

using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace ActorLibTest
{
    public abstract class TestKitXunit : TestKit
    {
        protected readonly AkkaService akkaService;

        protected IConfiguration configuration;

        protected readonly ITestOutputHelper output;

        private readonly TextWriter _originalOut;

        private readonly TextWriter _textWriter;

        protected readonly ILoggingAdapter logger;

        protected readonly Dictionary<int, int> _dictionary = new Dictionary<int, int>();

        protected readonly List<int[]> _dataCache = new List<int[]>();

        protected Counter _addCounter;

        protected int _key;


        public TestKitXunit(ITestOutputHelper output) : base(GetConfig())
        {
            this.output = output;
            _originalOut = Console.Out;
            _textWriter = new StringWriter();
            Console.SetOut(_textWriter);

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configuration = configurationBuilder.Build();
            akkaService = new AkkaService();


            // Xunit+Nbench
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));

            // 실 사용서비스에서는 ActorSystem을 생성해야합니다.
            // TestToolKit에서는 테스트검증을 위한 기본 ActorSystem이 생성되어
            // 여기서는 생성된 ActorSystem을 이용합니다.           
            // 시스템생성코드 : akkaService.CreateActorSystem("test");

            akkaService.FromActorSystem(this.Sys);

            logger = this.Sys.Log;

        }

        public static Config GetConfig()
        {
            // https://getakka.net/articles/actors/dispatchers.html

            return ConfigurationFactory.ParseString(@"
                akka {	
	                loglevel = DEBUG
	                loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]                
                }

                # Dispatchers

                custom-dispatcher {
                    type = Dispatcher
                    throughput = 100
                }

                custom-task-dispatcher {
                  type = TaskDispatcher
                  throughput = 100
                }

                custom-dedicated-dispatcher {
                  type = PinnedDispatcher
                }

                fork-join-dispatcher {
                  type = ForkJoinDispatcher
                  throughput = 1
                  dedicated-thread-pool {
                      thread-count = 1
                      deadlock-timeout = 3s
                      threadtype = background
                  }
                }

                synchronized-dispatcher {
                  type = SynchronizedDispatcher
                  throughput = 100
                }

            ");
        }

        protected override void Dispose(bool disposing)
        {
            output.WriteLine(_textWriter.ToString());
            Console.SetOut(_originalOut);
            base.Dispose(disposing);
        }

        [PerfCleanup]
#pragma warning disable xUnit1013 // Public method should be marked as test
        public void Cleanup(BenchmarkContext context)
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            _dictionary.Clear();
            _dataCache.Clear();
        }



    }
}
