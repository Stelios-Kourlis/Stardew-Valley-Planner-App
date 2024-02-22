using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class House : Building {//todo There is a T4 house with the cellar
    // public House(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public House() : base() {
    //     Init();
    // }

    protected override void Init(){
        baseHeight = 6;
        _buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER
        };
    }
}
