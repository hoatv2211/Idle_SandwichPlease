using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ETypeBonus
{
    speed_bonus,
    capaticy_bonus,
    staff_employ,
    staff_speed,
    staff_capacity,
    single_price_bonus,
    package_price_bonus
}

public class TagSkin : MonoBehaviour
{
    public ETypeBonus typeBonus;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtValue;
    [SerializeField] private Image imgBackground;
    [SerializeField] private Sprite[] sprBgs;

    public void CallStart(ETypeBonus _type,float _value)
    {
        typeBonus = _type;
        switch (typeBonus)
        {
            case ETypeBonus.speed_bonus:
                txtTitle.text = "Move speed";
                txtValue.text = "+" + _value*10;
                imgBackground.sprite = sprBgs[2];
                break;
            case ETypeBonus.capaticy_bonus:
                txtTitle.text = "Capacity";
                txtValue.text = "+" + _value;
                imgBackground.sprite = sprBgs[0];
                break;
            case ETypeBonus.staff_employ:
                txtTitle.text = "Staff More";
                txtValue.text = "+" + _value;
                imgBackground.sprite = sprBgs[1];
                break;
            case ETypeBonus.staff_speed:
                txtTitle.text = "Staff move speed";
                txtValue.text = "+" + _value * 10;
                imgBackground.sprite = sprBgs[2];
                break;
            case ETypeBonus.staff_capacity:
                txtTitle.text = "Staff capacity";
                txtValue.text = "+" + _value;
                imgBackground.sprite = sprBgs[0];
                break;
            case ETypeBonus.single_price_bonus:
                txtTitle.text = "Single Price";
                txtValue.text = "+" + _value;
                imgBackground.sprite = sprBgs[1];
                break;
            case ETypeBonus.package_price_bonus:
                txtTitle.text = "Package Price";
                txtValue.text = "+" + _value;
                imgBackground.sprite = sprBgs[2];
                break;
            default:
                break;
        }
    }
}
