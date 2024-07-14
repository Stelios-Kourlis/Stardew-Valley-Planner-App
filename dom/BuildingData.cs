using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData {
    public readonly Type type;
    public readonly Vector3Int lowerLeftCorner;
    public readonly string[] extraData;
    public BuildingData(Type type, Vector3Int lowerLeftCorner, string[] extraData = null) {
        this.type = type;
        this.lowerLeftCorner = lowerLeftCorner;
        this.extraData = extraData ?? (new string[0]);
    }

    public BuildingData(string stringData) {
        string[] data = stringData.Split('|');
        type = Type.GetType(data[0]);
        lowerLeftCorner = new Vector3Int(int.Parse(data[1]), int.Parse(data[2]), 0);
        extraData = data[3..];
    }

    public override string ToString() {
        return $"{type}|{lowerLeftCorner.x}|{lowerLeftCorner.y}|{string.Join("|", extraData)}";
    }
}
