using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mill : Building {
    // public Mill(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public Mill() : base() {
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 2;
        texture = Resources.Load("Buildings/Mill") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 2_500},
            {Materials.Stone, 50},
            {Materials.Wood, 150},
            {Materials.Cloth, 4}
        };
    }
}

