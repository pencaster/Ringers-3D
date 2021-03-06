using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

[System.Serializable]
public class pieceDataStruct
{
    public PieceColor colorOfPieceRight;
    public PieceSymbol symbolOfPieceRight;
    public PieceColor colorOfPieceLeft;
    public PieceSymbol symbolOfPieceLeft;
}

[System.Serializable]
public class Sequence
{
    public int levelTutorialIndex;
    public int EndPhaseID;
    public OutLineData[] cellOutlines;
    public Phase[] phase;
    public GameObject[] screens;
    public Sprite[] sprites;
}

[System.Serializable]
public class Phase
{
    public int phaseID;
    public bool isClipPhase, isBoardPhase;
    public bool dealPhase;

    public int[] unlockedClips;
    public int unlockedBoardCells;

    public int[] targetCells;

}
[System.Serializable]
public class OutLineData
{
    public int cellIndex;
    public bool right;
}

public class TutorialSequence : MonoBehaviour
{
    public static TutorialSequence Instacne;
    public int currentPhaseInSequence;

    public Sequence[] levelSequences;

    public bool duringSequence;

    public Image maskImage;
    private void Start()
    {
        Instacne = this;
        maskImage.gameObject.SetActive(false);
    }

    public void StartSequence(int levelNum)
    {
        if (!GameManager.Instance.isDisableTutorials)
        {
            UIManager.Instance.tutorialCanvas.SetActive(true);

            DisplayTutorialScreens();
            OutlineInstantiate();
            UIManager.Instance.dealButton.interactable = false;

            currentPhaseInSequence = 0;
            duringSequence = true;

            for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
            {
                Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

                //for (int k = 0; k < levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
                //{
                if (levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
                //}
            }
        }
    }

    public void DisableTutorialSequence()
    {
        UIManager.Instance.tutorialCanvas.SetActive(false);

        UIManager.Instance.dealButton.interactable = true;

        currentPhaseInSequence = 0;
        duringSequence = false;

        //for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        //{
        //    Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

        //    //for (int k = 0; k < levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
        //    //{
        //    if (levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
        //    {
        //        p.isTutorialLocked = false;
        //    }
        //    else
        //    {
        //        p.isTutorialLocked = true;
        //    }
        //    //}
        //}
    }

    private void DisplayTutorialScreens()
    {
        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens)
        {
            go.SetActive(false);
        }

        levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[0].SetActive(true);

        maskImage.gameObject.SetActive(true);
        maskImage.sprite = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].sprites[0];
    }

    public void IncrementCurrentPhaseInSequence()
    {
        currentPhaseInSequence++;

        if(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence])
        {
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence - 1].SetActive(false);
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence].SetActive(true);
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence].transform.GetChild(0).gameObject.SetActive(true);
        }


        if (currentPhaseInSequence >= levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].EndPhaseID)
        {
            maskImage.gameObject.SetActive(false);
            duringSequence = false;
            Debug.Log("Phases are done!");
            //Invoke("UnlockAll", 2);

            UnlockAll();
            Invoke("DeactivateTutorialScreens", 2);

            return;
        }

        maskImage.sprite = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].sprites[currentPhaseInSequence];

        ChangePhase();
    }

    public void ChangePhase()
    {
        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].isClipPhase)
        {
            ClipPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].isBoardPhase)
        {
            BoardPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].dealPhase)
        {
            DealPhaseLogic();
        }
    }

    public void ClipPhaseLogic()
    {
            UIManager.Instance.dealButton.interactable = false;

            foreach (Cell c in ConnectionManager.Instance.cells)
            {
                if (c.isFull)
                {
                    c.pieceHeld.isTutorialLocked = true;
                }
            }

            for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
            {
                Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

                //for (int k = 0; k < levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
                //{
                if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
                //}
            }
    }
    public void BoardPhaseLogic()
    {
        UIManager.Instance.dealButton.interactable = false;

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = true;
        }


        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = true;
                //int length = levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedBoardCells.Length;
                //for (int i = 0; i < length; i++)
                //{
                if (c.cellIndex == levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].unlockedBoardCells/*[i]*/)
                {
                    c.pieceHeld.isTutorialLocked = false;
                }
                //}
            }
        }
    }
    public void DealPhaseLogic()
    {
        UIManager.Instance.dealButton.interactable = true;

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = true;
        }

        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = true;
            }
        }
    }
    public void OutlineInstantiate()
    {
        foreach (OutLineData old in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].cellOutlines)
        {
            if (old.right)
            {
                Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteRight, ConnectionManager.Instance.cells[old.cellIndex].transform);
            }
            else
            {
                Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteLeft, ConnectionManager.Instance.cells[old.cellIndex].transform);
            }
        }
    }

    public void UnlockAll()
    {
        //foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens)
        //{
        //    go.SetActive(false);
        //}

        UIManager.Instance.dealButton.interactable = true;

        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = false;
            }
        }

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = false;
        }
    }

    public void DeactivateTutorialScreens()
    {
        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens)
        {
            if (go.activeInHierarchy)
            {
                //StartCoroutine(FadeImage(go, 2f));
                FadeImage(go, 2f,false);

                foreach (TMP_Text child in go.GetComponentsInChildren<TMP_Text>())
                {
                    FadeImage(child.gameObject, 2f, true);
                }
            }
        }
    }

    private void FadeImage(GameObject toFade, float speed, bool isText)
    {
        if (!isText)
        {
            LeanTween.value(toFade, 1, 0, speed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                Image sr = toFade.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });

        }
        else
        {
            LeanTween.value(toFade, 1, 0, speed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                TMP_Text sr = toFade.GetComponent<TMP_Text>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });
        }

        StartCoroutine(DisableFadeImage(toFade, speed, isText));
    }

    IEnumerator DisableFadeImage(GameObject go, float time, bool isText)
    {
        if (isText)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
            TMP_Text sr = go.GetComponent<TMP_Text>();
            sr.color = new Color(0.2f, 0.2f, 0.2f, 1); ////////////VERY TEMPORARY
        }
        else
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
            Image sr = go.GetComponent<Image>();

            sr.color = Color.white;
        }

    }
    public void TurnOnTutorialScreensAfterOptions()
    {
        if(currentPhaseInSequence < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].EndPhaseID)
        {
            if(currentPhaseInSequence > 0)
            {
                levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence - 1].SetActive(false);
            }
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence].SetActive(true);
        }
    }
}

