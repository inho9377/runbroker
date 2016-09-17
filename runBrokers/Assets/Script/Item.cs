
using UnityEngine;
using System.Collections;
using Logic;
using FreeNet;

public class Item : MonoBehaviour
{
    public enum ItemProperty : byte
    {
        TELEPORT = 0,
        ACCELATOR = 1,
    }

    public float yPos = 0.35f;


    public ItemProperty itemProperty;

    public byte itemIndex;

    public ParticleSystem accelatorParticle;

    public AudioClip TeleportSound;
    public AudioClip AccelatorSound;

    void OnTriggerEnter(Collider other)
    {
        Player colPlayer = other.GetComponent<Player>();

        
        
        ItemActive(colPlayer);
    }

    public void ItemActive(Player player)
    {
        switch(itemProperty)
        {
            case ItemProperty.TELEPORT:
                TeleportActive(player);
                break;

            case ItemProperty.ACCELATOR:
                AccelatorActive(player);
                break;
        }

        gameObject.SetActive(false);
    }


    void TeleportActive(Player player)
    {


        Vector3 ranPos = MapManager.Instance.GetRandomPosition();
        Vector3 pos = new Vector3(ranPos.x, player.transform.position.y, ranPos.z);

        if(player.player_index == UserManager.controller_index)
        {
            SoundManager.Instance.PlaySfx(TeleportSound);

            CPacket msg = CPacket.create((short)PROTOCOL.TELEPORT_ITEM_REQ);
            msg.push(itemIndex);
            msg.push(pos.x);
            msg.push(pos.z);
            NetworkManager.Instance.send(msg);
        }

        player.transform.position = pos;
    }

    void AccelatorActive(Player player)
    {
        if (player.player_index == UserManager.controller_index)
        {
            SoundManager.Instance.PlaySfx(AccelatorSound);
            CPacket msg = CPacket.create((short)PROTOCOL.ITEM_USE_REQ);
            msg.push(itemIndex);
            NetworkManager.Instance.send(msg);
        }

        if(player.playerType == Common.PlayerType.UNITY_CHAN)
        {
            player.skill.unityChanDefaultSpeed += 0.5f;
            player.skill.unityChanDefaultTurnSpeed += 0.4f;

        }
            

        player.moveSpeed += 0.5f;
        player.turnSpeed += 0.4f;
       // accelatorParticle.Simulate(2.0f);
    }
}
