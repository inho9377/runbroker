﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FreeNet;

namespace Logic
{
    public class CGameRoom
    {
        
        public List<CPlayer> players;
        int numLoading = 0;

        bool isGameStart = false;


        int termSec = 10;
        int limitTime;
        Stopwatch stop_watch;
        int time_index = 0;

        public CGameRoom()
        {
            this.players = new List<CPlayer>();
            stop_watch = new Stopwatch();
        }


        public void EnterGameRoom(CGameUser user)
        {
            players.Add(user.player);
        }

        public void ExitGameRoom(CGameUser user)
        {
            players.Remove(user.player);
            if(players.Count == 0)
            {
                ResetRoom();
            }

        }

        public void ResetRoom()
        {
            isGameStart = false;
            stop_watch.Reset();
            stop_watch.Stop();

            foreach(CPlayer user in players)
            {
                user.player_scene = Common.PLAYER_SCENE.WAIT_ROOM;
               
            }
        }


        public void broadcast(CPacket msg, CPlayer owner)
        {
            foreach (CPlayer player in players)
            {
                if(player.player_index != owner.player_index)
                     player.send_for_broadcast(msg);
                
            }
            CPacket.destroy(msg);
        }

        public void broadcast(CPacket msg)
        {
            foreach (CPlayer player in players)
            {
                  player.send_for_broadcast(msg);
                
            }
            CPacket.destroy(msg);
        }

        public void GameStart(int settingTime)
        {
            isGameStart = true;
            limitTime = settingTime;
            stop_watch.Start();
            time_index = 1;
            numLoading = 0;
        }

        public void Update()
        {
            if (!isGameStart)
                return;

            if (isOnePlayerLeft())
                GameSet();


            TimeCheck();
        }

        public bool isLoadComplete()
        {
            numLoading++;
            if (numLoading == players.Count)
                return true;
            else
                return false;
        }

        void TimeCheck()
        {
            if (stop_watch.ElapsedMilliseconds >= limitTime * 1000)
            {
                Console.WriteLine("set : " + stop_watch.ElapsedMilliseconds);
                time_index = 0;
                GameSet();
                return;
            }

            if (stop_watch.ElapsedMilliseconds >= time_index * termSec * 1000)
            {
                time_index++;
                CPacket msg = CPacket.create((short)PROTOCOL.PLAY_TIME);
                msg.push((Int16)(stop_watch.ElapsedMilliseconds / 1000));
                broadcast(msg);


                Console.WriteLine("Now Time : " + stop_watch.ElapsedMilliseconds + "sec : " + (int)(stop_watch.ElapsedMilliseconds / 1000));
            }

        }

        void GameSet()
        {
            CPacket msg = CPacket.create((short)PROTOCOL.TIME_OVER);
            broadcast(msg);
            ResetRoom();

            foreach(CPlayer user in players)
            {
                user.player_scene = Common.PLAYER_SCENE.WAIT_ROOM;
                
            }
            players.Clear();
        }


        bool isOnePlayerLeft()
        {
            if (players.Count == 1)
                return true;

            return false;
        }

    }
}
