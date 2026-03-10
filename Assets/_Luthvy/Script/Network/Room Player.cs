using Mirror;
using UnityEngine;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public string playerName;

    public override void OnClientEnterRoom()
    {
        if (LobbyMenuNet.Instance != null)
            LobbyMenuNet.Instance.Refresh();
    }
    
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (LobbyMenuNet.Instance != null)
            LobbyMenuNet.Instance.Refresh();
    }

}
