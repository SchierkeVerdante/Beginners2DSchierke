using System;
using UnityEngine;

public interface IInstanceConfig {
    public Type RuntimeType { get; }
}

public abstract class InstanceConfig : ScriptableObject, IInstanceConfig {
    [SerializeField] public string instanceName;

    public abstract Type RuntimeType { get; }

    protected virtual void OnValidate() {
        if (string.IsNullOrEmpty(instanceName)) {
            instanceName = GetType().Name;
        }
    }
}

public interface IGenericInstanceConfig<out T> : IInstanceConfig where T : class {

}

public class GenericInstanceConfig<TContext> : InstanceConfig, IGenericInstanceConfig<TContext>
    where TContext : class {
    public override Type RuntimeType => typeof(TContext);
}
