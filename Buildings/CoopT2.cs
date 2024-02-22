using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CoopT2 : Coop {
    public CoopT2(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public CoopT2() : base() {
        Init();
    }

    protected override void Init(){
        name = GetType().Name;
        texture = Resources.Load("Buildings/Big Coop") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/Coop2") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int> {
            {Materials.Coins, 4000 + 10000},
            {Materials.Wood, 300 + 400},
            {Materials.Stone, 100 + 150}
        };
        base.Init();
    }
}