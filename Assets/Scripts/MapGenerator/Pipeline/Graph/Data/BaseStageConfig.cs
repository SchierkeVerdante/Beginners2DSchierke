using System;

[Serializable]
public abstract class BaseStageConfig<TContext>
    where TContext : class {
    public string stageName;

    public abstract IPipelineStage<TContext> CreateStage();
}
