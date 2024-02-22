using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlankCabinT1 : Cabin {
    public PlankCabinT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        name = GetType().Name;
        texture = Resources.Load("Buildings/PlankCabin1") as Texture2D;
    }
}
