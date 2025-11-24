using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class SimpleMultiplayer : MonoBehaviourPunCallbacks
{
    public string roomName = "MainRoom";   // All players go here
    public string playerPrefab = "Player"; // Must be in Resources folder

    void Start()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    // --------------------------
    // Connection Callbacks
    // --------------------------
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master server!");

        // join or create shared room
        PhotonNetwork.JoinOrCreateRoom(
            roomName,
            new RoomOptions { MaxPlayers = 20 },
            TypedLobby.Default
        );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + roomName);

        // spawn the player
        PhotonNetwork.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }

    // --------------------------
    // Leave Room
    // --------------------------
    public void LeaveRoom()
    {
        Debug.Log("Leaving room...");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room!");

        // load your menu scene
        SceneManager.LoadScene("MainMenu");
    }

    // --------------------------
    // Disconnect callback
    // --------------------------
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected: " + cause);

        // back to menu
        SceneManager.LoadScene("MainMenu");
    }
}
