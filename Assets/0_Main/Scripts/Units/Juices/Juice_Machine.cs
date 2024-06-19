using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Juice_Machine : UnitBase
{
    public Juice_storage juice_Storage;

    [Header("Money Tip")]
    public Transform trMoneyArea;
    public GameObject cashBox;
    public int cashValue = 5;

    public void Action_MoneyArea()
    {
        if (trMoneyArea.childCount > 0)
        {
            try
            {
                CashBox _cashbox = trMoneyArea.GetComponentsInChildren<CashBox>().First(x => !x.isPickingCash);
                if (_cashbox != null)
                {
                    _cashbox.AddOn(cashValue);
                }
                else
                {
                    GameObject g = Instantiate(cashBox.gameObject, trMoneyArea.position, Quaternion.identity);
                    g.transform.SetParent(trMoneyArea);

                    _cashbox = g.GetComponent<CashBox>();
                    _cashbox.AddOn(cashValue);
                }
            }
            catch /*(Exception e)*/
            {
                //Debug.LogError(e.Message);
               
            }

        }
        else
        {
            GameObject g = Instantiate(cashBox.gameObject, trMoneyArea.position, Quaternion.identity);
            g.transform.SetParent(trMoneyArea);

            CashBox _cashbox = g.GetComponent<CashBox>();
            _cashbox.AddOn(cashValue);
        }
        SoundManager.Instance.PlayOnCamera(2);
    }

}
