using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FreeNet;
using Logic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitRoom : CSingletonMonobehaviour<WaitRoom> {


    public List<GameObject> playerColors;
    public List<GameObject> playerColorTexts;
    public List<GameObject> playerTypeTexts;
    public List<GameObject> playerTypes;
    public List<GameObject> playerIdTexts;
    public List<GameObject> playerReadyTexts;

    public Dictionary<byte, GameObject> playerColorDict;
    public Dictionary<byte, GameObject> playerColorTextDict;
    public Dictionary<byte, GameObject> playerTypeDict;
    public Dictionary<byte, GameObject> playerTypeTextDict;
    public Dictionary<byte, GameObject> playerTextDict;
    public Dictionary<byte, GameObject> playerReadyImageDict;

    public Dictionary<byte, int> playerUiSeqDict;

    public RenderTexture UNITY_CHAN_image;
    public RenderTexture TOON_BOT_image;
    public RenderTexture TEDDY_image;
    public RenderTexture KNIGHT_image;

    public GameObject unitychan_model;
    public GameObject knight_model;
    public GameObject toonbot_model;
    public GameObject teddy_model;

    public Sprite unitychanTextSprite;
    public Sprite knightTextSprite;
    public Sprite toonbotTextSprite;
    public Sprite teddyTextSprite;
    
    public Sprite RedTeamTextSprite;
    public Sprite GreenTeamTextSprite;
    public Sprite YellowTeamTextSprite;
    public Sprite PURPLETeamTextSprite;
    public Sprite WhiteTeamTextSprite;

    public RenderTexture unitychan_sprite;
    public RenderTexture knight_sprite;
    public RenderTexture toonbot_sprite;
    public RenderTexture teddy_sprite;

    public AudioClip BackgroundSound;
    public AudioClip ButtonClickSound;
    public AudioClip ReadySound;

    //public AudioClip KnightReadySound;

    public rulebook ruleBook;
    


    public Material default_material;
    public Sprite default_sprite;
    //bool isReady = false;

    int ui_index = 0;
    List<int> empty_ui_seq = new List<int>();
    public NetworkManager network_manager;

    public Image loadingImage;
    
    void Start()
    {
        SoundManager.Instance.PlaySfx(BackgroundSound, true, true);

        loadingImage.gameObject.SetActive(false);
        this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        this.network_manager.message_receiver = this;
        playerColors.AddRange(GameObject.FindGameObjectsWithTag("color"));
        playerTypes.AddRange(GameObject.FindGameObjectsWithTag("type"));
        playerIdTexts.AddRange(GameObject.FindGameObjectsWithTag("idText"));
        playerReadyTexts.AddRange(GameObject.FindGameObjectsWithTag("ready"));
        playerColorTexts.AddRange(GameObject.FindGameObjectsWithTag("color_text"));
        playerTypeTexts.AddRange(GameObject.FindGameObjectsWithTag("type_text"));

        playerColorDict = new Dictionary<byte, GameObject>();
        playerTypeDict = new Dictionary<byte, GameObject>();
        playerTextDict = new Dictionary<byte, GameObject>();
        playerReadyImageDict = new Dictionary<byte, GameObject>();
        playerColorTextDict = new Dictionary<byte, GameObject>();
        playerTypeTextDict = new Dictionary<byte, GameObject>();

          playerUiSeqDict = new Dictionary<byte, int>();
        
        foreach (Player user in UserManager.UserList)
        {
            playerColorDict.Add(user.player_index, playerColors[ui_index]);
            playerTypeDict.Add(user.player_index, playerTypes[ui_index]);
            playerTextDict.Add(user.player_index, playerIdTexts[ui_index]);
            playerReadyImageDict.Add(user.player_index, playerReadyTexts[ui_index]);
            playerColorTextDict.Add(user.player_index, playerColorTexts[ui_index]);
            playerTypeTextDict.Add(user.player_index, playerTypeTexts[ui_index]);


            playerUiSeqDict.Add(user.player_index, ui_index);

            PlayerColorChange(user.player_index, UserManager.UserList[ui_index].playerColor);
            PlayerTypeChange(user.player_index, UserManager.UserList[ui_index].playerType);
            PlayerTextChange(user.player_index);

            if (user.isReady == 1)
                playerReadyChange(user.player_index);
            else
                playerReadyImageDict[user.player_index].SetActive(false);

            ui_index++;
        }

       
       
    }


    public void ClickHelp()
    {
        ruleBook.gameObject.SetActive(true);
    }
 
    

    public void ClickPlayerColorChange()
    {
        SoundManager.Instance.PlaySfx(ButtonClickSound);
        if (UserManager.Instance.FindController().isReady == 1)
            return;

        Common.PlayerColor next_color = Helper.Instance.nextColor(UserManager.Instance.FindController().playerColor);
        CPacket msg = CPacket.create((short)PROTOCOL.COLOR_CHANGE_REQ);
        msg.push((byte)next_color);
        network_manager.send(msg);
    }

    void PlayerColorChange(byte player_index, Common.PlayerColor color)
    {
        GameObject colorM = playerColorDict[player_index];
        UserManager.Instance.FindPlayer(player_index).playerColor = color;

        colorM.GetComponent<MeshRenderer>().material.color = Helper.Instance.ReturnColor( color);
        
        playerColorTextDict[player_index].GetComponent<Image>().sprite = ReturnColorTextSprite(color);
    }

    public void ClickPlayerTypeChange()
    {
        SoundManager.Instance.PlaySfx(ButtonClickSound);
        if (UserManager.Instance.FindController().isReady == 1)
            return;
        Common.PlayerType next_type = Helper.Instance.nextType(UserManager.Instance.FindController().playerType);
        CPacket msg = CPacket.create((short)PROTOCOL.TYPE_CHANGE_REQ);
        msg.push((byte)next_type);
        network_manager.send(msg);

        SoundManager.Instance.PlayCharSound(next_type);
    }

    void AnimationStart(Common.PlayerType type)
    {
        ReturnTypeModel(type).GetComponent<Animator>().SetTrigger("Idle");
        ReturnTypeModel(type).GetComponent<Animator>().SetTrigger("Wait");
        ReturnTypeModel(type).GetComponent<Animator>().SetTrigger("Idle");
    }
    
    GameObject ReturnTypeModel(Common.PlayerType type)
    {
        switch(type)
        {
            case Common.PlayerType.TOON_BOT:
                return toonbot_model;

            case Common.PlayerType.UNITY_CHAN:
                return unitychan_model;

            case Common.PlayerType.KNIGHT:
                return knight_model;

            case Common.PlayerType.TEDDY:
                return teddy_model;

        }

        return teddy_model;
    }

    void PlayerTypeChange(byte player_index, Common.PlayerType type)
    {
        GameObject imageM = playerTypeDict[player_index];
        UserManager.Instance.FindPlayer(player_index).playerType = type;

        imageM.GetComponentInChildren<MeshRenderer>().material.mainTexture = ReturnTypeImage(type);
        AnimationStart(type);
        playerTypeTextDict[player_index].GetComponent<Image>().sprite = ReturnTypeTextSprite(type);
    }

    void PlayerTextChange(byte player_index)
    {
        GameObject textM = playerTextDict[player_index];
        textM.GetComponent<Text>().text = UserManager.Instance.FindPlayer(player_index).Id;
    }


    public void ClickReady()
    {
        if (UserManager.Instance.FindController().isReady == 1)
            return;
        SoundManager.Instance.PlaySfx(ReadySound);
        CPacket msg = CPacket.create((short)PROTOCOL.READY_REQ);
        msg.push(UserManager.controller_index);
        network_manager.send(msg);
    }

    void playerReadyChange(byte player_index)
    {
        playerReadyImageDict[player_index].SetActive(true);
    }

    void SetOtherPlayerUI(Player user)
    {
        int index;

        if(empty_ui_seq.Count != 0)
        {
            empty_ui_seq.Sort();
            index = empty_ui_seq[0];
            
        }
        else
        {
            index = UserManager.UserList.Count - 1;
        }


        playerUiSeqDict.Add(user.player_index, index);
        playerColorDict.Add(user.player_index, playerColors[index]);
        playerTypeDict.Add(user.player_index, playerTypes[index]);
        playerTextDict.Add(user.player_index, playerIdTexts[index]);
        playerColorTextDict.Add(user.player_index, playerColorTexts[index]);
        playerTypeTextDict.Add(user.player_index, playerTypeTexts[index]);
        playerReadyImageDict.Add(user.player_index, playerReadyTexts[index]);

        PlayerColorChange(user.player_index, UserManager.UserList[index].playerColor);
        PlayerTypeChange(user.player_index, UserManager.UserList[index].playerType);
        PlayerTextChange(user.player_index);
        playerReadyImageDict[user.player_index].SetActive(false);

        //ui_index++;
        //Todo : REady 처리
    }

    void RemovePlayer(byte player_index)
    {
        playerColorDict.Remove(player_index);
        playerColorTextDict.Remove(player_index);
        playerTextDict.Remove(player_index);
        playerTypeDict.Remove(player_index);
        playerTypeTextDict.Remove(player_index);

        int index = playerUiSeqDict[player_index];
        UiReset(index);
        empty_ui_seq.Add(index);
        playerUiSeqDict.Remove(player_index);
        
        playerReadyChange(player_index);
        playerReadyImageDict.Remove(player_index);

        UserManager.Instance.RemovePlayer(player_index);
        
    }

    void UiReset(int index)
    {

        playerColors[index].GetComponentInChildren<MeshRenderer>().material = default_material;
        playerColorTexts[index].GetComponent<Image>().sprite = default_sprite;
        playerIdTexts[index].GetComponent<Text>().text = "New Text";
        playerTypes[index].GetComponentInChildren<MeshRenderer>().material = default_material; 
        playerTypeTexts[index].GetComponent<Image>().sprite = default_sprite;
        playerReadyTexts[index].SetActive(false);

    }

    RenderTexture ReturnTypeImage(Common.PlayerType type)
    {
        switch(type)
        {
            case Common.PlayerType.TOON_BOT:
                 return TOON_BOT_image;

            case Common.PlayerType.UNITY_CHAN:
                return UNITY_CHAN_image;

            case Common.PlayerType.KNIGHT:
                return KNIGHT_image;

            case Common.PlayerType.TEDDY:
                return TEDDY_image;
            
        }

        return TEDDY_image;
    }

    Sprite ReturnColorTextSprite(Common.PlayerColor color)
    {
        switch(color)
        {
            case Common.PlayerColor.PURPLE:
                return PURPLETeamTextSprite;

            case Common.PlayerColor.WHITE:
                return WhiteTeamTextSprite;

            case Common.PlayerColor.GREEN:
                return GreenTeamTextSprite;

            case Common.PlayerColor.RED:
                return RedTeamTextSprite;

            case Common.PlayerColor.YELLOW:
                return YellowTeamTextSprite;


        }
        return PURPLETeamTextSprite;
    }

    Sprite ReturnTypeTextSprite(Common.PlayerType type)
    {
        switch (type)
        {
            case Common.PlayerType.KNIGHT:
                return knightTextSprite;

            case Common.PlayerType.TEDDY:
                return teddyTextSprite;

            case Common.PlayerType.TOON_BOT:
                return toonbotTextSprite;

            case Common.PlayerType.UNITY_CHAN:
                return unitychanTextSprite;
                


        }
        return unitychanTextSprite;
    }

    public void on_recv(CPacket msg)
    {
        PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

        switch (protocol_id)
        {
            case PROTOCOL.LOGIN_NTF:
                {
                    byte index = msg.pop_byte();
                    string id = msg.pop_string();
                    Player player = new Player();
                    player.player_index = index;
                    player.Id = id;
                    UserManager.Instance.AddPlayer(player);
                    SetOtherPlayerUI(player);
                    break;
                }

            case PROTOCOL.TYPE_CHANGE_RES:
                {

                    break;
                }

            case PROTOCOL.TYPE_CHANGE_NTF:
                {
                    byte index = msg.pop_byte();
                    Common.PlayerType type = (Common.PlayerType)msg.pop_byte();
                    PlayerTypeChange(index, type);
                    break;
                }

            case PROTOCOL.COLOR_CHANGE_RES:
                {

                    break;
                }

            case PROTOCOL.COLOR_CHANGE_NTF:
                {
                    byte index = msg.pop_byte();
                    Common.PlayerColor color = (Common.PlayerColor)msg.pop_byte();
                    PlayerColorChange(index, color);
                    break;
                }

            case PROTOCOL.READY_RES:
                {
                    playerReadyChange(UserManager.controller_index);
                    UserManager.Instance.FindController().isReady = 1;
                    break;
                }

            case PROTOCOL.READY_NTF:
                {
                    byte index = msg.pop_byte();
                    playerReadyChange(index);
                    UserManager.Instance.FindPlayer(index).isReady = 1;
                    break;
                }

            case PROTOCOL.USER_EXIT:
                {
                    byte index = msg.pop_byte();
                    LogManager.log(index + " exit");
                    RemovePlayer(index);
                    break;
                }

            case PROTOCOL.GAME_START:
                {
                    List<byte> index_seq = new List<byte>();
                    for(int i=0; i<UserManager.UserList.Count; i++)
                    {
                        index_seq.Add(msg.pop_byte());
                        
                    }


                    UserManager.Instance.SetPlayerSeq(index_seq);
                    LogManager.log("GAME_START!_!_!");
                    foreach (Player user in UserManager.UserList)
                    {
                        int index = playerUiSeqDict[user.player_index];
                        UiReset(index);
                        user.isReady = 0;
                    }
                    loadingImage.gameObject.SetActive(true);
                    SceneManager.LoadScene("game");
                    
           
                    break;
                }


            default: break;
        }



    }



}
