﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isFull;
    public int cellIndex;
    public Piece pieceHeld;
    public bool isLimited;
    public GameObject lockSprite;
    public Transform rightParticleZone, leftParticleZone;

    public void AddPiece(Transform followerTarget, bool isNew)
    {
        isFull = true;
        followerTarget.SetParent(transform);
        followerTarget.position = new Vector3(followerTarget.parent.position.x, followerTarget.parent.position.y, followerTarget.parent.position.z + 0.1f);
        followerTarget.rotation = followerTarget.parent.rotation;
        pieceHeld = followerTarget.GetComponent<Piece>();
        pieceHeld.partOfBoard = true;

        GameManager.Instance.connectionManager.subPiecesOnBoard[cellIndex * 2] = pieceHeld.leftChild;
        GameManager.Instance.connectionManager.subPiecesOnBoard[cellIndex * 2 + 1] = pieceHeld.rightChild;
        GameManager.Instance.connectionManager.FillSubPieceIndex();

        pieceHeld.leftChild.relevantSlice = GameManager.Instance.connectionManager.slicesOnBoard[Mathf.CeilToInt((float)pieceHeld.leftChild.subPieceIndex / 2)];

        if (pieceHeld.rightChild.subPieceIndex >= GameManager.Instance.connectionManager.lengthOfSubPieces - 1)
        {
            pieceHeld.rightChild.relevantSlice = GameManager.Instance.connectionManager.slicesOnBoard[0];
        }
        else
        {
            pieceHeld.rightChild.relevantSlice = GameManager.Instance.connectionManager.slicesOnBoard[Mathf.CeilToInt((float)pieceHeld.rightChild.subPieceIndex / 2)];
        }

        GameManager.Instance.connectionManager.CheckConnections(cellIndex);




        if (isNew)
        {
            GameManager.Instance.currentFilledCellCount++;

            if(GameManager.Instance.currentFilledCellCount == GameManager.Instance.currentLevel.cellsCountInLevel)
            {
                UIManager.Instance.ActivateCommitButton();
            }
        }

        //GameManager.Instance.cellManager.NeighborTest(cellIndex);
    }

    public void RemovePiece()
    {
        isFull = false;

        if (rightParticleZone.childCount > 0)
        {
            Destroy(rightParticleZone.GetChild(0).gameObject);
        }

        if (leftParticleZone.childCount > 0)
        {
            Destroy(leftParticleZone.GetChild(0).gameObject);
        }

        if (pieceHeld.leftChild.isBadConnection)
        {
            GameManager.Instance.unsuccessfullConnectionCount--;
            pieceHeld.leftChild.isBadConnection = false;
            int i = GameManager.Instance.connectionManager.CheckIntRange(pieceHeld.leftChild.subPieceIndex - 1);
            GameManager.Instance.connectionManager.subPiecesOnBoard[i].isBadConnection = false;

        }

        if (pieceHeld.rightChild.isBadConnection)
        {
            GameManager.Instance.unsuccessfullConnectionCount--;
            pieceHeld.rightChild.isBadConnection = false;
            int i = GameManager.Instance.connectionManager.CheckIntRange(pieceHeld.rightChild.subPieceIndex + 1);
            GameManager.Instance.connectionManager.subPiecesOnBoard[i].isBadConnection = false;
        }

        if (pieceHeld.rightChild.relevantSlice)
        {
            pieceHeld.rightChild.relevantSlice.fulfilledCondition = false;
            pieceHeld.leftChild.relevantSlice.fulfilledCondition = false;
            pieceHeld.rightChild.relevantSlice = null;
            pieceHeld.leftChild.relevantSlice = null;
        }
        GameManager.Instance.connectionManager.RemoveSubPieceIndex(pieceHeld.leftChild.subPieceIndex);
        GameManager.Instance.connectionManager.RemoveSubPieceIndex(pieceHeld.rightChild.subPieceIndex);
    }
}
