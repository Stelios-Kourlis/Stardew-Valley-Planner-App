using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Well : Building {
    public Well(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
       Init();
    }

    public Well() : base(){
        Init();
    }

    protected override void Init() {
        name = GetType().Name;
        baseHeight = 3;
        texture = Resources.Load("Buildings/Well") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 1_000},
            {Materials.Stone, 75},
        };
    }
}