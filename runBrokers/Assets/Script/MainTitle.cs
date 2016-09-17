using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FreeNet;
using Logic;
using System;
using UnityEngine.SceneManagement;

public class MainTitle : CSingletonMonobehaviour<MainTitle>
{
    
    public Texture bg;

    public NetworkManager network_manager;

    public AudioClip titleBackgroundSound;
    public AudioClip ButtonClickSound;
    public AudioClip RegisterSuccessSound;
    public AudioClip RegisterErrorSound;
    

    public Texture waiting_img;

    public string inputId = "a";
    public string inputPwd = "1111";
    public string inputIP = "127.0.0.1";
    public string inputPort = "7979";

    public GameObject connectBtn;
    public GameObject ipInputField;

    public GameObject loginBtn;
    public GameObject registerBtn;
    public GameObject idInputField;
    public GameObject passwordInputField;

    public GameObject popText;

    public bool isLoginScene = false;


    // Use this for initialization
    void Start()
    {
        Console.WriteLine("start");
        this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        LogManager.log("TitleStart");

        loginBtn.SetActive(false);
        registerBtn.SetActive(false);
        idInputField.SetActive(false);
        passwordInputField.SetActive(false);

        this.network_manager.message_receiver = this;

        SoundManager.Instance.PlaySfx(titleBackgroundSound, true);
        //network_manager.connect("127.0.0.1", "7979");
        idInputField.GetComponent<InputField>().ActivateInputField();

        /*
        this.user_state = USER_STATE.NOT_CONNECTED;
        //this.bg = Resources.Load("images/title_blue") as Texture;
        //this.battle_room = GameObject.Find("BattleRoom").GetComponent<BattleRoom>();
        //this.battle_room.gameObject.SetActive(false);

        this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        //this.waiting_img = Resources.Load("images/waiting") as Texture;
        LogManager.log("TitleStart");

        this.user_state = USER_STATE.NOT_CONNECTED;
        //enter();
  */
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isLoginScene)
        {
            ipInputField.GetComponent<InputField>().ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && !isLoginScene)
        {
            ConnectButton();
        }


        if (Input.GetKeyDown(KeyCode.Tab) && isLoginScene)
        {
            passwordInputField.GetComponent<InputField>().ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && isLoginScene)
        {
            LoginButton();
        }

        if (!isLoginScene && network_manager.is_connected())
            ConnectSuccess();
    }


    public void enterWaitRoom()
    {
        //StopCoroutine("after_connected");
        LogManager.log("id : " + inputId);
        LogManager.log("ip : " + inputIP);
        LogManager.log("Port : " + inputPort);
        this.network_manager.message_receiver = this;

        CPacket msg = CPacket.create((short)PROTOCOL.LOGIN_REQ);
        msg.push(inputId);
        network_manager.send(msg);

    }
    /*
    /// <summary>
    /// 서버에 접속된 이후에 처리할 루프.
    /// 마우스 입력이 들어오면 ENTER_GAME_ROOM_REQ프로토콜을 요청하고 
    /// 중복 요청을 방지하기 위해서 현재 코루틴을 중지 시킨다.
    /// </summary>
    /// <returns></returns>
    IEnumerator after_connected()
    {
        // CBattleRoom의 게임오버 상태에서 마우스 입력을 통해 메인 화면으로 넘어오도록 되어 있는데,
        // 한 프레임 내에서 이 코루틴이 실행될 경우 아직 마우스 입력이 남아있는것으로 판단되어
        // 메인 화면으로 돌아오자 마자 ENTER_GAME_ROOM_REQ패킷을 보내는 일이 발생한다.
        // 따라서 강제로 한 프레임을 건너뛰어 다음 프레임부터 코루틴의 내용이 수행될 수 있도록 한다.
        yield return new WaitForEndOfFrame();

        while (true)
        {
            /*if (this.user_state == USER_STATE.CONNECTED)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    this.user_state = USER_STATE.WAITING_MATCHING;

                    CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
                    this.network_manager.send(msg);

                 //   StopCoroutine("after_connected");
                }
            }

            yield return 0;
        }
    }*/


    public void IpInput(string IP)
    {
       
        inputIP = IP;
        LogManager.log("input" + inputIP);
    }
    
    public void IdInput(string ID)
    {
        LogManager.log("input" + ID);
        inputId = ID;
    }
    public void PwdInput(string pwd)
    {
        LogManager.log("input" + pwd);
        inputPwd = pwd;
    }


    public void ConnectButton()
    {
        SoundManager.Instance.PlaySfx(ButtonClickSound);
       // if (this.network_manager.is_connected())
         //   return;
        //if (isConnect)
          //  return;
        if (!this.network_manager.is_connected())
        {
            if (!this.network_manager.connect(inputIP, inputPort))
                return;
            
        }

        if(network_manager.is_connected())
            ConnectSuccess();
        

    }

    void ConnectSuccess()
    {
        LogManager.log("connect!");

        connectBtn.SetActive(false);
        ipInputField.SetActive(false);


        idInputField.SetActive(true);
        passwordInputField.SetActive(true);
        loginBtn.SetActive(true);
        registerBtn.SetActive(true);

        isLoginScene = true;
        //enterWaitRoom();
    }

    public void LoginButton()
    {
        SoundManager.Instance.PlaySfx(ButtonClickSound);
        CPacket send_msg = CPacket.create((short)PROTOCOL.LOGIN_REQ);
        send_msg.push(inputId);
        send_msg.push(inputPwd);
        network_manager.send(send_msg);
    }

    public void RegisterButton()
    {
        SoundManager.Instance.PlaySfx(ButtonClickSound);
        CPacket send_msg = CPacket.create((short)PROTOCOL.REGISTER_REQ);
        send_msg.push(inputId);
        send_msg.push(inputPwd);
        network_manager.send(send_msg);
    }

    
    /// <summary>
    /// 서버에 접속이 완료되면 호출됨.
    /// </summary>
    public void on_connected()
    {
     //   enterWaitRoom();
    }


    public void MuteButton()
    {
        SoundManager.Instance.Pause();
    }

    /// <summary>
    /// 패킷을 수신 했을 때 호출됨.
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="msg"></param>
    public void on_recv(CPacket msg)
    {
        // 제일 먼저 프로토콜 아이디를 꺼내온다.
        PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

        switch (protocol_id)
        {
            case PROTOCOL.CONNECT_SUCCESS:
                {
                    ConnectSuccess();
                    break;
                }

            case PROTOCOL.FULL_ROOM:
                {
                    popText.GetComponent<PopUp>().Pop("FULL ROOM!!", 1.0f);
                    break;
                }

            case PROTOCOL.LOGIN_RES:
                {
                    byte index = msg.pop_byte();
                    int player_number = msg.pop_int16();
                    UserManager.Instance.AddPlayer(index, inputId);
                    UserManager.controller_index = index;
                    //Todo : I는 처음 부분에 저장되나?

                    for (int i=0; i<player_number; i++)
                    {
                        
                        Player player = new Player();
                        player.player_index = msg.pop_byte();
                        player.Id = msg.pop_string();
                        player.playerColor = (Common.PlayerColor)msg.pop_byte();
                        player.playerType = (Common.PlayerType)msg.pop_byte();
                        player.isReady = msg.pop_int16();
                        UserManager.Instance.AddPlayer(player);
                        
                        LogManager.log("player Info : " + player.player_index + " ,  " + player.Id + 
                            " , color : " + player.playerColor + " , type : " + player.playerType);

                    }
                    UserManager.Instance.FindController().SetInit();
                    SceneManager.LoadScene("wait");
                   // Application.LoadLevel("wait");  


                    //this.battle_room.gameObject.SetActive(true);
                    //this.battle_room.start_loading(player_index);
                    //  gameObject.SetActive(false);
                }
                break;

            case PROTOCOL.LOGIN_FAIL:
                {
                    SoundManager.Instance.PlaySfx(RegisterErrorSound);
                    popText.GetComponent<PopUp>().Pop("Login Fail!", 1.0f);
                }
                break;

            case PROTOCOL.REGISTER_RES:
                {
                    int isRegisterSuccess = msg.pop_int16();
                    if(isRegisterSuccess == 0)
                    {
                        SoundManager.Instance.PlaySfx(RegisterErrorSound);
                        popText.GetComponent<PopUp>().Pop("Already Exists Id", 1.0f);
                    }
                        
                    else
                    { 
                        SoundManager.Instance.PlaySfx(RegisterSuccessSound);
                        popText.GetComponent<PopUp>().Pop("Register Success", 1.0f);
                    }
                        
                }
                break;
                
        }
    }
}
