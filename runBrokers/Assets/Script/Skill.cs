using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FreeNet;
using Logic;
public class Skill : MonoBehaviour {


    public GameObject playerObject;
    public Player playerScript;
    public Common.PlayerType playerType;
    public float coolTime;
    public float skillDurationTime = -1; //-1 : ActiveDirectly
    public float currentTime;
    public float timeSpan_cool = 0.0f;
    public float timeSpan_duration = 0.0f;
    public bool isActive = false;
    public bool isUseOk = false;
    public int max_use = -1; //-1 : infinity
    public UIManager ui_manager;
    public float unityChanDefaultSpeed = 0.7f;
     float unityChanSkillSpeed = 1.2f;
    public float unityChanDefaultTurnSpeed = 1.0f;
     float unityChanSkillTurnSpeed = 1.1f;
    
    NetworkManager network_manager;

    public bool isKnightState = false;
    public bool isInit = false;
    public Vector3 ToonbotPosition;

    public AudioClip TeddySkillSound;
    public AudioClip CoolTimeCompleteSound;

    public float progress = 0.0f;
    public ParticleSystem skillEffect;

    public bool isController = false;
    
    public void Init(GameObject player)
    {
        playerObject = player;
        playerScript = player.GetComponent<Player>();
        playerType = player.GetComponent<Player>().playerType;
        SkillSet();
        ToonbotPosition = new Vector3();
        this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        this.ui_manager = GameObject.FindGameObjectWithTag("Controller").GetComponentInChildren<UIManager>();
        isInit = true;

        
        isController = (UserManager.controller_index == playerScript.player_index);
        
        skillEffect.startColor = Helper.Instance.ReturnColor(playerScript.playerColor);
        skillEffect.Stop();

        unityChanDefaultSpeed = playerScript.moveSpeed;
        unityChanDefaultTurnSpeed = playerScript.turnSpeed;

    }



    
    void Update()
    {
        
        if (!isInit)
            return;
        if (!GameManager.Instance.isGameStart)
            return;

        if(isController)
            ShowSkillImage();

        if (isActive)
        {
            timeSpan_duration += Time.deltaTime;
            if (timeSpan_duration >= skillDurationTime)
            {
                SkillReset();
                skillEffect.gameObject.SetActive(false);
                skillEffect.Stop();
            }
            return;    
        }

        if (isUseOk)
            return;



        timeSpan_cool += Time.deltaTime;
        progress = timeSpan_cool / coolTime;

        if (timeSpan_cool >= coolTime)
        {
            if (isController)
                SoundManager.Instance.PlaySfx(CoolTimeCompleteSound);
            isUseOk = true;
        }
    }

    void ShowSkillImage()
    {

        if (isActive)
        {
            ui_manager.SkillImageProgress(4);
            return;
        }
        if (progress < 0.3f)
        {
            ui_manager.SkillImageProgress(0);
            return;
        }
        if (progress < 0.6f)
        {
            ui_manager.SkillImageProgress(1);
            return;
        }
        if (progress < 0.8f)
        {
            ui_manager.SkillImageProgress(2);
            return;
        }
        if (isUseOk)
        {
            ui_manager.SkillImageProgress(3);
            return;
        }
        
    }

    void SkillSet()
    {
        switch (playerType)
        {
            case Common.PlayerType.TOON_BOT:
                ToonBotSkillSet();
                return;
            case Common.PlayerType.UNITY_CHAN:
                UnityChanSkillSet();
                return;
            case Common.PlayerType.KNIGHT:
                KnightSkillSet();
                return;
            case Common.PlayerType.TEDDY:
                TeddySkillSet();
                return;

        }
    }

    public void SkillActive()
    {
        if (!isUseOk)
        {
            //ui_manager.PopText("Not Yet Used Skill CoolTime");
            return;
        }

        if (isController && playerType != Common.PlayerType.TOON_BOT)
        {
            CPacket msg = CPacket.create((short)PROTOCOL.SKILL_USE_REQ);
            network_manager.send(msg);
        }
        if (playerScript.player_index == UserManager.controller_index)
            SoundManager.Instance.PlayCharSound(playerType);
        skillEffect.gameObject.SetActive(true);
        skillEffect.Play();
        switch (playerType)
        {
            case Common.PlayerType.TOON_BOT:
                ToonBotSkillActive();
                break;
            case Common.PlayerType.UNITY_CHAN:
                UnityChanSkillActive();
                break;
            case Common.PlayerType.KNIGHT:
                KnightSkillActive();
                break;
            case Common.PlayerType.TEDDY:
                TeddySkillActive();
                break;
        }

        timeSpan_cool = 0.0f;
        isUseOk = false;
        isActive = true;
    }

    void SkillReset()
    {
        //skillEffect.Stop();
        switch (playerType)
        {
            case Common.PlayerType.TOON_BOT:
                ToonBotSkillReset();
                break;
            case Common.PlayerType.UNITY_CHAN:
                UnityChanSkillReset();
                break;
            case Common.PlayerType.KNIGHT:
                KnightSkillReset();
                break;
            case Common.PlayerType.TEDDY:
                TeddySkillReset();
                break;
        }

        timeSpan_duration = 0.0f;
        isActive = false;
        isUseOk = false;
    }

    void UnityChanSkillSet()
    {
        coolTime = 7.0f;
        skillDurationTime = 10.0f;
    }

    void KnightSkillSet()
    {
       coolTime = 10.0f;
        skillDurationTime = 10.0f;
    }

    void TeddySkillSet()
    {
        coolTime = 10.0f;
        skillDurationTime = 10.0f;
    }

    void ToonBotSkillSet()
    {
        coolTime = 5.0f;
        skillDurationTime = 2.0f;
    }

    void UnityChanSkillActive()
    {

        unityChanDefaultSpeed = playerScript.moveSpeed;
        unityChanDefaultTurnSpeed = playerScript.turnSpeed;

        playerScript.moveSpeed += unityChanSkillSpeed;
        playerScript.turnSpeed += unityChanSkillTurnSpeed;
        //playerScript.UseSkill();
    }

    void KnightSkillActive()
    {
        isKnightState = true;
    }

    void TeddySkillActive()
    {
        foreach (Player user in UserManager.UserList)
        {
            if (user.player_index == playerScript.player_index)
                continue;
            if (user.playerColor == playerScript.playerColor)
                continue;

            if (user.player_index == UserManager.controller_index)
            {
                ui_manager.LockSkillImagePop();
                SoundManager.Instance.PlaySfx(TeddySkillSound);
            }
               

            user.isColorDefault = true;
            //user.playerColor = Common.PlayerColor.DEFAULT;
        }
    }

    void ToonBotSkillActive()
    {

        ToonbotPosition = MapManager.Instance.GetRandomPosition();
        CPacket send_msg = CPacket.create((short)PROTOCOL.TELEPORT_REQ);
        send_msg.push(ToonbotPosition.x);
        send_msg.push(ToonbotPosition.z);
        network_manager.send(send_msg);
    }

    public void ToonBotSkillActive(float positionX, float positionZ)
    {
        Vector3 pos = new Vector3(positionX, playerObject.transform.position.y, positionZ);
        ToonbotPosition = pos;
        timeSpan_cool = 0.0f;
        isUseOk = false;
        isActive = true;
    }

    void UnityChanSkillReset()
    {
        playerScript.moveSpeed = unityChanDefaultSpeed;
        playerScript.turnSpeed = unityChanDefaultTurnSpeed;
    }

    void KnightSkillReset()
    {
        isKnightState = false;
    }

    void TeddySkillReset()
    {
        int idx = 0;
        foreach (Player user in UserManager.UserList)
        {
            if (user.player_index == playerScript.player_index)
                continue;
            if (user.player_index == UserManager.controller_index)
                ui_manager.LockSkillImageDowm();

            user.isColorDefault = false;
            idx++;
        }
    }

    void ToonBotSkillReset()
    {
        playerObject.transform.position = ToonbotPosition;
    }
}
