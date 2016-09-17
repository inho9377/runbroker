﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using FreeNet;

namespace Logic
{
    public class CWaitRoom
    {


        public List<CPlayer> playerList;
        Dictionary<byte, bool> ReadyStateDict;

        public CWaitRoom()
        {
            this.playerList = new List<CPlayer>();
            this.ReadyStateDict = new Dictionary<byte, bool>();
        }
        //Todo : Ready Error
        public void EnterWaitRoom(CGameUser user)
        {
            playerList.Add(user.player);
            ReadyStateDict.Add(user.player.player_index, false);
        }

        public void ExitWaitRoom(CGameUser user)
        {
            playerList.Remove(user.player);
            ReadyStateDict.Remove(user.player.player_index);
        }

        public bool isGameStart()
        {

            if (isAllReady() == false)
                return false;
            
            return true;

        }

        public void GameStart()
        {
            ReadyStateDict.Clear();
            foreach(CPlayer player in playerList)
            {
                ReadyStateDict.Add(player.player_index, false);
            }/*
            foreach (KeyValuePair<byte, bool> i in ReadyStateDict)
            {
                ReadyStateDict[i.Key] = false;
            }*/
        }

        public bool isAllReady()
        {


            foreach (KeyValuePair<byte, bool> i in ReadyStateDict)
            {
                if (ReadyStateDict[i.Key] == false)
                    return false;
            }


            return true;
        }


        public void SetReady(CPlayer player)
        {
            ReadyStateDict[player.player_index] = true;
        }

        public int GetReadyState(CPlayer player)
        {
            if (ReadyStateDict[player.player_index])
                return 1;
            else
                return 0;
        }

        public void SetNormal(CPlayer player)
        {
            ReadyStateDict[player.player_index] = false;
        }

        public void broadcast(CPacket msg, CPlayer owner)
        {
            foreach (CPlayer player in playerList)
            {
                //Todo : 플레이어.send가 그 플레이어 한테 보내는건가 그 플레이어 가 보내는건가
                if (player.player_index != owner.player_index)
                    player.send(msg);
            }
        }

        public void broadcast(CPacket msg)
        {
            foreach (CPlayer player in playerList)
            {
                player.send(msg);
            }
        }
    }
}
