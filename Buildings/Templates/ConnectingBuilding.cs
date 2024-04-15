using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utility.TilemapManager;

public enum ConnectFlag {
    TOP_ATTACHED = 1,
    BOTTOM_ATTACHED = 2,
    LEFT_ATTACHED = 4,
    RIGHT_ATTACHED = 8,
}

public class ConnectingBuilding{
    public int GetConnectingFlags(Vector3Int position, List<Vector3Int> otherBuildings){
        List<ConnectFlag> flags = new();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (otherBuildings.Contains(neighbors[0])) flags.Add(ConnectFlag.LEFT_ATTACHED);
        if (otherBuildings.Contains(neighbors[1])) flags.Add(ConnectFlag.RIGHT_ATTACHED);
        if (otherBuildings.Contains(neighbors[2])) flags.Add(ConnectFlag.BOTTOM_ATTACHED);
        if (otherBuildings.Contains(neighbors[3])) flags.Add(ConnectFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }

    public int GetConnectingFlagsNoTop(Vector3Int position, List<Vector3Int> otherBuildings){
        List<ConnectFlag> flags = new();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (otherBuildings.Contains(neighbors[0])) flags.Add(ConnectFlag.LEFT_ATTACHED);
        if (otherBuildings.Contains(neighbors[1])) flags.Add(ConnectFlag.RIGHT_ATTACHED);
        if (otherBuildings.Contains(neighbors[2])) flags.Add(ConnectFlag.BOTTOM_ATTACHED);
        return flags.Cast<int>().Sum();
    }
}
