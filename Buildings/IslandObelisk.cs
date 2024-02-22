using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandObelisk : Building {
    // public IslandObelisk(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public IslandObelisk() : base() {
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
        texture = Resources.Load("Buildings/Island Obelisk") as Texture2D;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 1_000_000},
            {Materials.IridiumBar, 10},
            {Materials.DragonTooth, 10},
            {Materials.Banana, 10}
        };
    }
}