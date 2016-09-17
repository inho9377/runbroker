using UnityEngine;
using FreeNet;
using FreeNetUnity;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NetworkManager : CSingletonMonobehaviour<NetworkManager>
{
    public enum NetworkStatus
    {
        ONLINE_MODE,
        OFFLINE_TESTING
    }

    public NetworkStatus networkStatus;

    CFreeNetUnityService gameserver;
    string received_msg;

    public MonoBehaviour message_receiver;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        this.received_msg = "";

        // 네트워크 통신을 위해 CFreeNetUnityService객체를 추가합니다.
        this.gameserver = gameObject.AddComponent<CFreeNetUnityService>();

        // 상태 변화(접속, 끊김등)를 통보 받을 델리게이트 설정.
        this.gameserver.appcallback_on_status_changed += on_status_changed;

        // 패킷 수신 델리게이트 설정.
        this.gameserver.appcallback_on_message += on_message;
    }

    public bool connect()
    {
       return  this.gameserver.connect("127.0.0.1", 7979);
    }
    public bool connect(string Ip, string Port)
    {
        return this.gameserver.connect(Ip, Int32.Parse(Port));
    }
    public bool is_connected()
    {
        return this.gameserver.is_connected();
    }

    public void disconnect()
    {
        this.gameserver.disconnect();
    }

    /// <summary>
    /// 네트워크 상태 변경시 호출될 콜백 매소드.
    /// </summary>
    /// <param name="server_token"></param>
    void on_status_changed(NETWORK_EVENT status)
    {
        switch (status)
        {
            // 접속 성공.
            case NETWORK_EVENT.connected:
                {
                    LogManager.log("on connected");
                    this.received_msg += "on connected\n";

                    //GameObject.Find("MainTitle").GetComponent<MainTitle>().on_connected();
                }
                break;

            // 연결 끊김.
            case NETWORK_EVENT.disconnected:
                LogManager.log("disconnected");
                this.received_msg += "disconnected\n";
                break;
        }
    }

    void on_message(CPacket msg)
    {

           this.message_receiver.SendMessage("on_recv", msg);
    }

    public void send(CPacket msg)
    {
        this.gameserver.send(msg);
    }
}
