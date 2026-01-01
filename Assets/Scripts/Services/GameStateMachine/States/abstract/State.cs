public abstract class State<TContext> : IState<TContext> {
    protected TContext Context { get; private set; }

    public void Initialize(TContext context) {
        Context = context;
        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}