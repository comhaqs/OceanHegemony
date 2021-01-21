using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiListAction : MonoBehaviour
{
    public Button move;

    private void OnEnable()
    {
        MessageManager.GetInstance().Add<bool>("ui_action_move_update", OnActionMoveUpdate, gameObject);

        move.onClick.AddListener(()=> {
            MessageManager.GetInstance().Notify("action_move");
        });
    }

    void OnActionMoveUpdate(bool flag) {
        move.gameObject.SetActive(flag);
    }
}
