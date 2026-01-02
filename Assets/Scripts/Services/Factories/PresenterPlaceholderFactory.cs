using UnityEditor.Rendering.LookDev;
using Zenject;

public interface IPresenterFactory<TPresenter> {
    TPresenter Create(IView view, IModel model);
}

public class PresenterPlaceholderFactory<TPresenter> : IPresenterFactory<TPresenter> {
    private DiContainer _container;

    public PresenterPlaceholderFactory(DiContainer container) {
        _container = container;
    }

    public TPresenter Create(IView view, IModel model) {
        return _container.Instantiate<TPresenter>(new object[] { view, model });
    }
}


public interface IView {

}

public interface IModel {

}
