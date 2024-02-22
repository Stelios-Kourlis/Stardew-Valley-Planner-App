using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseT1 : House {
    // public HouseT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public HouseT1() : base() {
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        texture = Resources.Load("Buildings/House1") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/FarmHouse1") as Texture2D;
        base.Init();
    }
}
