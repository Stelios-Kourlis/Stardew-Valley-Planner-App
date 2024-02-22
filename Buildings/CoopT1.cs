using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CoopT1 : Coop {
    // public CoopT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public CoopT1() : base() {
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        texture = Resources.Load("Buildings/Coop") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int> {
            {Materials.Coins, 4000},
            {Materials.Wood, 300},
            {Materials.Stone, 100},
        };
        base.Init();
    }
}

