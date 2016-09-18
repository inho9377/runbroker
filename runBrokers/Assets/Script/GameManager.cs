using UnityEngine;
using System.Collections;
using Logic;
using System.Collections.Generic;
using FreeNet;
public class GameManager : CSingletonMonobehaviour<GameManager> {
    public float LimitTimeSec;
    


    public Common.PlayerColor winTeamColor;

    public GameObject Player_UNITY_CHAN;
    public GameObject Player_KNIGHT;
    public GameObject Player_TEDDY;
    public GameObject Player_TOON_BOT;

    public GameObject model_Unity_chan;
    public GameObject model_Knight;
    public GameObject model_Teddy;
    public GameObject model_Toonbot;

    public GameObject[] player_models;

    public Dictionary<byte, Vector3> player_start_locatonDict;
    public Dictionary<byte, GameObject> player_modelDict;

    public List<AudioClip> BackgroundSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip gameStartSound;
    public AudioClip AlertSound;
    public AudioClip timeoutSound;

    

    public bool isResult = false;

    public bool isGameStart = false;
	void Start()
    {
        isResult = false;
        SoundManager.Instance.PlaySfx(BackgroundSound[Random.Range(0,BackgroundSound.Count)], true, true);
        SoundManager.Instance.PlaySfx(gameStartSound);
        LimitTimeSec = 60.0f;
        player_models = new GameObject[UserManager.UserList.Count];
        player_start_locatonDict = new Dictionary<byte, Vector3>();
        player_modelDict = new Dictionary<byte, GameObject>();

        SetPlayer();

        CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETE_REQ);
        NetworkManager.Instance.send(msg);
    }

    void SetPlayer()
    {
        int idx = 0;
        List<Vector3> startPosList = MapManager.Instance.GetStartPosition();

        foreach (var player in UserManager.UserList)
        {
            if(player.player_index != UserManager.controller_index)
            {
                player_models[idx] = Instantiate(ReturnPlayerModel(player.playerType)) as GameObject;
                Helper.Instance.CopyComponent<Player>(player, player_models[idx]);
                Vector3 player_startPos = startPosList[UserManager.player_seq[player.player_index]];

                player_start_locatonDict.Add(player.player_index, player_startPos);
                
                idx++;
            }


        }


        var controller = UserManager.Instance.FindController();
        player_models[idx] = Instantiate(ReturnPlayer(controller.playerType)) as GameObject;
        Helper.Instance.CopyComponent<Player>(controller, player_models[idx]);

        Vector3 controller_start_pos = startPosList[UserManager.player_seq[controller.player_index]];

        player_start_locatonDict.Add(controller.player_index, controller_start_pos);



        UserManager.Instance.ClearUserList();
        foreach (var player_model in player_models)
        {
            UserManager.Instance.AddPlayer(player_model.GetComponent<Player>());
            player_model.GetComponent<Player>().playerObject = player_model;
        }

        UserManager.Instance.SetPlayerStartLocation(startPosList);
        
        foreach(var player in UserManager.UserList)
        {
            player.Start();
            
        }
        InputManager.Instance.SetInit();
        PacketProcess.Instance.uiSet();

    }

    GameObject ReturnPlayer(Common.PlayerType type)
    {
        switch(type)
        {
            case Common.PlayerType.TOON_BOT:
                return Player_TOON_BOT;

            case Common.PlayerType.TEDDY:
                return Player_TEDDY;

            case Common.PlayerType.UNITY_CHAN:
                return Player_UNITY_CHAN;

            case Common.PlayerType.KNIGHT:
                return Player_KNIGHT;

            default:
                return null;
        }
    }

    GameObject ReturnPlayerModel(Common.PlayerType type)
    {
        switch (type)
        {
            case Common.PlayerType.TOON_BOT:
                return model_Toonbot;

            case Common.PlayerType.TEDDY:
                return model_Teddy;

            case Common.PlayerType.UNITY_CHAN:
                return model_Unity_chan;

            case Common.PlayerType.KNIGHT:
                return model_Knight;

            default:
                return null;
        }
    }


    public bool isWin(Common.PlayerColor win_team_color)
    {
        if (win_team_color == UserManager.Instance.FindController().playerColor)
            return true;
        else
            return false;
    }

    public void GameSet()
    {
        isResult = true;
        UserManager.UserList.Insert(0, UserManager.Instance.FindController());
        UserManager.UserList.RemoveAt(UserManager.UserList.Count-1);
        isGameStart = false;
    }

   
}
