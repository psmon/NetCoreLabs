# NetCoreLabs

NetCore����� Ȱ���Ͽ� Reactive Stream�̶�� ������ �ַ� �ٷ�� ����������Ʈ��

������ ���� �����Ǿ� �ֽ��ϴ�.


## ActorLib

akka.net(https://getakka.net/)�� �̿��Ͽ� ������ ���͸𵨻����� ���� �߱��ϴ� ��������Ʈ�Դϴ�.
akka�� �ƴϿ��� �ֺ������ �����ϰ� ��Ʈ������ ó���ϴ� �޽��������� �����ϴ°Ϳ� �� ��ġ�� �ΰ� �ֽ��ϴ�.


### �⺻����
```
    public class BasicActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private IActorRef testProbe;

        public BasicActor()
        {

            ReceiveAsync<IActorRef>(async actorRef =>
            {
                testProbe = actorRef;
            });

            ReceiveAsync<string>(async msg =>
            {                
                if (msg.Contains("slowCommand"))
                {                                        
                    await Task.Delay(10);
                    
                    if (testProbe != null)
                    {
                        testProbe.Tell(msg);
                    }
                }
                else
                {                    
                    if (testProbe != null)
                    {
                        testProbe.Tell("world");
                    }
                    else
                    {
                        Sender.Tell("world");
                    }
                }                                
            });
        }
    }
```

## ActorLibTest

�ǽð����� �߻��ϴ� �޽�¡�� �����׽�Ʈȭ�Ͽ� �����۵��ϴ� ������� �帧�� �ľ��ϰ� ����� �����Ҽ� �ֽ��ϴ�.
���ʽ��� ��������(https://nbench.io/) ���� Ȱ���ϴ� ����� �˼� �ֽ��ϴ�.
���ͳݿ� ������ ���� �л�ó�������� ��Ű��ó�� �÷������� �۵������ϰ� ��õ������ ��Ű��ó�� ����°Ϳ� ��ġ�� �ΰ� �ֽ��ϴ�.


### ���ɰ��� ���� �����׽�Ʈ
```
[Theory(DisplayName = "�׽�Ʈ n�ʴ� 1ȸ ȣ������")]
[InlineData(5, 1, false)]
public void ThrottleLimitTest(int givenTestCount, int givenLimitSeconds, bool isPerformTest)
{
.......
  // Create ThrottleLimit Actor
  throttleLimitActor = actorSystem.ActorOf(Props.Create(() => new ThrottleLimitActor(1, givenLimitSeconds, 1000)));
  throttleLimitActor.Tell(new SetTarget(probe))
   
  for (int i = 0; i < givenTestCount; i++)
  {
      throttleLimitActor.Tell(new EventCmd()
      {
          Message = "test",
      });
  }
   
  //Then : Safe processing within N seconds limit
  for (int i = 0; i < givenTestCount; i++)
  {
      probe.ExpectMsg<EventCmd>(message =>
      {
          Assert.Equal("test", message.Message);                       
      });
   
      output.WriteLine($"[{DateTime.Now}] - GTPRequestCmd");
   
      if (isPerformTest)
      {
          _dictionary.Add(_key++, _key);
          _addCounter.Increment();
      }
  }
...........
}

[NBenchFact]
[PerfBenchmark(NumberOfIterations = 3, RunMode = RunMode.Throughput,
RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
[CounterThroughputAssertion("TestCounter", MustBe.LessThanOrEqualTo, 1.0d)]
[CounterTotalAssertion("TestCounter", MustBe.LessThanOrEqualTo, 1)]
[CounterMeasurement("TestCounter")]
public void ThrottleLimitPerformanceTest()
{
    ThrottleLimitTest(1, 1, true);
}
```


## BlazorActorApp

Blazor�� ���� ���͸��� ���� ����Ǿ� �۵��Ǵ� ���õ��� ����� ������Ʈ�Դϴ�.
���͸��� �������� �����Ҽ� �ִ� ���ڵ����� ���������� ���ͽð�ȭ Ȱ���� ���ԵǾ� �ֽ��ϴ�.


### 
![dispacher](Doc/router-roundrobin.png)




