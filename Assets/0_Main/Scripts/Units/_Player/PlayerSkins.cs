using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkins : MonoBehaviour
{
    public ETypeSkin typeSkin;
    [SerializeField] private List<GameObject> listSkins;


    private void OnEnable()
    {
        Module.Event_SkinChange += Module_Event_SkinChange;
    }

    private void Module_Event_SkinChange(ETypeSkin _type)
    {
        string _name = "skin_" + _type.ToString();
        //Debug.LogError(_name);
        foreach (var k in listSkins)
            k.SetActive(k.name == _name);
    }

    private void OnDisable()
    {
        Module.Event_SkinChange -= Module_Event_SkinChange;
    }
}
