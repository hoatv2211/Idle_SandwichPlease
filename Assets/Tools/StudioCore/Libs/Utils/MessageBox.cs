using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

namespace NekoMart.Utils
{
    public static class MessageBox
    {
        public static void ShowOkOnly(System.Action onOk, string content)
        {
            ModalObject.Popup(
                ModalCreatorPrefabsName.PopupOkNew,
                "",
                content,
                "Got it",
                "",
                onOK: onOk
            );

        }
    }

}