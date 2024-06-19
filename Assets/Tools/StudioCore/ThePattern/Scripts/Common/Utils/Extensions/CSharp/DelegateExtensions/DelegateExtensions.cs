using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DelegateExtensions
{
    public static Type[] GetParameterTypes<T>(this T mDelegate) where T : Delegate
    {
        Type type = mDelegate.GetType();
        if (type.BaseType != typeof(MulticastDelegate))
            throw new ArgumentException("Not a delegate.", nameof(type));

        MethodInfo invoke = type.GetMethod("Invoke");
        if (invoke == null)
            throw new ArgumentException("Not a delegate.", nameof(type));


        ParameterInfo[] parameters = invoke.GetParameters();
        Type[] typeParameters = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            typeParameters[i] = parameters[i].ParameterType;
        }
        return typeParameters;
    }

    public static Type GetReturnType<T>(this T mDelegate) where T : Delegate
    {
        Type type = mDelegate.GetType();
        if (type.BaseType != typeof(MulticastDelegate))
            throw new ArgumentException("Not a delegate.", nameof(type));

        MethodInfo invoke = type.GetMethod("Invoke");
        if (invoke == null)
            throw new ArgumentException("Not a delegate.", nameof(type));

        return invoke.ReturnType;
    }

    public static Action<object> Convert<T>(this Action<T> myActionT)
    {
        if (myActionT == null) return null;
        else return new Action<object>(o => myActionT((T)o));
    }
}