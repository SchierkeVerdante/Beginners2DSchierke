public interface IDataRuntimeFactory {
    T CreateInstance<T>(IGenericInstanceConfig<T> config) where T : class;
    object CreateInstance(IInstanceConfig runtimeConfig);
}