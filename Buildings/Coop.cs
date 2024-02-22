using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Coop : Building {
    public Coop(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public Coop() : base(){
        Init();
    }

    protected override void Init(){
        baseHeight = 3;
        _buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
    }
}
