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
        idInputField.GetComponent<InputField>().ActivateInputField();
        
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
        LogManager.log("id : " + inputId);
        LogManager.log("ip : " + inputIP);
        LogManager.log("Port : " + inputPort);
        this.network_manager.message_receiver = this;

        CPacket msg = CPacket.create((short)PROTOCOL.LOGIN_REQ);
        msg.push(inputId);
        network_manager.send(msg);

    }

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

    
    public void on_connected()
    {

    }


    public void MuteButton()
    {
        SoundManager.Instance.Pause();
    }
    public void on_recv(CPacket msg)
    {
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
