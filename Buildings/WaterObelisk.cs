using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterObelisk : Building {
    // public WaterObelisk(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public WaterObelisk() : base(){
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
        texture = Resources.Load("Buildings/Water Obelisk") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 500_000},
            {Materials.IridiumBar, 5},
            {Materials.Clam, 10},
            {Materials.Coral, 10}
        };
    }
}