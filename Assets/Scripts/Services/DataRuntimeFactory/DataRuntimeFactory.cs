using Zenject;
using static UnityEngine.Rendering.STP;

public class DataRuntimeFactory : IDataRuntimeFactory {
    private DiContainer _container;

    public DataRuntimeFactory(DiContainer container) {
        _container = container;
    }

    public T CreateInstance<T>(IGenericInstanceConfig<T> config) where T : class {
        return _container.Instantiate<T>(new object[] { config });
    }

    public object CreateInstance(IInstanceConfig runtimeConfig) {
        return _container.Instantiate(runtimeConfig.RuntimeType, new object[] { runtimeConfig });
    }
}