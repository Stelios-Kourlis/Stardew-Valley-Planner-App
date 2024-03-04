using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAction
{
    public readonly Actions action;
    public readonly int UID;
    public readonly string buildingData;

    public UserAction(Actions action, int UID, string data) {
        this.action = action;
        this.UID = UID;
        buildingData = data;
        
    }
}
