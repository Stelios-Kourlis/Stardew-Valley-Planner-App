using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;

public class Sprinkler : Building, ITieredBuilding, IMultipleTypeBuilding<Sprinkler.Types>, IRangeEffectBuilding{//todo maybe switch addons and types so it displayer on type bar

    public enum Types{
        Normal,
        PressureNozzle,
        Enricher
    }
    public override string TooltipMessage => "Right Click For More Options";
    public MultipleTypeBuilding<Types> MultipleBuildingComponent {get; private set;}
    public SpriteAtlas Atlas => MultipleBuildingComponent != null ? MultipleBuildingComponent.Atlas : TieredBuildingComponent.Atlas;
    public TieredBuilding TieredBuildingComponent {get; private set;}
    public RangeEffectBuilding RangeEffectBuildingComponent {get; private set;}
    public int Tier {get; private set;}
    public Types Type {get; private set;}
    public Types CurrentType {get; private set;}

    public override void OnAwake(){
        BaseHeight = 1;
        base.OnAwake();
        MultipleBuildingComponent = new MultipleTypeBuilding<Types>(this);
        SetType(CurrentType);
        TieredBuildingComponent = new TieredBuilding(this, 3);
        RangeEffectBuildingComponent = new RangeEffectBuilding(this);
        SetTier(1);

        Sprite[] sprites = new Sprite[Atlas.spriteCount];
        Atlas.GetSprites(sprites);

        foreach (Sprite sprite in sprites){
            Debug.Log(sprite.name);
            if (sprite.name == $"{GetType()}{TieredBuildingComponent.Tier}{MultipleBuildingComponent?.Type ?? Types.Normal}") Debug.Log("MATCH");
        }
    }

    public void SetTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier for {GetType()} must be between 1 and 3 (got {tier})");
        Tier = tier;
        Debug.Log($"{GetType()}{tier}{MultipleBuildingComponent?.Type ?? Types.Normal}");
        Debug.Log(Atlas == null);
        UpdateTexture(Atlas.GetSprite($"{GetType()}{tier}{MultipleBuildingComponent?.Type ?? Types.Normal}"));
    }

        protected override void PlacePreview(Vector3Int position){
            base.PlacePreview(position);
            Vector3Int[] coverageArea = Tier switch{
                1 => GetCrossAroundPosition(position).ToArray(),
                2 => GetAreaAroundPosition(position, 1).ToArray(),
                3 => GetAreaAroundPosition(position, 2).ToArray(),
                _ => throw new System.ArgumentException($"Invalid tier {Tier}")
            };
            RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
        }

        public override void Place(Vector3Int position){
            base.Place(position);
            RangeEffectBuildingComponent.HideEffectRange();
        }

        public override List<MaterialInfo> GetMaterialsNeeded(){
            return Tier switch{
                1 => new List<MaterialInfo>{
                    new(1, Materials.IronBar),
                    new(1, Materials.CopperBar),
                },
                2 => new List<MaterialInfo>{
                    new(1, Materials.GoldBar),
                    new(1, Materials.IronBar),
                    new(1, Materials.RefinedQuartz)
                },
                3 => new List<MaterialInfo>{
                    new(1, Materials.IridiumBar),
                    new(1, Materials.GoldBar),
                    new(1, Materials.BatteryPack),
                },
                _ => throw new System.ArgumentException($"Invalid tier {Tier}")
            };
        }

        public override string GetBuildingData(){
            return base.GetBuildingData() + $"|{Tier}";
        }

        public override void RecreateBuildingForData(int x, int y, params string[] data){
            OnAwake();
            Place(new Vector3Int(x,y,0));
            SetTier(int.Parse(data[0]));
        }

        protected override void OnMouseRightClick(){
            CycleType();
        }

    public void ShowEffectRange(Vector3Int[] RangeArea) => RangeEffectBuildingComponent.ShowEffectRange(RangeArea);

    public void HideEffectRange() => RangeEffectBuildingComponent.HideEffectRange();

    public void CycleType(){
        int enumLength = Enum.GetValues(typeof(Types)).Length;
        int intValue = Convert.ToInt32(Type);
        intValue = (intValue + 1) % enumLength;
        SetType((Types)Enum.ToObject(typeof(Types), intValue));
    }

    public void SetType(Types type){
        Type = type;
        CurrentType = type;
        sprite = Atlas.GetSprite($"{GetType()}{TieredBuildingComponent?.Tier ?? 1}{type}");
        if (sprite == null){
            Debug.Log($"{GetType()}{TieredBuildingComponent?.Tier ?? 1}{type} not found");
            Debug.Log(Atlas == null);
        }
        UpdateTexture(sprite);
    }

    public GameObject[] CreateButtonsForAllTypes(){
        return null;//todo should I choose from befire or click to cycle?
    }
}
