using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void BroadcastKuldoCrystalChange();
    public event BroadcastKuldoCrystalChange KuldoChanged;

    public delegate void BroadcastGoldChange();
    public event BroadcastGoldChange GoldChanged;

    private int kuldoCrystal;
    private int gold;

    private void Start()
    {
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        kuldoCrystal = PlayerData.kuldoCount;
        gold = PlayerData.playerGold;
        if (KuldoChanged != null)
        {
            KuldoChanged();
        }
        if (GoldChanged != null)
        {
            GoldChanged();
        }
    }

    public void KuldoCrystalChange(int change)
    {
        kuldoCrystal += change;
        PlayerData.kuldoCount = kuldoCrystal;
        if (KuldoChanged != null)
        {
            KuldoChanged();
        }
    }

    public void GoldChange(int change)
    {
        gold += change;
        if (change > 0)
        {
            PlayerData.totalGoldCollected += change;
        }
        PlayerData.playerGold = gold;
        Debug.Log(PlayerData.playerGold);
        if (GoldChanged != null)
        {
            GoldChanged();
        }
    }

    public int KuldoCrystalCount()
    {
        return kuldoCrystal;
    }

    public int GoldCount()
    {
        return gold;
    }
}


