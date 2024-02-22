using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAction
{
    public readonly Actions action;
    public readonly Building building;
    public readonly Vector3Int position;
    public readonly Floor floor;

    public UserAction(Actions action, Building building, Vector3Int position) {
        this.action = action;
        this.position = position;
        this.building = building;
        floor = null;
    }

    public UserAction(Actions action, Floor floor, Vector3Int position) {
        this.action = action;
        this.position = position;
        building = null;
        this.floor = floor; 
    }

    public override string ToString() {
        return $"Action: {action} Building: {building} Floor: {floor} Position: {position}";
    }
}
