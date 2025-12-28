public interface IState {
    void Enter();
    void Exit();
    void Update();
}

public interface IState<TContext> : IState {
    void Initialize(TContext context);
}