using System;

public class ReactiveProperty<T> {
    private T _value;
    public event Action<T> OnChanged;

    public T Value {
        get => _value;
        set {
            if (Equals(_value, value)) return;
            _value = value;
            OnChanged?.Invoke(_value);
        }
    }

    public ReactiveProperty(T initial) => _value = initial;
}