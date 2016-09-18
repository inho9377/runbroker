
using UnityEngine;
using System.Collections;
using Logic;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public enum PLAYER_STATE : byte
    {
        IDLE,
        RUN,
        ATTACK,
        DEAD
    }
    
    public PLAYER_STATE state = PLAYER_STATE.IDLE;
    public Animator animator;
    public Logic.Common.PlayerColor playerColor;// = Common.PlayerColor.RED;
    public Logic.Common.PlayerType playerType;// = Common.PlayerType.UNITY_CHAN;

    public string Id;
    public byte player_index;
    public int shield = 0;

    

    public float moveSpeed = 0.7f;
    public float turnSpeed = 1.0f;
    public CharacterController characterController = null;
    public Vector3 targetPos;
    
    
    public int isReady = 0;
    public bool isController = false;

    public bool isColorDefault = false;

    public float gravity = -9.8f;

    public GameObject playerObject;

    public Skill skill;
    

    public int skillState;
    public void SetInit()
    {
        if (player_index == UserManager.controller_index)
            isController = true;

       
    }

    public void CameraOff()
    {
        GetComponent<Camera>().cullingMask = 1 << 0;
    }

    public void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        skill = GetComponent<Skill>();
        LogManager.log("player start!");
        targetPos = transform.position;
        while (characterController.isGrounded)
            characterController.Move(new Vector3(transform.position.x, gravity * Time.deltaTime, transform.position.z));
        SetSkillScript();

        moveSpeed = 0.7f;
        turnSpeed = 1.0f;

        skillState = Animator.StringToHash("Basde.Skill");

        if(!isController)
        {
            GetComponentInChildren<TextMesh>().text = Id;
            GetComponentInChildren<TextMesh>().color = Helper.Instance.ReturnColor(playerColor);
        }
        isColorDefault = false;
    }

    public void SetModel(GameObject model)
    {
        playerObject = model;
    }
    
    public void SetSkillScript()
    {
        skill.Init(this.gameObject);
    }

    public void SetInfo(Player info)
    {
        this.playerColor = info.playerColor;
        this.playerType = info.playerType;
        this.Id = info.Id;
        this.player_index = info.player_index;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameStart)
            return;

        if (isController)
            return ;

        if (targetPos == transform.position)
            Stop();


        transform.LookAt(targetPos);
        float moveFloat = Time.deltaTime * moveSpeed;

        this.transform.position = Vector3.MoveTowards(transform.position, targetPos, moveFloat);
    }

    public void Move(float positionX, float positionY, float positionZ, float speed)
    {
        


        if (transform.position.x == positionX && transform.position.z == positionZ)
            return;

        animator.SetTrigger("Run");

        targetPos.x = positionX;
        targetPos.y = positionY;
        targetPos.z = positionZ;
        moveSpeed = speed;
    }


    public void Move(bool isUp)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
            return;

        animator.SetTrigger("Run");
        state = PLAYER_STATE.RUN;

        if (isUp)
            characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
        else
            return;
    }
    public void Rotate(bool isRightSpin)
    {
        Vector3 PrePosition;
        if (isRightSpin)
            PrePosition = Vector3.right;
        else
            PrePosition = Vector3.left;

        Quaternion toRotation = transform.rotation * Quaternion.LookRotation(PrePosition);


        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.fixedDeltaTime*turnSpeed);
        
    }
    

    public void Stop()
    {
        animator.SetTrigger("Idle");
        state = PLAYER_STATE.IDLE;
    }

    public void UseSkill()
    {
        if(!skill.isUseOk)
        {
            return;
        }
        skill.SkillActive();
        animator.SetTrigger("Skill");
    }

    public void UseSkillDirect()
    {
        skill.isActive = true;
        skill.isUseOk = true;
        skill.SkillActive();
        animator.SetTrigger("Skill");
    }
}
