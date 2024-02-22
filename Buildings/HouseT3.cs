using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseT3 : House {
    // public HouseT3(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public HouseT3() : base() {
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        texture = Resources.Load("Buildings/House3") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/Barn1") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 10_000 + 50_000},
            {Materials.Wood, 450},
            {Materials.Hardwood, 150},
        };
        base.Init();
    }
}
