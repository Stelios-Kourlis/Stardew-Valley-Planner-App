using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JunimoHut : Building {

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 2;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 20_000},
            {Materials.Stone, 200},
            {Materials.Starfruit, 9},
            {Materials.Fiber, 100}
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }
}