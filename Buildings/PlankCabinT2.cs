using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlankCabinT2 : Cabin {
    public PlankCabinT2(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        name = GetType().Name;
        texture = Resources.Load("Buildings/PlankCabin2") as Texture2D;
    }
}
