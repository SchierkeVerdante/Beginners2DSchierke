using System.Linq;
using UnityEditor.Rendering.LookDev;
using Zenject;
public interface IPresenterFactory<TPresenter> {
    TPresenter Create(IView view, IModel model, params object[] extraArgs);
}

public class PresenterPlaceholderFactory<TPresenter> : IPresenterFactory<TPresenter> {
    private readonly DiContainer _container;

    public PresenterPlaceholderFactory(DiContainer container) {
        _container = container;
    }

    public TPresenter Create(IView view, IModel model, params object[] extraArgs) {
        // об'єднуємо базові аргументи з додатковими
        var args = new object[] { view, model }
            .Concat(extraArgs)
            .ToArray();

        return _container.Instantiate<TPresenter>(args);
    }
}


public interface IView {

}

public interface IModel {

}
