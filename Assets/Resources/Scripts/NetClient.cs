using UnityEngine;
using System.Collections;
using System;

public class NetClient : NetBase
{
    void OnEnable()
    {
        MessageManager.GetInstance().Add<InfoParam2<string, string>>("net_player_get", OnNetPlayerGet, gameObject);
    }

    void OnNetPlayerGet(InfoParam2<string, string> param) {
        var player = new ProtocolOcean.Player() { Name = "name", Id = 1, Level = 2 };
        MessageManager.GetInstance().Notify("net_player_update", player);
    }
}
