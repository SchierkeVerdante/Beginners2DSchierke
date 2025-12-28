using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {
    private IState _currentState;
    public IState CurrentState => _currentState;
    public Action<IState> OnStateChanged;

    public void ChangeState(IState newState) {
        if (ReferenceEquals(newState, _currentState)) {
            Debug.Log($"Already in state {newState?.GetType().Name ?? "null"}");
            return;
        }
        _currentState?.Exit();

        _currentState = newState;
        _currentState.Enter();

        OnStateChanged?.Invoke(_currentState);
    }

    public void Tick() {
        _currentState?.Update();
    }
}

public class ContextStateMachine<TContext> : StateMachine {

    private readonly IStateFactory<TContext> _stateFactory;
    public TContext Context { get; }

    private readonly Dictionary<Type, IState<TContext>> _states = new Dictionary<Type, IState<TContext>>();

    public ContextStateMachine(TContext context, IStateFactory<TContext> stateFactory) {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        _stateFactory = stateFactory ?? throw new ArgumentNullException(nameof(stateFactory));
    }

    public void ChangeState<TState>() where TState : class, IState<TContext> {
        var type = typeof(TState);

        if (CurrentState?.GetType() == type) {
            Debug.Log($"Already in state {type.Name}");
            return;
        }

        if (!_states.TryGetValue(type, out var newState)) {
            newState = _stateFactory.CreateState<TState>()
                ?? throw new InvalidOperationException($"State factory failed to create state of type {type.Name}");

            newState.Initialize(Context);
            _states.Add(type, newState);
        }

        base.ChangeState(newState);
    }

    public void ForceChangeState<TState>() where TState : class, IState<TContext> {
        var type = typeof(TState);
        _states.Remove(type);
        ChangeState<TState>();
    }

    public void Dispose() {
        foreach (var state in _states.Values) {
            if (state is IDisposable disposable) {
                disposable.Dispose();
            }
        }
        _states.Clear();
    }
}
