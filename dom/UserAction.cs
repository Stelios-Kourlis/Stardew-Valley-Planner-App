using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAction {
    public readonly Actions action;
    public BuildingData BuildingData { get; private set; }
    public bool IsMassAction { get; set; }

    public UserAction(Actions action, BuildingData data) {
        this.action = action;
        BuildingData = data;
    }

    public Vector3Int GetLowerLeftCorner() {
        return BuildingData.lowerLeftCorner;
    }

    public UserAction FlipAction() {
        if (action == Actions.PLACE) return new UserAction(Actions.DELETE, BuildingData);
        else if (action == Actions.DELETE) return new UserAction(Actions.PLACE, BuildingData);
        else if (action == Actions.EDIT) return new UserAction(Actions.PLACE, BuildingData);
        else throw new System.ArgumentException($"Invalid action {action}");
    }
}
