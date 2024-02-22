using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseT2 : House {
    public HouseT2(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public HouseT2() : base() {
        Init();
    }

    protected override void Init(){
        name = GetType().Name;
        texture = Resources.Load("Buildings/House2") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/FarmHouse2") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 10_000},
            {Materials.Wood, 450}
        };
        base.Init();
    }
}
