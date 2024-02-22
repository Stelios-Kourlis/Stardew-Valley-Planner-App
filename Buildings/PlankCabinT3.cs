using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlankCabinT3 : Cabin {
    public PlankCabinT3(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        name = GetType().Name;
        texture = Resources.Load("Buildings/PlankCabin3") as Texture2D;
    }
}
