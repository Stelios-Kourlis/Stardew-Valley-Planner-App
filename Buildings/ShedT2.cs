using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShedT2 : Shed {
    public ShedT2(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public ShedT2() : base(){
        Init();
    }

    protected override void Init() {
        name = GetType().Name;
        texture = Resources.Load("Buildings/Big Shed") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/Shed2") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 15_000 + 20_000},
            {Materials.Wood, 300 + 550},
            {Materials.Stone, 300}
        };
        base.Init();
    }
}