﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LootPacks ////// Names of loot packs containing loot packs
{
    None,
    M1,
    M2,
    M3,
    R1,
    R2,
    I1,
    I2,
    I3,
    I4,
    I5,
    I6,
    I7
}

[System.Serializable]
public enum ItemTables
{
    None,
    L1,
    L2,
    L3,
    L4,
    L5,
    L6,
    L7
}

[System.Serializable]

public class RewardBag /// Class containing the values for lootpakcs (The loot latbles for each pack, OR the integer values for money and rubies).
{
    public bool IsMoneyOrRubies; ///// Differenciates between (Money & Rubies) Or Items.

    public List<ItemTables> Pack;

    public int[] minMaxValues;

    public int[] chancesPerItemTable;
}

[System.Serializable]
public class ItemTablesToCraftingMats
{
    public List<CraftingMats> tableToMat;
}

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    public GameObject keyPrefab;

    public GameObject lootDisplayPrefab;
    public Transform winScreenLootDisplayContent;

    public Texture goldSprite, rubySprite;

    public bool giveKey;

    [Header("List for Loot by Level")]
    /////// LIST FOR LOOT BY LEVEL
    public List<ItemTablesToCraftingMats> listOfTablesToMats;

    [Header("List for Reward Bags")]
    /////// LIST FOR REWARDBAGS
    public List<RewardBag> listOfRewardBags;

    [Header("During Gameplay")]
    public List<LootPacks> currentLevelLootToGive;

    public Dictionary<LootPacks, RewardBag> lootpackEnumToRewardBag;

    public Dictionary<ItemTables, List<CraftingMats>> itemTableToListOfMats;

    public List<CraftingMats> craftingMatsLootForLevel;

    private void Start()
    {
        Instance = this;

        craftingMatsLootForLevel = new List<CraftingMats>();
        currentLevelLootToGive = new List<LootPacks>();
        itemTableToListOfMats = new Dictionary<ItemTables, List<CraftingMats>>();
        lootpackEnumToRewardBag = new Dictionary<LootPacks, RewardBag>();

        for (int i = 1; i < System.Enum.GetValues(typeof(LootPacks)).Length; i++)
        {
            lootpackEnumToRewardBag.Add((LootPacks)i, listOfRewardBags[i - 1]);
        }

        for (int i = 1; i < System.Enum.GetValues(typeof(ItemTables)).Length; i++)
        {
            itemTableToListOfMats.Add((ItemTables)i, listOfTablesToMats[i - 1].tableToMat);
        }
    }
    
    public void GiveLoot()
    {
        foreach (LootPacks LP in currentLevelLootToGive)
        {
            RollOnTable(LP);
        }

        currentLevelLootToGive.Clear();

        if (giveKey)
        {
            Instantiate(keyPrefab, GameManager.Instance.destroyOutOfLevel);
            ZoneManagerHelpData.Instance.currentZoneCheck.hasAwardedKey = true;
            ZoneManagerHelpData.Instance.nextZoneCheck.isUnlocked = true;
            ZoneManagerHelpData.Instance.nextZoneCheck.maxLevelReachedInZone = 1;
            ZoneManagerHelpData.Instance.nextZoneCheck.zoneHeader.sprite = Resources.Load<Sprite>(ZoneManagerHelpData.Instance.nextZoneCheck.unlockedZonePath);

            ZoneManager.Instance.unlockedZoneID.Add(ZoneManagerHelpData.Instance.nextZoneCheck.id);
        }

        giveKey = false;
    }
    public void RollOnTable(LootPacks lootPack)
    {
        //if (lootPack == LootPacks.None)
        //{
        //    return;
        //}

        RewardBag rewardBagByLootPack = new RewardBag();

        rewardBagByLootPack = lootpackEnumToRewardBag[lootPack];

        if (!rewardBagByLootPack.IsMoneyOrRubies)
        {
            foreach (CraftingMats CM in craftingMatsLootForLevel)
            {
                DisplayLootMaterialsToPlayer(5, CM);

                PlayerManager.Instance.AddMaterials(CM, 5); //////// Figure out how to get amount from outside dynamically

            }

            craftingMatsLootForLevel.Clear();
            //List<CraftingMats> craftingMatsFromTables = new List<CraftingMats>();


            //for (int i = 0; i < rewardBagByLootPack.Pack.Count; i++)
            //{
            //    craftingMatsFromTables.AddRange(itemTableToListOfMats[rewardBagByLootPack.Pack[i]]);

            //    int chance = Random.Range(1, 101);

            //    if (chance > rewardBagByLootPack.chancesPerItemTable[i])
            //    {
            //        Debug.Log("Youa sucka Fuckkkkaeaeaeaeaeae");
            //        craftingMatsFromTables.Clear();
            //    }
            //    else
            //    {
            //        int randomMat = Random.Range(0, craftingMatsFromTables.Count);

            //        Debug.Log(craftingMatsFromTables[randomMat]);

            //        DisplayLootMaterialsToPlayer(5, craftingMatsFromTables[randomMat]);

            //        PlayerManager.Instance.AddMaterials(craftingMatsFromTables[randomMat], 5); //////// Figure out how to get amount from outside dynamically

            //        craftingMatsFromTables.Clear();
            //    }
            //}
        }
        else
        {
            int[] valuesToRecieve;
            valuesToRecieve = rewardBagByLootPack.minMaxValues;


            int randomNum = (Random.Range(valuesToRecieve[0], valuesToRecieve[1] + 1));

            if (lootPack.ToString().Contains("M"))
            {
                DisplayLootGoldRubyToPlayer(randomNum, goldSprite);
                PlayerManager.Instance.AddGold(randomNum);
            }
            else
            {
                DisplayLootGoldRubyToPlayer(randomNum, rubySprite);
                PlayerManager.Instance.AddRubies(randomNum);
            }

        }
    }

    public void ResetLevelLootData()
    {
        currentLevelLootToGive.Clear();
        craftingMatsLootForLevel.Clear();
    }

    public void DisplayLootMaterialsToPlayer(int count, CraftingMats CM)
    {
        GameObject go = Instantiate(lootDisplayPrefab, winScreenLootDisplayContent);

        CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

        CMD.materialImage.texture = Resources.Load(MaterialsAndForgeManager.Instance.materialSpriteByName[CM]) as Texture2D;
        CMD.materialCount.text = count.ToString();
    }
    public void DisplayLootGoldRubyToPlayer(int count, Texture s)
    {
        GameObject go = Instantiate(lootDisplayPrefab, winScreenLootDisplayContent);

        CraftingMatDisplayer CMD = go.GetComponent<CraftingMatDisplayer>();

        CMD.materialImage.texture = s as Texture2D;
        CMD.materialCount.text = count.ToString();
    }

    public void DestoryWinScreenDisplyedLoot()
    {
        foreach (Transform t in winScreenLootDisplayContent)
        {
            Destroy(t.gameObject);
        }
    }
}
