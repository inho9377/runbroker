using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;
using System.Linq;

public class MapManager : CSingletonMonobehaviour<MapManager>
{
    

    public int width_area_num;
    public int height_area_num;
       
    public List<GameObject> areaList;

    public Texture wallTexture;

    void Start()
    {
        //확인필요
        areaList = GameObject.FindGameObjectsWithTag("area").ToList<GameObject>();

        //   CreateMap();
        FillWallTexture();
    }

    void FillWallTexture()
    {
        foreach(var wall in GameObject.FindGameObjectsWithTag("wall").ToList<GameObject>())
        {
            wall.GetComponent<MeshRenderer>().material.mainTexture = wallTexture;
        }
    }

    public Area FindAreaByPosition(Vector3 pos)
    {
        foreach(var area in areaList)
        {
            if (area.gameObject.transform.position == pos)
                return area.GetComponent<Area>();
        }

        return null;
    }

    /*void CreateMap(int width_num = 10, int height_num = 8)
    {
        width_area_num = width_num;
        height_area_num = height_num;
        int idx = 0;

        for(int width=0; width<width_area_num; width++)
        {
            for(int height=0; height < height_area_num; height++ )
            {
               GameObject area = Instantiate(area_object) as GameObject;
               idx++;
            }
        }
    }
    */
    
    public List<Vector3> GetStartPosition()
    {
        List<Vector3> startPosList = new List<Vector3>();
        foreach (GameObject area in areaList)
        {
            if(area.GetComponent<Area>().isStartPoint == true)
            {
                startPosList.Add(area.transform.position);
            }
        }

        return startPosList;
    }


    
    public Dictionary<Common.PlayerColor, int> GetColorAreaNum()
    {
        var colorDict = new Dictionary<Common.PlayerColor, int>();
        colorDict.Add(Common.PlayerColor.PURPLE, 0);
        colorDict.Add(Common.PlayerColor.WHITE, 0);
        colorDict.Add(Common.PlayerColor.RED, 0);
        colorDict.Add(Common.PlayerColor.GREEN, 0);
        colorDict.Add(Common.PlayerColor.YELLOW, 0);
        
        foreach (Area area in GetComponentsInChildren<Area>())
        {
            if (area.area_color == Common.PlayerColor.DEFAULT)
                continue;

            colorDict[area.area_color]++;
        }

        //Helper.Instance.DictDescendingOrderByValue<Common.PlayerColor>(ref colorDict);
        
        return colorDict;
    }


    public Vector3 GetRandomPosition()
    {
        int range = areaList.Count - 1;
        Vector3 ranAreaPos = areaList[Random.Range(0, range)].transform.position;


        return ranAreaPos;

    }

    public void AllAreaReset()
    {
        foreach (GameObject area in areaList )
        {
            Area areaS = area.GetComponent<Area>();
            areaS.area_color = Common.PlayerColor.DEFAULT;
            areaS.ChangeAreaColor(Color.cyan);
        }
    }


    public List<Vector3> GetItemAreaPos()
    {
        List<Vector3> itemPosLis = new List<Vector3>();
        foreach (Area area in GetComponentsInChildren<Area>())
        {
            if (area.isItemPoint)
                itemPosLis.Add(area.transform.position);
            
        }

        return itemPosLis;
    }
}
