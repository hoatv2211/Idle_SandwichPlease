using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThePattern.Unity.Extensions
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventReceiver : MonoBehaviour
    {
        private Animator _animtor;
        private Dictionary<Tuple<string, float>, List<Action>> animationTimelineCallbacks = new Dictionary<Tuple<string, float>, List<Action>>();

        private Animator Animator
        {
            get
            {
                if (!_animtor)
                {
                    _animtor = GetComponent<Animator>();
                }
                return _animtor;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Remove AnimatorEventReceiver", false, 0)]
        void RemoveGUIButton()
        {
            AnimationEventReceiver animationEventReceiver = Selection.activeGameObject.GetComponent<AnimationEventReceiver>();
            Animator animator = Selection.activeGameObject.GetComponent<Animator>();
            if (animationEventReceiver != null)
                GameObject.DestroyImmediate(animationEventReceiver);
            if (animator != null)
                GameObject.DestroyImmediate(animator);
        }
#endif

        public void RegisterTimelineCallback(float atPosition, string clipName, Action callback)
        {
            if (callback == null)
            {
                Debug.LogWarning("Trying to register null animation timeline callback");
                return;
            }

            if (!animationTimelineCallbacks.ContainsKey(new Tuple<string, float>(clipName, atPosition)))
            {
                animationTimelineCallbacks.Add(new Tuple<string, float>(clipName, atPosition), new List<Action>());
            }

            animationTimelineCallbacks[new Tuple<string, float>(clipName, atPosition)].Add(callback);
        }

        public bool UnregisterTimelineCallback(float atPosition, string clipName, Action callback)
        {
            if (callback == null)
            {
                Debug.LogWarning("Trying to unregister null animation timeline callback");
                return false;
            }

            if (!animationTimelineCallbacks.ContainsKey(new Tuple<string, float>(clipName, atPosition)))
            {
                Debug.LogWarningFormat("Trying to unregister animation timeline callback not registered at timeline position {0}", atPosition);
                return false;
            }

            var removed = animationTimelineCallbacks[new Tuple<string, float>(clipName, atPosition)].Remove(callback);
            if (!removed)
            {
                Debug.LogWarning("Failed to unregister animation timeline callback since it was not registered");
                return false;
            }

            var lastCallbackForPositionRemoved = animationTimelineCallbacks[new Tuple<string, float>(clipName, atPosition)].Count == 0;
            if (lastCallbackForPositionRemoved)
            {
                animationTimelineCallbacks.Remove(new Tuple<string, float>(clipName, atPosition));
            }

            return lastCallbackForPositionRemoved;
        }

        // Unity binds animation events by method name. This means all components
        // which have method with the name from animation event will be called.
        // Such component must be attached to the object with Animator/Animation.
        public void OnTimelineEventRaised(float atPosition)
        {
            string clipName;
            if (Mathf.Approximately(atPosition, 0) && Animator.GetAnimatorTransitionInfo(0).duration > 0)
            {
                clipName = Animator.GetNextAnimatorClipInfo(0)[0].clip.name;
            }
            else
            {
                clipName = Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            }
            if (!animationTimelineCallbacks.ContainsKey(new Tuple<string, float>(clipName, atPosition)))
            {
                // Debug.LogWarningFormat("Callbacks not registered for timeline position {0}", atPosition);
                return;
            }
            var animationPositionCallbacks = animationTimelineCallbacks[new Tuple<string, float>(clipName, atPosition)];
            FireCallbacks(animationPositionCallbacks);
        }

        // Unity cannot call static method from animation event so FireCallbacks
        // cannot be called for AnimationEventReceiver added by user.
        private static void FireCallbacks(List<Action> callbacks)
        {
            // In current implementation registered callbacks cannot be removed.
            // Store current count in local variable for the case if callback
            // adds new callback to the list. Added one will be triggered next
            // time animation event happens.
            callbacks.ForEach(callback =>
            {
                callback?.Invoke();
            });
        }
    }
}
