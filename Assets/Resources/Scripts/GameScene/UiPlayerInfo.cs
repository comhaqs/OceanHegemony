using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UiPlayerInfo : MonoBehaviour
{
    public Image header;
    public Text nm;
    public Image hp;
    public Image mp;

    private void OnEnable()
    {
        MessageManager.GetInstance().Add<Person>("ui_player_info_update", OnPlayerInfoUpdate, gameObject);
        MessageManager.GetInstance().Add<float>("ui_player_hp_update", OnPlayerHpUpdate, gameObject);
        MessageManager.GetInstance().Add<float>("ui_player_mp_update", OnPlayerMpUpdate, gameObject);
    }

    void OnPlayerInfoUpdate(Person person)
    {
        nm.text = person.nm;
        OnPlayerHpUpdate(1.0f * person.hp / person.hp_max);
        OnPlayerMpUpdate(1.0f * person.mp / person.mp_max);
    }

    void OnPlayerHpUpdate(float rate) {
        hp.fillAmount = rate;
    }
    void OnPlayerMpUpdate(float rate)
    {
        mp.fillAmount = rate;
    }
}
