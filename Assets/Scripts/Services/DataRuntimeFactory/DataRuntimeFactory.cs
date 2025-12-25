using Zenject;

public class DataRuntimeFactory : IDataRuntimeFactory {
    private DiContainer _container;

    public DataRuntimeFactory(DiContainer container) {
        _container = container;
    }

    public object CreateInstanse(IRuntimeConfig data) {
        return _container.Instantiate(data.RuntimeType, new object[] { data });
    }
}