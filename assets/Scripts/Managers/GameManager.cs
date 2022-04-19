using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public Player[]  players;

    public UnityEvent GameOver;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple GameManagers in scene.");
        }
        players = PhotonNetwork.PlayerList;
    }

    // Update is called once per frame
    void Update()
    {
        //string printer = "";
        //foreach(Player player in PhotonNetwork.PlayerList)
        //{
        //    if (!player.CustomProperties.ContainsKey("Alive")) continue;
        //    bool isAlive = (bool)player.CustomProperties["Alive"];

        //    printer += player.NickName +": alive: " + isAlive + ". ";
        //}
        //Debug.Log(printer);
    }

    public void MarkPlayerAsDead()
    {
        var properties = PhotonNetwork.LocalPlayer.CustomProperties;
        properties["Alive"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        if(AreAllPlayersDead())
        {
            EndGame();
        }
    }

    public void MarkPlayerAsAlive()
    {
        var properties = PhotonNetwork.LocalPlayer.CustomProperties;
        properties["Alive"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        players = PhotonNetwork.PlayerList;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        players = PhotonNetwork.PlayerList;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }

    private void EndGame()
    {
        GameOver.Invoke();
        LeaveRoom();
    }

    private bool AreAllPlayersDead()
    {
        foreach(Player player in players)
        {
            try
            {
                bool isAlive = (bool)player.CustomProperties["Alive"];
                if (isAlive) return false;
            }catch(KeyNotFoundException)
            {
                Debug.LogError("Player did not have Alive property!");
            }
        }
        return true;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (AreAllPlayersDead())
        {
            EndGame();
        }
    }
}
