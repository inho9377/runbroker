using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class rulebook : MonoBehaviour {


    public List<Sprite> ruleImageList;
    int currentPage;
	// Update is called once per frame

        void Start()
    {
        gameObject.SetActive(false);
    }
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
            NextPage();

	}

    void OnEnable()
    {
        currentPage = -1;
        NextPage();
    }

    void NextPage()
    {
        currentPage++;

        if (currentPage > ruleImageList.Count-1)
        {
            currentPage = -1;
            this.gameObject.SetActive(false);
            return;
        }

        this.GetComponent<Image>().sprite = ruleImageList[currentPage];
    }
}
