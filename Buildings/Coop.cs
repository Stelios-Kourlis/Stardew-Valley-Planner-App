using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Coop : Building, ITieredBuilding {
    private SpriteAtlas atlas;
    private int tier;

    protected override void Init(){
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
    }

    public new void Start(){
        Init();
        base.Start();
        atlas = Resources.Load("Buildings/CoopAtlas") as SpriteAtlas;
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);
        foreach (Sprite sprite in sprites){
            Debug.Log($"Name {sprite.name}, Height {sprite.rect.height /16}, Width {sprite.rect.width/16}");
        }
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        this.tier = tier;
        UpdateTexture(atlas.GetSprite($"CoopAtlas_{tier}"));
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return tier switch{
            1 => new Dictionary<Materials, int>{
                {Materials.Coins, 4_000},
                {Materials.Wood, 300},
                {Materials.Stone, 100}
            },
            2 => new Dictionary<Materials, int>{
                {Materials.Coins, 4_000 + 10_000},
                {Materials.Wood, 300 + 400},
                {Materials.Stone, 100 + 150}
            },
            3 => new Dictionary<Materials, int>{
                {Materials.Coins, 4_000 + 10_000 + 20_000},
                {Materials.Wood, 300 + 400 + 500},
                {Materials.Stone, 100 + 150 + 200}
            },
            _ => throw new System.ArgumentException($"Invalid tier {tier}")
        };
    }
}
