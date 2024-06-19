using System;
using System.Collections;
using System.Collections.Generic;
using ThePattern;
using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerCtrl : UnitBase
{
    
    [SerializeField] private float scaleAnim = 1;
    [SerializeField] private float alphaSub = 10;
    public PlayerData m_PlayerData => MapController.Instance.mapData.playerData;

    [Space, Header("PLAYER UNIT - DATA")]
    [SerializeField] private float _gravity;
    [SerializeField] private Animator _animator;
    //[SerializeField] private PlayerSkinController _skinController;

    [SerializeField] private GameObject _fxRunSmoke;

    private CharacterController m_characterCtrl;
    private PlayerPickup m_playerPickup;

    private DataBind<Vector2> _inputDir = new DataBind<Vector2>(Vector2.zero);
    private Vector3 _moveDir = Vector3.zero;
    private Vector3 _rotDir = Vector3.zero;

    [SerializeField] private EPlayer_State _state;
    public EPlayer_State State
    {
        get { return _state; }
        set {
            if (_state == value)
                return;

            _state = value ;
            switch (_state)
            {
                case EPlayer_State.Idle:
                    _animator.SetBool("Run",false);
                    break;
                case EPlayer_State.Run:
                    _animator.SetBool("Run", true);
                    _animator.speed = scaleAnim + m_PlayerData.Speed / alphaSub;
                    _animator.SetFloat("speedRun", m_PlayerData.Speed);
                    break;
                case EPlayer_State.Work:
                    _animator.SetBool("Work",true);
                    break;
                case EPlayer_State.Dancer:
                    break;
                case EPlayer_State.Segway:
                    break;
                default:
                    break;
            }

        }
    }


    bool isCanMove;
    // Start is called before the first frame update
    void Start()
    {
        //LoadPos();
        m_characterCtrl = GetComponent<CharacterController>();
        m_playerPickup = GetComponent<PlayerPickup>();

        MapController.Instance.OnChangedInput += v =>
        {
            _inputDir.Value = v;

        };

        DOVirtual.DelayedCall(1.5f, () => {
            _fxRunSmoke.SetActive(true);
            isCanMove = true;
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCanMove)
            return;

        if (m_characterCtrl.isGrounded)
        {
            _moveDir = new Vector3(_inputDir.Value.x, 0, _inputDir.Value.y);

            if (_moveDir != Vector3.zero)
            {
                State = EPlayer_State.Run;
            }
            else
            {
                State = EPlayer_State.Idle;
            }

            //_animator.SetBool("Pick", PlayerPickUp.ProductUnits.Count > 0);
            _fxRunSmoke.SetActive(_moveDir != Vector3.zero);

        }
        _moveDir.y += _gravity * Time.deltaTime;
        _rotDir = new Vector3(_inputDir.Value.x, 0, _inputDir.Value.y);

        if (m_UnitData != null)
        {
            Move(_moveDir, _rotDir, m_PlayerData.Speed);

            if (transform.position.y < -0.6f)
            {
                //Debug.LogError("block");
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }
    }
    public bool IsMoving()
    {
        if (_moveDir != Vector3.zero && m_characterCtrl.velocity.magnitude > 0)
        {
            _animator.SetBool("isPay", false);
            return true;
        }
        return false;
    }

    public void Move(Vector3 moveDir, Vector3 rotDir, float speed)
    {
        if (!m_characterCtrl) return;

        Vector3 moveDirection = moveDir.normalized;
        //_animator.SetFloat("speedRun", speed);
        float angleInDegrees = 45;
        moveDirection = Quaternion.Euler(0, angleInDegrees, 0) * moveDirection;
       
        m_characterCtrl.Move(moveDirection * speed * Time.deltaTime);

        rotDir = Quaternion.Euler(0, angleInDegrees,0 ) * rotDir;
        Quaternion targetRot = rotDir != Vector3.zero ? Quaternion.LookRotation(rotDir) : transform.rotation;
        this.transform.rotation = targetRot;
    }
    public void PayState(bool isPay)
    {
        _animator.SetBool("isPay", isPay);
        if(!isPay)
            UIMainGame.Instance.Hide_MoneyEffect();
    }
    public void OnChangedSkin(ETypeSkin currentSkin)
    {
        //_skinController.ChangeSkin(currentSkin);
    }
    public void CarryState(int _value = 0)
    {
        _animator.SetInteger("Carry", _value);
    }

    [SerializeField] private TextMeshPro txtMoney;
    Tween twMoney;
    public void ShowPickCash(int _value)
    {
        txtMoney.gameObject.SetActive(true);
        txtMoney.text = "+" + _value + "$";

        if (twMoney != null)
            twMoney.Kill();
        twMoney = DOVirtual.DelayedCall(1, () => {
            //txtMoney.DOFade(0, 1);
            txtMoney.gameObject.SetActive(false);
        });
    }

    [SerializeField] private ParticleSystem fxDoor;
    public void ShowAnimWellcome()
    {
        _animator.SetBool("isWelcome", true);

        DOVirtual.DelayedCall(1, () => {
            fxDoor?.gameObject.SetActive(true);
            fxDoor?.Play();
        });
       
    }

    public void HideAnimWellcome()
    {
        _animator.SetBool("isWelcome", false);
    }

}

public enum EPlayer_State
{
    Idle,
    Run,
    Work,
    Dancer,
    Segway
}

[System.Serializable]
public class PlayerData : UnitData
{
    //public float Speed =>GameManager.Instance.m_DataConfigRemote.speedBonus +
    //    (MapController.Instance.speedBooster?MapController.Instance.mapData.upgradePlayer.crSpeed().value*1.8f 
    //    : MapController.Instance.mapData.upgradePlayer.crSpeed().value);
    public float Speed
    {
        get
        {
            float _value = GameManager.Instance.m_DataConfigRemote.speedBonus 
                + MapController.Instance.mapData.upgradePlayer.crSpeed().value 
                + GameManager.Instance.skinModel.infoSkin.speed_bonus;

            if (MapController.Instance.speedBooster)
                _value *= 1.8f;

            return _value;
        }
    }

    //public int CarryStack => (int)(MapController.Instance.stackBooster?MapController.Instance.mapData.upgradePlayer.crCapacity().value*1.8f
    //    : MapController.Instance.mapData.upgradePlayer.crCapacity().value);

    public int CarryStack
    {
        get
        {
            int _value = (int)(MapController.Instance.mapData.upgradePlayer.crCapacity().value + GameManager.Instance.skinModel.infoSkin.capaticy_bonus);

            if (MapController.Instance.stackBooster)
                _value = (int)(_value*1.8f);

            return _value;
        }
    }
}