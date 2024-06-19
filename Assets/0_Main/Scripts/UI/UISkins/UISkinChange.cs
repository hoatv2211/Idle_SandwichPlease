using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;

public class UISkinChange : MonoBehaviour
{
    public SkinModel skinModel;

    [Header("HUD")]
    [SerializeField] private UIButton btnExit;
    [SerializeField] private List<GameObject> objShows;
    [SerializeField] private List<GameObject> objHides;
    [SerializeField] private TextMeshProUGUI txtName;
   

    [Header("Skin Change")]
    public InfoSkin infoSkin;

    [SerializeField] private List<TagSkin> tagSkins;
   

    [SerializeField] private List<GameObject> listSkins;
    [SerializeField] private SimpleScrollSnap scrollSnap;
    [SerializeField] private List<TaskSkin> taskSkins;

    private void Start()
    {
        scrollSnap.OnPanelSelected.AddListener((x) => {

            //Debug.LogError("-----" + scrollSnap.CenteredPanel);
            int indexSelect = scrollSnap.CenteredPanel;
            for(int i = 0; i < taskSkins.Count; i++)
            {
                if (i == scrollSnap.CenteredPanel)
                {
                    taskSkins[i].transform.localScale = Vector3.one * 1.2f;

                    if (taskSkins[i].infoSkin.isUnlock)
                        taskSkins[i].Action_selected();
                }
                else
                {
                    taskSkins[i].transform.localScale = Vector3.one;
                }

                
            }

            ShowSkin(taskSkins[indexSelect].infoSkin);
           
        });

        foreach(var k in taskSkins)
        {
            k.CallStart();
        }

        scrollSnap.GoToPanel(0);


    }



    public void CallStart()
    {
        ShowObject(true);
        

        btnExit.SetUpEvent(Action_btnExit);
    }

    public void ShowSkin(InfoSkin _info)
    {
        infoSkin = _info;
        foreach (var k in listSkins)
            k.SetActive(k.name == infoSkin.id_skin);

        txtName.text = _info.name;

        ShowInfoTag();
    }


    public void ShowInfoTag()
    {
        foreach (var k in tagSkins)
            k.gameObject.SetActive(false);
        int _index = 0;
        //Bonus
        if (infoSkin.speed_bonus > 0)
        {
            tagSkins[_index].gameObject.SetActive(true);
            tagSkins[_index].CallStart(ETypeBonus.speed_bonus, infoSkin.speed_bonus);

            _index++;
        }

        if (infoSkin.capaticy_bonus > 0)
        {
            tagSkins[_index].gameObject.SetActive(true);
            tagSkins[_index].CallStart(ETypeBonus.capaticy_bonus, infoSkin.capaticy_bonus);

            _index++;
        }

        if (infoSkin.staff_employ > 0)
        {
            tagSkins[_index].gameObject.SetActive(true);
            tagSkins[_index].CallStart(ETypeBonus.staff_employ, infoSkin.staff_employ);

            _index++;
        }

        if (infoSkin.staff_speed > 0)
        {
            tagSkins[_index].gameObject.SetActive(true);
            tagSkins[_index].CallStart(ETypeBonus.staff_speed, infoSkin.staff_speed);

            _index++;
        }

        if (infoSkin.staff_capacity > 0)
        {
            tagSkins[_index].gameObject.SetActive(true);
            tagSkins[_index].CallStart(ETypeBonus.staff_capacity, infoSkin.staff_capacity);

            _index++;
        }

        if (infoSkin.single_price_bonus > 0)
        {
            tagSkins[_index].gameObject.SetActive(true);
            tagSkins[_index].CallStart(ETypeBonus.single_price_bonus, infoSkin.single_price_bonus);

            _index++;
        }

        if (infoSkin.package_price_bonus > 0)
        {
            tagSkins[_index].gameObject.SetActive(true);
            tagSkins[_index].CallStart(ETypeBonus.package_price_bonus, infoSkin.package_price_bonus);

            _index++;
        }
    }


    private void Action_btnExit()
    {
        ShowObject(false);

        FirebaseManager.Instance.LogEvent_click_button("close_skin");

        DOVirtual.DelayedCall(1, () => MapController.Instance.CheckWorker());
        
    }

    public void ShowObject(bool _isOn = true)
    {
        foreach (var k in objShows)
            k.SetActive(_isOn);

        foreach (var k in objHides)
            k.SetActive(!_isOn);
    }
}
