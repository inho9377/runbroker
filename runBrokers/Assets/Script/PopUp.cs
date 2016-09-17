using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PopUp : MonoBehaviour
{


    float time_span = 0.0f;
    float duration_popup = 2.0f;

    void Awake()
    {
        gameObject.SetActive(false);
        // setDisable();
    }

    public void Pop(Sprite popupImage, float duration_popup_set = 1.5f)
    {
        gameObject.SetActive(true);
        duration_popup = duration_popup_set;
        GetComponent<Image>().sprite = popupImage;
    }

    public void Pop(string popupMsg, float duration_popup_set = 1.5f)
    {
        gameObject.SetActive(true);
        duration_popup = duration_popup_set;
        GetComponent<Text>().text = popupMsg;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameStart)
            return;
        if (duration_popup == 0.0f)
            return;

        time_span += Time.deltaTime;

        if (time_span > duration_popup)
        {
            gameObject.SetActive(false);
            time_span = 0.0f;

        }

    }

    // 오브젝트 Disable
    void setDisable()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        gameObject.SetActive(false);
    }

}
