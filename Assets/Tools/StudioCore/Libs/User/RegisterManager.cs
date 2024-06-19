using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class RegisterManager : MonoBehaviour
{
    [SerializeField]
    private InputField txtUsername;
    [SerializeField]
    private InputField txtPw;
    [SerializeField]
    private InputField txtEmail;
    [SerializeField]
    private InputField txtPhone;
    [SerializeField]
    private Button btOk;
    [SerializeField]
    private Button btCancel;

    // Start is called before the first frame update
    void Start()
    {
        btOk.OnClickAsObservable().Subscribe(_ => {
            Debug.Log(string.Format("{0}{1}{2}{3}", txtUsername.text, txtPw.text, txtEmail.text, txtPhone.text));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
