using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShippingBin : Building {
    public ShippingBin(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public ShippingBin() : base(){
        Init();
    }

    protected override void Init() {
        name = GetType().Name;
        baseHeight = 1;
        texture = Resources.Load("Buildings/Shipping Bin") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 250},
            {Materials.Wood, 150}
        };
    }
}