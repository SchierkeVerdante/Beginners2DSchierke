using System;
using UnityEngine;

public interface IRuntimeConfig {
    public Type RuntimeType { get; }
}

public abstract class BaseStageConfig<TContext> : ScriptableObject, IRuntimeConfig
    where TContext : class {
    public string stageName;

    public abstract Type RuntimeType { get; }

    protected virtual void OnValidate() {
        if (string.IsNullOrEmpty(stageName)) {
            stageName = GetType().Name;
        }
    }
}

