using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class QBitData {
    public QBitType qType;
    public Color color;

    public QBitData(QBitType qType, Color color) {
        this.qType = qType;
        this.color = color;
    }
}


public enum QBitType {GREEN, RED, BLUE, NONE, DEFECT}