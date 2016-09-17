using UnityEngine;
using System.Collections;
using Logic;
using FreeNet;
using UnityEngine.SceneManagement;

public class InputManager : CSingletonMonobehaviour<InputManager>
{
    public GameObject playerObject;
    public Player player;
    public NetworkManager network_manager;
    public bool isResult = false;

    void Start()
    {
        this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        
    }

    public void SetInit()
    {
        player = UserManager.Instance.FindController();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (isResult)
                SceneManager.LoadScene("wait");
        }

        if (!GameManager.Instance.isGameStart)
            return;


        bool isStop = true;
        
        if (Input.GetKey(KeyCode.LeftArrow) == true)
        {
            player.Rotate(false);
        }
        else if (Input.GetKey(KeyCode.RightArrow) == true)
        {
           player.Rotate(true);
        }

        if(Input.GetKey(KeyCode.UpArrow) == true)
        {
            player.Move(true);
            isStop = false;
        }
        else if (Input.GetKey(KeyCode.DownArrow) == true)
        {
            player.Move(false);
            isStop = false;
        }

        if(isStop)
            player.Stop();


        if (Input.GetKey(KeyCode.Z) == true)
        {
            player.UseSkill();
        }
            

      


    }
    
}
