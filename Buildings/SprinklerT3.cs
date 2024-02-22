using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SprinklerT3 : Sprinkler
{
    // public SprinklerT3(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public SprinklerT3() : base() {
    //     Init();
    // }

    // public SprinklerT3(Vector3Int position, Tilemap tilemap) : base(position, tilemap) {
    //     Init();
    // }

    protected override void Init(){
        texture = Resources.Load("Buildings/Sprinkler3") as Texture2D;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.GoldBar, 1},
            {Materials.IridiumBar, 1},
            {Materials.BatteryPack, 1},
        };
        base.Init();
    }
}