using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;

namespace ActorLib.Actor.Tools.FSMBatch;

public class FSMBatchActor : FSM<State, IData>
{
    private readonly ILoggingAdapter _log = Context.GetLogger();

    public FSMBatchActor()
    {
        // <StartWith>
        StartWith(State.Idle, Uninitialized.Instance);
        // </StartWith>
        
        When(State.Idle, state =>
        {
            if (state.FsmEvent is SetTarget target && state.StateData is Uninitialized)
            {
                return Stay().Using(new Todo(target.Ref, ImmutableList<object>.Empty));
            }

            return null;
        });
        
        When(State.Active, state =>
        {
            if (state.FsmEvent is Flush or StateTimeout 
                && state.StateData is Todo t)
            {
                return GoTo(State.Idle).Using(t.Copy(ImmutableList<object>.Empty));
            }

            return null;
        }, TimeSpan.FromSeconds(1));
        
        
        WhenUnhandled(state =>
        {
            if (state.FsmEvent is Queue q && state.StateData is Todo t)
            {
                return GoTo(State.Active).Using(t.Copy(t.Queue.Add(q.Obj)));
            }
            else
            {
                _log.Warning("Received unhandled request {0} in state {1}/{2}", state.FsmEvent, StateName, state.StateData);
                return Stay();
            }
        });
        
        OnTransition((initialState, nextState) =>
        {
            if (initialState == State.Active && nextState == State.Idle)
            {
                if (StateData is Todo todo)
                {
                    todo.Target.Tell(new Batch(todo.Queue));
                }
                else
                {
                    // nothing to do
                }
            }
        });
        
        Initialize();
    }
    
}