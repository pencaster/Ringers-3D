﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject circleBoardPrefab;
    public GameObject clipPrefab;

    public GameObject gameBoard;
    public GameObject gameClip;

    public Transform destroyOutOfLevel;

    public CursorController cursorControl;
    public ClipManager clipManager;
    public SliceManager sliceManager;
    public ConnectionManager connectionManager;
    public PowerUpManager powerupManager;

    public LevelScriptableObject currentLevel;

    public CSVParser csvParser;

    public int currentFilledCellCount;
    public int unsuccessfullConnectionCount;

    public bool gameStarted;

    public LevelScriptableObject[] levelArray;

    private void Awake()
    {
        Instance = this;
    }

    //void Start()
    //{
    //    StartLevel();
    //}

    public void StartLevel()
    {
        gameStarted = true;
        gameBoard = Instantiate(circleBoardPrefab, destroyOutOfLevel);
        gameClip = Instantiate(clipPrefab, destroyOutOfLevel);

        UIManager.Instance.GetCommitButton(gameBoard); 
        clipManager.Init();
        cursorControl.Init();

        sliceManager.SpawnSlices(currentLevel.slicesToSpawn.Length);
        connectionManager.GrabCellList(gameBoard.transform);

        PlayerManager.Instance.HandleItemCooldowns();

        PlayerManager.Instance.PopulatePowerUps();

        powerupManager.instnatiatedZonesCounter = 0;
    }

    public void ChooseLevel(int levelNum)
    {
        currentLevel = (LevelScriptableObject)Resources.Load("Scriptable Objects/Levels/Level " + levelNum);


        StartLevel();
    }

    public void DestroyAllLevelChildern()
    {
        gameStarted = false;

        foreach (Transform child in destroyOutOfLevel)
        {
            Destroy(child.gameObject);
        }

        foreach (Button butt in powerupManager.powerupButtons)
        {
            Destroy(butt.gameObject);
        }
        powerupManager.powerupButtons.Clear();

        currentFilledCellCount = 0;
        unsuccessfullConnectionCount = 0;
    }

    public void CheckEndLevel()
    {

        if(currentFilledCellCount == currentLevel.cellsCountInLevel && unsuccessfullConnectionCount == 0)
        {
            Debug.Log("YOU WIN");
            LootManager.Instance.GiveLoot();
            UIManager.Instance.WinLevel();

            if (currentLevel.levelNum >= PlayerManager.Instance.maxLevel)
            {
                PlayerManager.Instance.maxLevel++;
            }

        }
        else
        {
            LootManager.Instance.currentLevelLootToGive.Clear();
            UIManager.Instance.LoseLevel();
            Debug.Log("You Lose");
        }

        PlayerManager.Instance.SavePlayerData();
    }

    [ContextMenu ("Save Game Data")]
    public void GameSaveData()
    {
        
    }

    [ContextMenu("Game Load Data")]
    public void GameLoadData()
    {

    }
}
