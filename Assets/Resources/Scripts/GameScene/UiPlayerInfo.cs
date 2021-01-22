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
        var hp_rate = 1.0f * person.hp / person.hp_max;
        hp_rate = Mathf.Min(1.0f, hp_rate);
        hp_rate = Mathf.Max(0.0f, hp_rate);
        OnPlayerHpUpdate(hp_rate);
        var mp_rate = 1.0f * person.mp / person.mp_max;
        mp_rate = Mathf.Min(1.0f, mp_rate);
        mp_rate = Mathf.Max(0.0f, mp_rate);
        OnPlayerMpUpdate(mp_rate);
    }

    void OnPlayerHpUpdate(float rate) {
        rate = Mathf.Min(1.0f, rate);
        rate = Mathf.Max(0.0f, rate);
        hp.fillAmount = rate;
    }
    void OnPlayerMpUpdate(float rate)
    {
        rate = Mathf.Min(1.0f, rate);
        rate = Mathf.Max(0.0f, rate);
        mp.fillAmount = rate;
    }
}
