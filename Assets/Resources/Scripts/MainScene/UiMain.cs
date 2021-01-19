using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiMain : MonoBehaviour
{
    public Text ui_name;
    public Text ui_level;
    void Start()
    {
        MessageManager.GetInstance().Notify("net_player_get", new InfoParam2<string, string>() { param1 = "name", param2 = "password" });
    }

    public void OnClickMultiPlayer() {
        SceneManager.LoadScene("GameScene");
    }

    void OnNetPlayerUpdate(ProtocolOcean.Player player) {
        ui_name.text = player.Name;
        ui_level.text = player.Level.ToString();
    }
}
