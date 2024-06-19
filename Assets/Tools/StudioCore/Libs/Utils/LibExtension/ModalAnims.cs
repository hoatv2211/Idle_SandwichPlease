using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ModalAnims : MonoBehaviour
{
    [SerializeField] private List<DOTweenAnimation> _animations;

    public List<DOTweenAnimation> Animations => _animations;
}
