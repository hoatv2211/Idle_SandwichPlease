using System;
using System.Linq;
using UnityEngine;

namespace ThePattern.Unity.Extensions
{
    public static class AnimatorExtensions
    {
        public static bool ContainsParam(this Animator animator, string paramName)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
                if (param.name == paramName)
                    return true;
            return false;
        }

        public static float GetAnimationClipLength(this Animator animator, string clipName)
        {
            AnimationClip clip = animator.runtimeAnimatorController.animationClips.First(animClip => animClip.name == clipName);
            if (!clip) return 0;
            return clip.length;
        }

        public static string GetCurrentAnimationClipName(this Animator animator)
        {
            AnimatorClipInfo[] temp = animator.GetCurrentAnimatorClipInfo(0);
            return temp[0].clip.name;
        }

        public static void AddClipStartCallback(this Animator animator, int layerIndex, string clipName, Action callback)
        {
            animator.AddClipCallback(layerIndex, clipName, 0.0f, callback);
        }

        public static void AddClipEndCallback(this Animator animator, int layerIndex, string clipName, Action callback)
        {
            var clip = animator.GetAnimationClip(layerIndex, clipName);
            if (clip == null)
            {
                Debug.LogWarning("Failed to get animation clip for Animator component");
                return;
            }

            clip.BindCallback(animator.gameObject, clip.length, callback);
        }

        public static void AddClipCallback(this Animator animator, int layerIndex, string clipName, float atPosition, Action callback)
        {
            var clip = animator.GetAnimationClip(layerIndex, clipName);
            if (clip == null)
            {
                Debug.LogWarning("Failed to get animation clip for Animator component");
                return;
            }

            clip.BindCallback(animator.gameObject, atPosition, callback);
        }

        public static void RemoveClipStartCallback(this Animator animator, int layerIndex, string clipName, Action callback)
        {
            animator.RemoveClipCallback(layerIndex, clipName, 0.0f, callback);
        }

        public static void RemoveClipEndCallback(this Animator animator, int layerIndex, string clipName, Action callback)
        {
            var clip = animator.GetAnimationClip(layerIndex, clipName);
            if (clip == null)
            {
                Debug.LogWarning("Failed to get animation clip for Animator component");
                return;
            }

            clip.UnbindCallback(animator.gameObject, clip.length, callback);
        }

        public static void RemoveClipCallback(this Animator animator, int layerIndex, string clipName, float atPosition, Action callback)
        {
            var clip = animator.GetAnimationClip(layerIndex, clipName);
            if (clip == null)
            {
                Debug.LogWarning("Failed to get animation clip for Animator component");
                return;
            }

            clip.UnbindCallback(animator.gameObject, atPosition, callback);
        }

        private static AnimationClip GetAnimationClip(this Animator animator, int layerIndex, string clipName)
        {
            AnimationClip clip = animator.runtimeAnimatorController.animationClips.First(animClip => animClip.name == clipName);
            if (!clip)
            {
                Debug.LogWarningFormat("Clip with name {0} not found in layer with index {1}", clipName, layerIndex);
                return null;
            }
            return clip;
        }
    }
}
