using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeNet;
using Logic;

public class PacketProcess : CSingletonMonobehaviour<PacketProcess>
{
    delegate void MyDelegate(CPacket msg);

    public NetworkManager network_manager;
    public UIManager ui_manager;
    public MapManager map_manager;
    public GameManager game_manager;
    //public GameObject test;
    Dictionary<PROTOCOL, MyDelegate> Process_Func_Dict;

    public Sprite drawImage;
    void Start()
    {
        this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        this.network_manager.message_receiver = this;

        this.map_manager = GameObject.Find("MapManager").GetComponent<MapManager>();
        this.game_manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Process_Func_Dict = new Dictionary<PROTOCOL, MyDelegate>();
        Process_Func_Dict.Add(PROTOCOL.GAME_START, error);
        Process_Func_Dict.Add(PROTOCOL.LOADING_COMPLETE_NTF, LoadingComplete);
        Process_Func_Dict.Add(PROTOCOL.PLAY_TIME, ShowPlayTime);
        Process_Func_Dict.Add(PROTOCOL.TIME_OVER, TimeOver);
        Process_Func_Dict.Add(PROTOCOL.PLAYER_POSITION_NTF, ReflectPlayerPos);
        Process_Func_Dict.Add(PROTOCOL.USER_EXIT, RemovePlayer);
       Process_Func_Dict.Add(PROTOCOL.SKILL_USE_NTF, SkillUse);
        Process_Func_Dict.Add(PROTOCOL.TELEPORT_NTF, TeleportUse);
        Process_Func_Dict.Add(PROTOCOL.ITEM_USE_NTF, ItemUse);
        Process_Func_Dict.Add(PROTOCOL.TELEPORT_ITEM_NTF, TeleportItemUse);
    }

    public void uiSet()
    {
        this.ui_manager = GameObject.FindGameObjectWithTag("Controller").GetComponentInChildren<UIManager>();

    }

    
    public void on_recv(CPacket msg)
    {
        PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
        Console.WriteLine("protocol id " + protocol);
        Process_Func_Dict[protocol](msg);
    }
    public void error(CPacket msg)
    {

    }

    public void LoadingComplete(CPacket msg)
    {
        ui_manager.popImage();
        GameManager.Instance.isGameStart = true;
    }

    void ShowPlayTime(CPacket msg)
    {
        SoundManager.Instance.PlaySfx(game_manager.AlertSound);
        int playTime = msg.pop_int16();
        ui_manager.popImage();
    }

    void TimeOver(CPacket msg)
    {

        if (UserManager.UserList.Count == 1)
        {
            OnePlayerLeft();
            return;
        }
           


        var AreaColorDict = map_manager.GetColorAreaNum();

        SoundManager.Instance.PlaySfx(game_manager.timeoutSound);

        int tmp_maxColor = 0;
        Common.PlayerColor winColor = Common.PlayerColor.DEFAULT;
        foreach(var color in AreaColorDict)
        {
            if (color.Value >= tmp_maxColor)
            {
                tmp_maxColor = color.Value;
                winColor = color.Key;
            }
               
        }

        foreach (var color in AreaColorDict)
        {
            if (color.Key == winColor)
                continue;

            if (color.Value == AreaColorDict[winColor])
            {
                Draw(AreaColorDict);
                return;
            }

        }



        bool isWin = game_manager.isWin(winColor);
        ui_manager.popWinLostImage(isWin);

        if(isWin)
            SoundManager.Instance.PlaySfx(game_manager.winSound);
        else
            SoundManager.Instance.PlaySfx(game_manager.loseSound);

        float time_span = 0.0f;
        while(time_span < 20.0f)
        {
            time_span += Time.deltaTime;
        }


        CPacket send_msg = CPacket.create((short)PROTOCOL.GAME_RESULT);
        if (isWin)
            send_msg.push((Int16)1);
        else
            send_msg.push((Int16)0);
        network_manager.send(send_msg);

        ui_manager.ShowResult(AreaColorDict);

        InputManager.Instance.isResult = true;

        game_manager.GameSet();

    }

    void ReflectPlayerPos (CPacket msg)
    {
        byte player_index = msg.pop_byte();
        float positionX = msg.pop_float();
        float positionY = msg.pop_float();
        float positionZ = msg.pop_float();
        float speed = msg.pop_float();

        UserManager.Instance.FindPlayer(player_index).Move(positionX, positionY,  positionZ, speed);
    }

    void RemovePlayer (CPacket msg)
    {  
        byte removePlayerIndex = msg.pop_byte();
        ui_manager.PopText(UserManager.Instance.FindPlayer(removePlayerIndex).Id + "가 탈주했습니다!");
        UserManager.Instance.FindPlayer(removePlayerIndex).playerObject.SetActive(false);
        UserManager.Instance.RemovePlayer(removePlayerIndex);
        LogManager.log("Player Exit! : " + removePlayerIndex);
    }

    void SkillUse(CPacket msg)
    {
        byte index = msg.pop_byte();
        UserManager.Instance.FindPlayer(index).UseSkillDirect();
    }

    void TeleportUse(CPacket msg)
    {
        byte index = msg.pop_byte();
        float posX = msg.pop_float();
        float posZ = msg.pop_float();

        UserManager.Instance.FindPlayer(index).skill.ToonBotSkillActive(posX, posZ);
    }

    void ItemUse(CPacket msg)
    {
        byte player_index = msg.pop_byte();
        byte item_index = msg.pop_byte();
        ItemManager.Instance.ItemActive(item_index, player_index);
    }

    void TeleportItemUse(CPacket msg)
    {
        byte player_index = msg.pop_byte();
        byte item_index = msg.pop_byte();
        float posX = msg.pop_float();
        float posZ = msg.pop_float();

        Player player = UserManager.Instance.FindPlayer(player_index);
        player.transform.position = new Vector3(posX, player.transform.position.y, posZ);
        ItemManager.Instance.ItemDisable(item_index);
    }






    void Draw(Dictionary<Common.PlayerColor, int> AreaColorDict)
    {
        ui_manager.PopDrawImage(drawImage);
        

        float time_span = 0.0f;
        while (time_span < 20.0f)
        {
            time_span += Time.deltaTime;
        }


        CPacket send_msg = CPacket.create((short)PROTOCOL.GAME_RESULT);
        send_msg.push((Int16)2);
        network_manager.send(send_msg);

        ui_manager.ShowResult(AreaColorDict);

        InputManager.Instance.isResult = true;

        game_manager.GameSet();
    }

    void OnePlayerLeft()
    {
        ui_manager.popWinLostImage(true);
        
        SoundManager.Instance.PlaySfx(game_manager.winSound);

        float time_span = 0.0f;
        while (time_span < 20.0f)
        {
            time_span += Time.deltaTime;
        }


        CPacket send_msg = CPacket.create((short)PROTOCOL.GAME_RESULT);
        send_msg.push((Int16)1);
        network_manager.send(send_msg);
        

        InputManager.Instance.isResult = true;

        game_manager.GameSet();
    }
}
