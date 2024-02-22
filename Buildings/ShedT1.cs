using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShedT1 : Shed {
    // public ShedT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public ShedT1() : base(){
    //     Init();
    // }

    protected override void Init() {
        name = GetType().Name;
        texture = Resources.Load("Buildings/Shed") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/Shed1") as Texture2D;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 15_000},
            {Materials.Wood, 300}
        };
        base.Init();
    }
}
