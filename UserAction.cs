using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAction
{
    public readonly Actions action;
    public readonly Building building;
    public readonly Vector3Int position;
    public readonly string buildingData;

    public UserAction(Actions action, Building building, Vector3Int position) {
        this.action = action;
        this.position = position;
        this.building = building;
    }

    public UserAction(Actions action, string buildingData) {
        this.action = action;
        this.buildingData = buildingData;
    }

    public override string ToString() {
        return $"Action: {action} Building: {building} Position: {position}";
    }
}
