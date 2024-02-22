using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LogCabinT1 : Cabin {//todo all Cabins materials needed
    public LogCabinT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        name = GetType().Name;
        texture = Resources.Load("Buildings/LogCabin1") as Texture2D;
    }
}
