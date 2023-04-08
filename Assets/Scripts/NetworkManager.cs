using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Serializable Fields
    [Header("Panels")]
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject connectionStatusPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject roomPanel;
    [Header("RoomPanel Items")]
    [SerializeField] GameObject playersList;
    [SerializeField] GameObject playerFieldPrefab;
    #endregion

    #region Unity Methods
    private void Start()
    {
        loginPanel.SetActive(true);
        connectionStatusPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }
    #endregion

    #region Private Methods
    void CreateAndJoinRoom()
    {
        string roomName = "Room" + Random.Range(1000, 9999);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion

    #region UI Callbacks

    public void OnClick_ConnectToServer(TMP_InputField nameText)
    {
        PhotonNetwork.NickName = nameText.text;
        PhotonNetwork.ConnectUsingSettings();
        loginPanel.SetActive(false);
        connectionStatusPanel.SetActive(true);
    }

    public void OnClick_JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClick_Logout()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to the internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the server using nickname" + PhotonNetwork.NickName);
        connectionStatusPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server: " + cause);
        lobbyPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("You joined room" + PhotonNetwork.CurrentRoom.Name);
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if(player.IsMasterClient)
            {
                GameObject playerField = Instantiate(playerFieldPrefab, playersList.transform);
                playerField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;
                playerField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                GameObject playerField = Instantiate(playerFieldPrefab, playersList.transform);
                playerField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;
                playerField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("You have left the room");
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        foreach(Transform playerEntry in playersList.transform)
        {
            Destroy(playerEntry.gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " Joined the room");
        GameObject playerField = Instantiate(playerFieldPrefab, playersList.transform);
        playerField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " Left the room");
        foreach(Transform child in playersList.transform)
        {
            if(child.GetChild(0).GetComponent<TextMeshProUGUI>().text == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room: " + message);
        CreateAndJoinRoom();
    }

    

    

    #endregion
}
