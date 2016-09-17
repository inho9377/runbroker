using UnityEngine;
using System.Collections;
using FreeNet;
using Logic;

public class NetworkProcessManager : MonoBehaviour {


    public Player controller;
    public NetworkManager network_manager;

    public float interval_controller_state_packetprocess_sec;
    public float timeSpan = 0.0f;


    int num = 0;
	void Start ()
    {
        controller = UserManager.Instance.FindController();
        this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        interval_controller_state_packetprocess_sec = 0.2f;
    }
	
	void Update ()
    {

        if (!GameManager.Instance.isGameStart)
            return;

        if (GameManager.Instance.isResult)
            return;

        timeSpan += Time.deltaTime;

        if (timeSpan >= interval_controller_state_packetprocess_sec)
        {
            ControllerPositionTransport();
            LogManager.log("pos count : " + num++);
            //LogManager.log(timeSpan + "sec");
            timeSpan = 0.0f;
        }
	}

    void ControllerPositionTransport()
    {
        Player controller = UserManager.Instance.FindController();
        CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_POSITION_REQ);
        msg.push(controller.transform.position.x);
        msg.push(controller.transform.position.y);
        msg.push(controller.transform.position.z);
        msg.push(controller.moveSpeed);

        network_manager.send(msg);

    }
}
