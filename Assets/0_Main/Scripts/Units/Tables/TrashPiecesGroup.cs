using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class TrashPiecesGroup : MonoBehaviour
{
    public Transform _parent;
    public int countTrash = 4;
    private bool isPickingCash = false;

    public TableCreateArea tableCreate;
    private Coroutine coPlayerPick;
    private Coroutine coWorkerPick;

    private void OnEnable()
    {
        countTrash = transform.childCount;
    }

    public bool isGotTrash()
    {
        bool _bool = _parent.transform.GetComponentsInChildren<TrashPiece>().Length > 0;
            
        
        return _bool;
            
    }

    public void SetInit(TableCreateArea _table)
    {
        tableCreate = _table;
    }

    public void Clean()
    {
        if (isGotTrash())
        {
            foreach (var k in _parent.GetComponentsInChildren<TrashPiece>())
                SimplePool.Despawn(k.gameObject);

            _parent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        PlayerPickup playerPick = other.gameObject.GetComponent<PlayerPickup>();
        if (playerPick)
        {
            if (!isGotTrash())
            {
                tableCreate.CheckClean();
                return;
            }
              

            if (!isPickingCash && isGotTrash())
            {
                isPickingCash = true;
                coPlayerPick= StartCoroutine(PickUpTrashSeq(playerPick));
            }
            
        }

        BotCleanUnit botClean = other.gameObject.GetComponent<BotCleanUnit>();
        if (botClean)
        {
            if (!isGotTrash())
            {
                tableCreate.CheckClean();
                return;
            }

            if (!isPickingCash && isGotTrash() && botClean._tableCr== tableCreate)
            {
                isPickingCash = true;
                coPlayerPick = StartCoroutine(PickUpTrashSeq(botClean));
                return;
            }
        }

        WorkerUnit worker = other.gameObject.GetComponent<WorkerUnit>();
        if (worker&& worker.workerTask!=null)
        {
            if (!isGotTrash())
            {
                tableCreate.CheckClean();
                return;
            }

            if (worker.workerTask.unitBase != tableCreate)
                return;

            if (worker.workerTask.typeWorkerTask != EWorkerTask.Clean_Table)
            {
                worker.EWorkerState = EWorkerState.DeliveryOuput;
                return;
            }

            if (!isPickingCash && isGotTrash())
            {
                isPickingCash = true;
                coWorkerPick = StartCoroutine(PickUpTrashSeq(worker.GetComponent<WorkerPickUp>()));
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (isPickingCash)
            return;

        PlayerPickup playerPick = other.gameObject.GetComponent<PlayerPickup>();
        if (playerPick)
        {
            if (!isGotTrash())
            {
                tableCreate.CheckClean();
                return;
            }


            if (!isPickingCash && isGotTrash())
            {
                isPickingCash = true;
                coPlayerPick = StartCoroutine(PickUpTrashSeq(playerPick));
            }

        }

        BotCleanUnit botClean = other.gameObject.GetComponent<BotCleanUnit>();
        if (botClean)
        {
            if (!isGotTrash())
            {
                tableCreate.CheckClean();
                return;
            }

            if (!isPickingCash && isGotTrash() && botClean._tableCr == tableCreate)
            {
                isPickingCash = true;
                coPlayerPick = StartCoroutine(PickUpTrashSeq(botClean));
                return;
            }
        }

        WorkerUnit worker = other.gameObject.GetComponent<WorkerUnit>();
        if (worker && worker.workerTask != null && worker.workerTask.unitBase == tableCreate)
        {
            if (!isGotTrash())
            {
                tableCreate.CheckClean();
                return;
            }

          
            if (worker.workerTask.typeWorkerTask != EWorkerTask.Clean_Table)
            {
                worker.EWorkerState = EWorkerState.DeliveryOuput;
                return;
            }

            if (!isPickingCash && isGotTrash() && tableCreate.stage == ETableState.GotTrash)
            {
                isPickingCash = true;
                coWorkerPick = StartCoroutine(PickUpTrashSeq(worker.GetComponent<WorkerPickUp>()));
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {

        PlayerPickup playerPick = other.gameObject.GetComponent<PlayerPickup>();

        if (playerPick)
        {
            isPickingCash = false;
            if(coPlayerPick!=null)
                StopCoroutine(coPlayerPick);

        }

        WorkerUnit worker = other.gameObject.GetComponent<WorkerUnit>();
        if (worker)
        {
            isPickingCash = false;
            if (coWorkerPick != null)
                StopCoroutine(coWorkerPick);
        }
    }


    IEnumerator PickUpTrashSeq(PlayerPickup playerPick)
    {
        yield return new WaitUntil(() => _parent.gameObject.activeInHierarchy);

        List<TrashPiece> cash_storage_ref = new List<TrashPiece>();
        for (int i = 0; i < _parent.childCount; i++)
        {
            TrashPiece _cash = _parent.GetChild(i).GetComponent<TrashPiece>();

            if(_cash!=null)
                cash_storage_ref.Add(_cash);
        }

        for (int i = cash_storage_ref.Count - 1; i >=0; i--)
        {
            yield return new WaitUntil(() => playerPick.PickUpTrash(cash_storage_ref[i]));
            yield return null;

            if (i == 0)
            {
                isPickingCash = false;
                tableCreate.CleanTable();

                if (_parent.childCount > 0)
                {
                    Debug.LogError("TRASH OVER");
                    foreach(var k in _parent.GetComponentsInChildren<TrashPiece>())
                        SimplePool.Despawn(k.gameObject);
                   
                }

                _parent.gameObject.SetActive(false);
            }
        }

    
    }

    IEnumerator PickUpTrashSeq(BotCleanUnit bot)
    {
        yield return new WaitUntil(() => _parent.gameObject.activeInHierarchy);

        List<TrashPiece> cash_storage_ref = new List<TrashPiece>();
        for (int i = 0; i < _parent.childCount; i++)
        {
            TrashPiece _cash = _parent.GetChild(i).GetComponent<TrashPiece>();

            if (_cash != null)
                cash_storage_ref.Add(_cash);
        }

        for (int i = cash_storage_ref.Count - 1; i >= 0; i--)
        {
            yield return new WaitUntil(() => bot.PickUp(cash_storage_ref[i]));
            yield return null;

            if (i == 0)
            {
                isPickingCash = false;
                tableCreate.CleanTable();

                if (_parent.childCount > 0)
                {
                    Debug.LogError("TRASH OVER");
                    foreach (var k in _parent.GetComponentsInChildren<TrashPiece>())
                        SimplePool.Despawn(k.gameObject);

                }
                //bot.EBotState = EBotState.Waiting;
                _parent.gameObject.SetActive(false);
            }
        }


    }


    IEnumerator PickUpTrashSeq(WorkerPickUp worker)
    {
        List<TrashPiece> cash_storage_ref = new List<TrashPiece>();
        for (int i = 0; i < _parent.childCount; i++)
        {
            TrashPiece _cash = _parent.GetChild(i).GetComponent<TrashPiece>();

            if (_cash != null)
                cash_storage_ref.Add(_cash);
        }

        for (int i = cash_storage_ref.Count - 1; i >= 0; i--)
        {
            yield return new WaitUntil(() => worker.PickUpTrash(cash_storage_ref[i]));
            yield return null;

            if (i == 0)
            {
                isPickingCash = false;
                worker.GetComponent<WorkerUnit>()._isDoneTask = true;
                tableCreate.CleanTable();

                if (_parent.childCount > 0)
                {
                    Debug.LogError("TRASH OVER");
                    foreach (var k in _parent.GetComponentsInChildren<TrashPiece>())
                        SimplePool.Despawn(k.gameObject);

                }

                _parent.gameObject.SetActive(false);
            }
        }

      
    }
}
