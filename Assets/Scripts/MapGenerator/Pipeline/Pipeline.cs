using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPipelineStage<TContext> where TContext : class {
    string StageName { get; }
    void Execute(TContext context);
}

public abstract class Pipeline<TContext> where TContext : class {
    protected List<IPipelineStage<TContext>> stages = new List<IPipelineStage<TContext>>();

    public Pipeline<TContext> AddStage(IPipelineStage<TContext> stage) {
        stages.Add(stage);
        return this;
    }

    public Pipeline<TContext> RemoveStage(int stageIndex) {
        if (stageIndex >= 0 && stageIndex < stages.Count)
            stages.RemoveAt(stageIndex);
        return this;
    }

    public Pipeline<TContext> InsertStage(int index, IPipelineStage<TContext> stage) {
        if (index >= 0 && index <= stages.Count)
            stages.Insert(index, stage);
        return this;
    }

    public Pipeline<TContext> ClearStages() {
        stages.Clear();
        return this;
    }

    public virtual void Execute(TContext context) {
        OnExecutionStart(context);

        foreach (var stage in stages) {
            OnStageStart(stage, context);
            stage.Execute(context);
            OnStageComplete(stage, context);
        }

        OnExecutionComplete(context);
    }

    protected virtual void OnExecutionStart(TContext context) { }
    protected virtual void OnStageStart(IPipelineStage<TContext> stage, TContext context) { }
    protected virtual void OnStageComplete(IPipelineStage<TContext> stage, TContext context) { }
    protected virtual void OnExecutionComplete(TContext context) { }

    public List<string> GetStageNames() => stages.Select(s => s.StageName).ToList();
}

