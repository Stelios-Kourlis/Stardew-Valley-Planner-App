using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mill : Building {
        

    public new void Start(){
        name = GetType().Name;
        baseHeight = 2;
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 2_500},
            {Materials.Stone, 50},
            {Materials.Wood, 150},
            {Materials.Cloth, 4}
        };
    }
}

