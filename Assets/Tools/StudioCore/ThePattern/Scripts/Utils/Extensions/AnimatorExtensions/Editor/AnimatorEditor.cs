using UnityEngine;
using UnityEditor;

using ThePattern.Unity.Extensions;

[CustomEditor(typeof(Animator))]
public class AnimatorEditor : Editor
{
    private Animator _animator;

    private void Awake()
    {
        _animator = (Animator)this.target;
        if(_animator.GetComponent<AnimationEventReceiver>() == null)
        {
            _animator.gameObject.AddComponent<AnimationEventReceiver>();
        }
    }
}
