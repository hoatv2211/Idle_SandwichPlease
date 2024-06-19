using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;

public class ReactiveVariable<T> : ReactiveProperty<T>
{
    public T OldValue { get; private set; }
    private T tempValue = default(T);
    Subject<T> _onValueChange = new Subject<T>();
    public ReactiveVariable() : base()
    {
        this.Subscribe((obj) =>
        {
            CheckValueChange(obj);
        });
    }

    public ReactiveVariable(T initialVal) : base(initialVal)
    {
        this.Subscribe((obj) =>
        {
            CheckValueChange(obj);
        });
    }

    private void CheckValueChange(T obj)
    {
        OldValue = tempValue;
        tempValue = obj;
        if (tempValue !=null && !tempValue.Equals(OldValue))
        {
            _onValueChange.OnNext(obj);
        }
    }
    public IObservable<T> OnValueChange() => _onValueChange;
}