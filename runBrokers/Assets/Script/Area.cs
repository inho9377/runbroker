using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;

public class Area : MonoBehaviour {

    

   public Logic.Common.PlayerColor area_color = Logic.Common.PlayerColor.DEFAULT;
    public float color_change_speed = 0.5f;

    Color changeColor = Color.cyan;
    Color beforeColor = Color.cyan;

    public float color_change_sec = 2.0f;

    public AudioClip colorChangeSound;
    
    
    public bool isStartPoint = false;
    public Texture KnightLocTextuer;
    public bool isKnightLoc = false;

    public bool isItemPoint = false;
    
	
	// Update is called once per frame
	void Update () {

        if (!GameManager.Instance.isGameStart)
            return;

        if (changeColor == beforeColor)
            return;

        beforeColor = changeColor;

        StartCoroutine(ColorChangeLerpProcess());

    }
    

    void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.isGameStart)
            return;

        if (isKnightLoc)
            return;


        LogManager.log("col");

        Common.PlayerColor changeColor = other.GetComponent<Player>().playerColor;
		
        if(this.area_color == changeColor )
			return;

        if (changeColor == Common.PlayerColor.DEFAULT)
            return;

        if (other.GetComponent<Player>().isColorDefault)
            return;

        if(other.gameObject.name.Contains("Player"))
        {
            LogManager.log("colPlayer");
		
            ChangeAreaColor(changeColor, other.GetComponent<Player>().player_index);

            if (other.GetComponent<Skill>().isKnightState)
                ChangeKingtLocation();
        }



    }

    public void ChangeAreaColor(Common.PlayerColor change_color, byte colliderIndex)
    {
		if(colliderIndex == UserManager.controller_index)
			SoundManager.Instance.PlaySfx(colorChangeSound);
        
		this.area_color = change_color;
        changeColor = Helper.Instance.ReturnColor(change_color);

        /*foreach (MeshRenderer material in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (changeColor == material.material.color)
                return;
            LogManager.log("changecolor");
            material.material.color = Color.Lerp(material.material.color, changeColor, Time.time);
        }

        /*
        foreach (MeshRenderer material in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            LogManager.log("changecolor");
            material.material.color = changeColor;
        }*/



    }

    void ChangeKingtLocation()
    {
        isKnightLoc = true;

        foreach (MeshRenderer material in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            LogManager.log("changecolor");
            
            material.material.mainTexture = KnightLocTextuer;

        }
    }

    public void ChangeAreaColor (Color color)
    {
        foreach (MeshRenderer material in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            LogManager.log("changecolor");

            material.material.color = color;

        }
    }


    IEnumerator ColorChangeLerpProcess()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        yield return null;

        float elapsed = 0.0f;

        while(elapsed < color_change_sec)
        {
            elapsed += Time.deltaTime;
            foreach (MeshRenderer material in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                LogManager.log("changecolor");
                material.material.color = Color.Lerp(material.material.color, changeColor, elapsed/color_change_sec);
                
            }
            yield return wait;
        }

        foreach (MeshRenderer material in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            LogManager.log("changecolor");
            material.material.color = changeColor;

        }

    }
}
