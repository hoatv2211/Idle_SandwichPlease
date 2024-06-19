using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CashBox : MonoBehaviour
{
    [SerializeField] private List<Transform> _slots = new List<Transform>();
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _parent;

    [SerializeField] private List<Cash> _listCast;

    public int cashValue = 100;


    [Space, Header("Bonus setup")]
    [SerializeField] private bool _isBonus  = false;
    [SerializeField] private int _cashBonus = 0;

    public bool isPickingCash = false;
    public TutNode tutNode;

    private void OnEnable()
    {
        if (cashValue > 0)
            CallStart();
    }

    public void CallStart()
    {
        SetValueToCashChild();
    }


    [ContextMenu("SetValueToCashChild")]
    private void SetValueToCashChild()
    {
        int _base = 4;
        if (cashValue <= 8)
        {
            int _vlCash = 1;
            int _scale = 1 * _base;
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].gameObject.SetActive(i < cashValue);
                _slots[i].GetComponent<Cash>().SetData(_vlCash);
                _slots[i].transform.localScale = new Vector3(1.5f, _scale, 1.5f);
            }

            return;
        }

        int _surplus = cashValue / 8;
        int _mod = cashValue % 8;
        if (cashValue <= 40)
        {
          
            //Debug.LogError(cashValue +"--" +_surplus + "--" + _mod);
            for (int i = 0; i < _slots.Count; i++)
            {

                int _vlCash = _surplus + (i<_mod ? 1 : 0);
                int _scale = (_surplus + (i<_mod ? 1 : 0)) * _base ;
                _slots[i].gameObject.SetActive(true);
                _slots[i].GetComponent<Cash>().SetData(_vlCash);
                _slots[i].transform.localScale = new Vector3(1.5f, _scale, 1.5f);
                //Debug.LogError(_scale);
            }

            return;
        }


        for (int i = 0; i < _slots.Count; i++)
        {
            int _vlCash = _surplus + (i < _mod ? 1 : 0);
            int _scale = 20+ cashValue /4;
            if (_scale>130)
                _scale=130;

            _slots[i].gameObject.SetActive(true);
            _slots[i].GetComponent<Cash>().SetData(_vlCash);
            _slots[i].transform.localScale = new Vector3(1.5f, _scale, 1.5f);
        }

       
    }

    public void AddOn(int _value)
    {
        cashValue += _value;
        if (gameObject == null)
            return;

        SetValueToCashChild();
    }

    public bool PickUp(Cash cash)
    {
        cash.transform.SetParent(_parent);
        int index = _listCast.Count;
        if (index < _slots.Count)
        {
            _listCast.Add(cash);
        }
        else
        {
            index = _slots.Count - 1;
        }
        Vector3 targetPos = _slots[index].localPosition;
        cash.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
        .OnComplete(() =>
        {
            cash.transform.localPosition = targetPos;
            cash.transform.localRotation = Quaternion.identity;
         
        });

        return false;
    }

    private int costReceive;
    private void OnTriggerEnter(Collider other)
    {
        PlayerPickup playerPick = other.gameObject.GetComponent<PlayerPickup>();
  
        if (playerPick)
        {
            Debug.Log("playerPick");
            costReceive = 0;
            if (!isPickingCash && transform.childCount > 0)
            {
                isPickingCash = true;
                StartCoroutine(PickUpCashSeq(playerPick));
            }
        }

    }

    IEnumerator PickUpCashSeq(PlayerPickup playerPick)
    {
        List<Cash> cash_storage_ref = new List<Cash>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Cash _cash = transform.GetChild(i).GetComponent<Cash>();
            //_cash.value = cashValue / transform.childCount;
            cash_storage_ref.Add(_cash);
            _listCast.Add(_cash);
        }


        //SoundManager.Instance.PlaySFX(SoundDefine.COIN_COLLECTING);
        for (int i = cash_storage_ref.Count - 1; i > -1; i--)
        {
            yield return new WaitUntil(() => playerPick.PickUpCash(cash_storage_ref[i]));
            yield return new WaitForEndOfFrame();
            costReceive += cash_storage_ref[i].value;
            yield return new WaitForSeconds(0.1f);
            _listCast.Remove(cash_storage_ref[i]);
            MapController.Instance.playerCtrl.ShowPickCash(costReceive);
            if (i == 0)
            {
                isPickingCash = false;
                if (_isBonus)
                {
                    Module.money_currency += _cashBonus;
                    costReceive += _cashBonus;
                }


                int money = Module.money_currency;
                FirebaseManager.Instance.LogEvent_receive("money", "money",
                  cashValue, money - cashValue, money,
                  "cash", "money_area", cashValue);

                Destroy(this.gameObject);
            }
        }

        if (tutNode != null)
            tutNode.Action_CompleteTut();
    }

}
