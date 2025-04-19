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
        protected readonly AkkaService _akkaService;

        protected IConfiguration _configuration;

        protected readonly ITestOutputHelper output;

        private readonly TextWriter _originalOut;

        private readonly TextWriter _textWriter;

        protected readonly ILoggingAdapter _logger;

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
            _configuration = configurationBuilder.Build();
            _akkaService = new AkkaService();
            
            // Xunit+Nbench
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
            
            _akkaService.SetDeafaultSystem(this.Sys);

            _logger = this.Sys.Log;

        }

        public static Config GetConfig()
        {
            // https://getakka.net/articles/actors/dispatchers.html

            return ConfigurationFactory.ParseString(@"
                akka {	
	                loglevel = DEBUG
	                loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                }

                # MailBox
                my-custom-mailbox {
                    mailbox-type : ""ActorLib.Actor.Test.IssueTrackerMailbox, ActorLib""
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

                akka.persistence {

                  # Setup the RavenDB journal store:
                  journal {
                    plugin = ""akka.persistence.journal.ravendb""
                    ravendb {
                        class = ""Akka.Persistence.RavenDb.Journal.RavenDbJournal, Akka.Persistence.RavenDb""
                        plugin-dispatcher = ""akka.actor.default-dispatcher""
                        urls = [""http://localhost:9000""]
                        name = ""net-core-labs""
                        auto-initialize = false
                        #certificate-path = ""\\path\\to\\cert.pfx""
                        #save-changes-timeout = 30s
                        #http-version = ""2.0""
                        #disable-tcp-compression = false
                    }
                  }
                   
                  # Setup the RavenDB snapshot store:
                  snapshot-store {
                      plugin = ""akka.persistence.snapshot-store.ravendb""
                      ravendb {
                          class = ""Akka.Persistence.RavenDb.Snapshot.RavenDbSnapshotStore, Akka.Persistence.RavenDb""
                          plugin-dispatcher = ""akka.actor.default-dispatcher""
                          urls = [""http://localhost:9000""]
                          name = ""net-core-labs""
                          auto-initialize = false
                          #certificate-path = ""\\path\\to\\cert.pfx""
                          #save-changes-timeout = 30s
                          #http-version = ""2.0""
                          #disable-tcp-compression = false
                      }
                  }
                  
                  query {
                    # Configure RavenDB as the underlying storage engine for querying:
                    ravendb {
                        class = ""Akka.Persistence.RavenDb.Query.RavenDbReadJournalProvider, Akka.Persistence.RavenDb""
                        #refresh-interval = 3s
                        #max-buffer-size = 65536
                    }
                  }
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
