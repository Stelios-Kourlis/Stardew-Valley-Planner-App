using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DesertObelisk : Building {
    public DesertObelisk(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public DesertObelisk() : base(){
        Init();
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
        texture = Resources.Load("Buildings/Desert Obelisk") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int> {
            {Materials.Coins, 1_000_000},
            {Materials.IridiumBar, 20},
            {Materials.Coconut, 10},
            {Materials.CactusFruit, 10}
        };
    }
        
}
