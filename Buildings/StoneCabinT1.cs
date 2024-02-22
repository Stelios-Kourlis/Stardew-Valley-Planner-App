using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StoneCabinT1 : Cabin {
    public StoneCabinT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        name = GetType().Name;
        texture = Resources.Load("Buildings/StoneCabin1") as Texture2D;
    }
}
