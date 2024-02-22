using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SprinklerT1 : Sprinkler
{
    // public SprinklerT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public SprinklerT1() : base() {
    //     Init();
    // }

    // public SprinklerT1(Vector3Int position, Tilemap tilemap) : base(position, tilemap) {
    //     Init();
    // }

    protected override void Init(){
        texture = Resources.Load("Buildings/Sprinkler1") as Texture2D;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.CopperBar, 1},
            {Materials.IronBar, 1},
        };
        base.Init();
    }
}
