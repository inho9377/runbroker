using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FreeNet;
using Logic;

public class UIManager : MonoBehaviour
{


    public Sprite Gamestart_popup_image;
    public Sprite _50seconds_left_image;
    public Sprite _40seconds_left_image;
    public Sprite _30seconds_left_image;
    public Sprite _20seconds_left_image;
    public Sprite _10seconds_left_image;
    
    public Sprite win_image;
    public Sprite lose_image;

    public GameObject result_object;

    List<Sprite> popup_list;
    public List<GameObject> result_team_name_texts;
    public List<GameObject> result_team_score_texts;
    int popup_index = 0;
    

    public Image image;
    public Text popupText;
    public List<Sprite> skillImageList;
    public Image skillImage;
    public Image LockSkillEventImage;

    
    int curretSkillImageIndex = 0;

    void Start()
    {

        popup_list = new List<Sprite>();
        popup_list.Add(Gamestart_popup_image);
        popup_list.Add(_50seconds_left_image);
        popup_list.Add(_40seconds_left_image);
        popup_list.Add(_30seconds_left_image);
        popup_list.Add(_20seconds_left_image);
        popup_list.Add(_10seconds_left_image);
        result_object.SetActive(false);
        LockSkillEventImage.gameObject.SetActive(false);
        skillImage.GetComponent<Image>().sprite = skillImageList[0];
    }
    public void LockSkillImagePop()
    {
        LockSkillEventImage.gameObject.SetActive(true);

    }
    public void LockSkillImageDowm()
    {
        LockSkillEventImage.gameObject.SetActive(false);
    }

    public void SkillImageProgress(int index)
    {
        if (index == curretSkillImageIndex)
            return;
        curretSkillImageIndex = index;
        skillImage.GetComponent<Image>().sprite = skillImageList[index];

    }

    public void popImage()
    {
        image.GetComponent<PopUp>().Pop(popup_list[popup_index]);
        popup_index++;
    }


    public void popWinLostImage(bool isWin)
    {
        if (isWin)
            image.GetComponent<PopUp>().Pop(win_image);
        else
            image.GetComponent<PopUp>().Pop(lose_image);
    }

    public void ShowResult (Dictionary<Common.PlayerColor, int> areaColorDict)
    {
        result_object.SetActive(true);
        int index = 0;
        foreach(var playerColorDict in areaColorDict.OrderByDescending(i => i.Value))
        {
            if (!UserManager.Instance.IsContainTeamColor(playerColorDict.Key))
                continue;
            result_team_name_texts[index].GetComponent<Text>().text = Helper.Instance.ReturnColorTGext( playerColorDict.Key);
            result_team_score_texts[index].GetComponent<Text>().text = playerColorDict.Value.ToString();
            index++;
        }
        
    }

    public void PopText(string text)
    {
        popupText.GetComponent<PopUp>().Pop(text);
    }

    public void PopDrawImage(Sprite draw_image)
    {
        image.GetComponent<PopUp>().Pop(draw_image);
    }
}
