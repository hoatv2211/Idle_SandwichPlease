using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ECurrency
{
    Money,
    Ticket
}
public class LabelCurrency : MonoBehaviour
{
    [SerializeField] private ECurrency Type;
    [SerializeField] private TextMeshProUGUI txtCurrency;
    private void OnEnable()
    {
        Module.Event_Change_Money += Module_Event_Change_Money;
        Module_Event_Change_Money();
    }

    private void Module_Event_Change_Money()
    {
        switch (Type)
        {
            case ECurrency.Money:
                txtCurrency.text = Module.money_currency.ToString();
                break;
            case ECurrency.Ticket:
                txtCurrency.text = Module.ticket_currency.ToString();
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        Module.Event_Change_Money -= Module_Event_Change_Money;
    }
}
