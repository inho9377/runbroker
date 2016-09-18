using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;

public class UserManager : CSingletonMonobehaviour<UserManager>
{


    public static List<Player> UserList;
    public static Dictionary<byte, int> player_seq;
    public static byte controller_index;


    void Awake()
    {
        DontDestroyOnLoad(this);
        UserList = new List<Player>();
        player_seq = new Dictionary<byte, int>();
    }

    public void SetPlayerSeq(List<byte> players_index)
    {
        player_seq.Clear();
        for(int i=0; i<UserList.Count; i++)
        {
            player_seq.Add(players_index[i], i);
        }
    }

    public void SetPlayerStartLocation(List<Vector3> startPosList)
    {
        foreach(Player user in UserList)
        {
            user.transform.position = startPosList[player_seq[user.player_index]];
            user.targetPos = user.transform.position;

            MapManager.Instance.FindAreaByPosition(startPosList[player_seq[user.player_index]]).ChangeAreaColor(user.playerColor, user.player_index);

        }
    }

    public void AddPlayer(Player player)
    {
        UserList.Add(player);
    }

    public void AddPlayer(byte player_index, string playerId)
    {

        Player player = new Player();
        player.player_index = player_index;
        player.Id = (string)playerId.Clone();
        UserList.Add(player);
        
    }

    public void ClearUserList()
    {
        UserList.Clear();
    }

    public Player FindPlayer(int find_index)
    {
        foreach(Player player in UserList)
        {
            if (player.player_index == find_index)
                return player;
        }

        LogManager.log("Finding User Error! Don't Exist");
        return null;
    }

    public Player FindController()
    {
        foreach (Player player in UserList)
        {
            if (player.player_index == controller_index)
                return player;
        }

        LogManager.log("Finding User Error! Don't Exist");
        return null;
    }

    public void RemovePlayer(byte remove_player_index)
    {
        
        UserList.RemoveAt(FindPlayerIndex(remove_player_index));
    }

    public int FindPlayerIndex(byte playerIndex)
    {
        int idx = 0;
        foreach(Player p in UserList)
        {
            if (p.player_index == playerIndex)
                return idx;
            idx++;
        }
        return -1;
    }

    public int GetNumberOfTeam()
    {
        List<Common.PlayerColor> list_color = new List<Common.PlayerColor>();
        foreach(Player user in UserList)
        {
            if(!list_color.Contains(user.playerColor))
                list_color.Add(user.playerColor);
        }

        return list_color.Count;
        
    }

    public bool IsContainTeamColor(Common.PlayerColor color)
    {
        foreach(Player user in UserList)
        {
            if (user.playerColor == color)
                return true;
        }

        return false;
    }
}
