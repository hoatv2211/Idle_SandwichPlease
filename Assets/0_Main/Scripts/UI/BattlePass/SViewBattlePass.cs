using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SViewBattlePass : MonoBehaviour, IEnhancedScrollerDelegate
{
    public List<Level_User> listLevels;

    [Space(5)]
    [SerializeField] private EnhancedScroller sView;
    [SerializeField] private EnhancedScrollerCellView cellView;

    [Header("Parameter")]
    [SerializeField] private float sizeView = 240;       // size
    [SerializeField] private int numOfCell = 0;        // so luong cell
    [SerializeField] private int surplus = 0;         // so du sau khi chia
    [SerializeField] private int countOfCell = 4;    // so luong item trong 1 hang

    public void CallStart(List<Level_User> _listLv, bool _isRefresh = false)
    {
        listLevels = _listLv;
        numOfCell = listLevels.Count / countOfCell;
        surplus = listLevels.Count % countOfCell;

        if (surplus != 0)
            numOfCell += 1;

        sView.Delegate = this;
        DOVirtual.DelayedCall(0.3f, () => sView.ReloadData());

        //if (_isRefresh)
        //    sView.RefreshActiveCellViews();
        //else
        //    sView.ReloadData();
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        TaskViewBattle _cellView = scroller.GetCellView(cellView) as TaskViewBattle;
        //List<LevelInfo> _listView = new List<LevelInfo>();

        //int _s = dataIndex * countOfCell;
        //if (dataIndex == (numOfCell - 1) && surplus != 0)
        //{
        //    _listView = listLevels.GetRange(_s, surplus);
        //}
        //else
        //{
        //    _listView = listLevels.GetRange(_s, countOfCell);
        //}

        _cellView.CallStart(listLevels[dataIndex]);


        return _cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return sizeView;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return numOfCell;
    }
}
