using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EarthObelisk : Building {
    // public EarthObelisk(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public EarthObelisk() : base(){
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
        texture = Resources.Load("Buildings/Earth Obelisk") as Texture2D;
        materialsNeeded = new Dictionary<Materials, int> {
            {Materials.Coins, 500_000},
            {Materials.IridiumBar, 10},
            {Materials.EarthCrystal, 10}
        };
    }
        
}
