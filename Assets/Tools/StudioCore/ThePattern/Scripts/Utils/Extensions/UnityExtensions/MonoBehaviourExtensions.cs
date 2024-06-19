using System;
using System.Collections;

using UniRx;

using UnityEngine;

namespace ThePattern.Extensions
{
    public static class MonoBehaviourExtensions
    {
        #region Coroutine Defination
        private static IEnumerator DoAction(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
        private static IEnumerator DoActionRealtime(float time, Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action?.Invoke();
        }
        private static IEnumerator DoActionWaitUntil(Func<bool> func, Action action)
        {
            yield return new WaitUntil(func);
            action?.Invoke();
        }
        private static IEnumerator DoActionWaitWhile(Func<bool> func, Action action)
        {
            yield return new WaitWhile(func);
            action?.Invoke();
        }
        private static IEnumerator DoActionWaitForEndOfFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
        private static IEnumerator DoActionWhile(Func<bool> func, Action action, Action after = null)
        {
            while (func.Invoke())
            {
                action?.Invoke();
                yield return new WaitForEndOfFrame();
            }
            after?.Invoke();
        }
        #endregion

        #region Extension Method
        /// <summary>
        /// Do Action Wait a Time
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="time">time</param>
        /// <param name="action">action to do</param>
        /// <param name="isUseMainThread">using MainThreadDispatcher of UniRx?</param>
        /// <returns></returns>
        public static Coroutine ActionWaitTime(this MonoBehaviour mono, float time, Action action, bool isUseMainThread = false)
        {
            return isUseMainThread ? MainThreadDispatcher.StartCoroutine(DoAction(time, action)) : mono.StartCoroutine(DoAction(time, action));
        }
        /// <summary>
        /// Do Action Wait a Real time
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="time">time</param>
        /// <param name="action">action to do</param>
        /// <param name="isUseMainThread">using MainThreadDispatcher of UniRx?</param>
        /// <returns></returns>
        public static Coroutine ActionWaitRealTime(this MonoBehaviour mono, float time, Action action, bool isUseMainThread = false)
        {
            return isUseMainThread ? MainThreadDispatcher.StartCoroutine(DoActionRealtime(time, action)) : mono.StartCoroutine(DoActionRealtime(time, action));
        }
        /// <summary>
        /// Do Action Wait until a Func is true
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="func">func check</param>
        /// <param name="action">action to do</param>
        /// <param name="isUseMainThread">using MainThreadDispatcher of UniRx?</param>
        /// <returns></returns>
        public static Coroutine ActionWaitUntil(this MonoBehaviour mono, Func<bool> func, Action action, bool isUseMainThread = false)
        {
            return isUseMainThread ? MainThreadDispatcher.StartCoroutine(DoActionWaitUntil(func, action)) : mono.StartCoroutine(DoActionWaitUntil(func, action));
        }
        /// <summary>
        /// Do Action Wait while a Func is true
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="func">func check</param>
        /// <param name="action">action to do</param>
        /// <param name="isUseMainThread">using MainThreadDispatcher of UniRx?</param>
        /// <returns></returns>
        public static Coroutine ActionWaitWhile(this MonoBehaviour mono, Func<bool> func, Action action, bool isUseMainThread = false)
        {
            return isUseMainThread ? MainThreadDispatcher.StartCoroutine(DoActionWaitWhile(func, action)) : mono.StartCoroutine(DoActionWaitWhile(func, action));
        }
        /// <summary>
        /// Do Action Wait For End of Frame
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="action">action to do</param>
        /// <param name="isUseMainThread">using MainThreadDispatcher of UniRx?</param>
        /// <returns></returns>
        public static Coroutine ActionWaitForEndOfFrame(this MonoBehaviour mono, Action action, bool isUseMainThread = false)
        {
            return isUseMainThread ? MainThreadDispatcher.StartCoroutine(DoActionWaitForEndOfFrame(action)) : mono.StartCoroutine(DoActionWaitForEndOfFrame(action));
        }
        /// <summary>
        /// Do Action While Func is true
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="func">func check</param>
        /// <param name="action">action to do</param>
        /// <param name="after">action onComplete</param>
        /// <param name="isUseMainThread">using MainThreadDispatcher of UniRx?</param>
        /// <returns></returns>
        public static Coroutine ActionWhile(this MonoBehaviour mono, Func<bool> func, Action action, Action after = null, bool isUseMainThread = false)
        {
            return isUseMainThread ? MainThreadDispatcher.StartCoroutine(DoActionWhile(func, action, after)) : mono.StartCoroutine(DoActionWhile(func, action, after));
        }
        #endregion
    }
}

