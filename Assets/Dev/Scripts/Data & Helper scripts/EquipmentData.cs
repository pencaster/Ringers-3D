using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//public enum slotType
//{
//    All,
//    Head,
//    MainHand,
//    Ring
//}

[Serializable]
public class EquipmentData 
{
    public string name;
    //public slotType slot;
    public PowerUp power;
    public PieceSymbol specificSymbol;
    public PieceColor specificColor;
    public int numOfUses;
    public int scopeOfUses;
    public int timeForCooldown;
    public string mats;
    public string spritePath;
    public string nextTimeAvailable /*= DateTime.Now.AddMinutes(15).ToString()*/;

    public EquipmentData(string equipName, int equiptype, int equippower, int equipnumUses, string equipmatList, string path)
    {
        name = equipName;
        //slot = (slotType)equiptype;
        power = (PowerUp)equippower;
        numOfUses = equipnumUses;
        mats = equipmatList;
        spritePath = path;
    }
    public EquipmentData() //// Override for an empty constructor
    {

    }
}
