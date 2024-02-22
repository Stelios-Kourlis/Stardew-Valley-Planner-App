using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FishPondBottom : Building {
    // public FishPondBottom(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public FishPondBottom() : base(){
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 5;
        texture = Resources.Load("Buildings/Fish Pond Bottom") as Texture2D;
    }
}
