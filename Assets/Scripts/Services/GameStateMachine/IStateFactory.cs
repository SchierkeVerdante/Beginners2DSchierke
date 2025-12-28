using System;
using System.Collections.Generic;
using Zenject;

public interface IStateFactory<TContext> {
    IState<TContext> CreateState<TState>() where TState : class, IState<TContext>;
}


public class StateFactory<TContext> : IStateFactory<TContext> {
    private readonly DiContainer _container;

    public StateFactory(DiContainer container) {
        _container = container;
    }

    public IState<TContext> CreateState<TState>() where TState : class, IState<TContext> {
        return _container.Instantiate<TState>();
    }
}
