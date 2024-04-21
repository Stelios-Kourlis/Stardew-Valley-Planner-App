using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using UnityEngine.UI;
using System.Linq;

public class Sprinkler : Building, IMultipleTypeBuilding<Sprinkler.Types>, IRangeEffectBuilding{

    public enum Types{
        Normal,
        Quality,
        Iridium,
        // NormalPressureNozzle,
        // QualityPressureNozzle,
        // IridiumPressureNozzle,
        // NormalEnricher,
        // QualityEnricher,
        // IridiumEnricher
    }
    public override string TooltipMessage => "Right Click For More Options";
    public MultipleTypeBuilding<Types> MultipleBuildingComponent {get; private set;}
    public SpriteAtlas Atlas => MultipleBuildingComponent.Atlas;
    public RangeEffectBuilding RangeEffectBuildingComponent {get; private set;}
    public Types CurrentType {get; private set;}
    public Types Type => MultipleBuildingComponent.Type;
    private bool hasPressureNozzle;
    private bool hasEnricher;

    public override void OnAwake(){
        BaseHeight = 1;
        base.OnAwake();
        MultipleBuildingComponent = new MultipleTypeBuilding<Types>(this);
        RangeEffectBuildingComponent = new RangeEffectBuilding(this);
    }

        protected override void PlacePreview(Vector3Int position){
            base.PlacePreview(position);
            Vector3Int[] coverageArea = MultipleBuildingComponent.Type switch{
                Types.Normal => !hasPressureNozzle ? GetCrossAroundPosition(position).ToArray() : GetAreaAroundPosition(position, 1).ToArray(),
                Types.Quality => !hasPressureNozzle ? GetAreaAroundPosition(position, 1).ToArray() : GetAreaAroundPosition(position, 2).ToArray(),
                Types.Iridium => !hasPressureNozzle ? GetAreaAroundPosition(position, 2).ToArray() : GetAreaAroundPosition(position, 3).ToArray(),
                _ => throw new System.ArgumentException($"Invalid type {MultipleBuildingComponent.Type}")
            };
            RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
        }

        public override void Place(Vector3Int position){
            base.Place(position);
            RangeEffectBuildingComponent.HideEffectRange();
        }

        public override List<MaterialInfo> GetMaterialsNeeded(){
            return MultipleBuildingComponent.Type switch{
                Types.Normal => new List<MaterialInfo>{
                    new(1, Materials.IronBar),
                    new(1, Materials.CopperBar),
                },
                Types.Quality => new List<MaterialInfo>{
                    new(1, Materials.GoldBar),
                    new(1, Materials.IronBar),
                    new(1, Materials.RefinedQuartz)
                },
                Types.Iridium => new List<MaterialInfo>{
                    new(1, Materials.IridiumBar),
                    new(1, Materials.GoldBar),
                    new(1, Materials.BatteryPack),
                },
                _ => throw new System.ArgumentException($"Invalid type {MultipleBuildingComponent.Type}")
            };
        }

        public override string GetBuildingData(){
            return base.GetBuildingData() + $"|{MultipleBuildingComponent.Type}";
        }

        public override void RecreateBuildingForData(int x, int y, params string[] data){
            OnAwake();
            Place(new Vector3Int(x,y,0));
            SetType((Types)Enum.Parse(typeof(Types), data[0]));
        }

        protected override void OnMouseRightClick(){
            if (hasPressureNozzle){
                hasPressureNozzle = false;
                hasEnricher = true;
                UpdateTexture(Atlas.GetSprite($"{Type}Enricher"));
            }else if (hasEnricher){
                hasEnricher = false;
                UpdateTexture(Atlas.GetSprite($"{Type}"));
            }else{
                hasPressureNozzle = true;
                UpdateTexture(Atlas.GetSprite($"{Type}PressureNozzle"));
            }
            OnMouseEnter();
        }

    public void ShowEffectRange(Vector3Int[] RangeArea) => RangeEffectBuildingComponent.ShowEffectRange(RangeArea);

    public void HideEffectRange() => RangeEffectBuildingComponent.HideEffectRange();
    public void CycleType() => MultipleBuildingComponent.CycleType();
    public void SetType(Types type) => MultipleBuildingComponent.SetType(type);
    public GameObject[] CreateButtonsForAllTypes() => MultipleBuildingComponent.CreateButtonsForAllTypes();

    protected override void OnMouseEnter(){
        Vector3Int position = BaseCoordinates[0];
        Vector3Int[] coverageArea = MultipleBuildingComponent.Type switch{
            Types.Normal => !hasPressureNozzle ? GetCrossAroundPosition(position).ToArray() : GetAreaAroundPosition(position, 1).ToArray(),
            Types.Quality => !hasPressureNozzle ? GetAreaAroundPosition(position, 1).ToArray() : GetAreaAroundPosition(position, 2).ToArray(),
            Types.Iridium => !hasPressureNozzle ? GetAreaAroundPosition(position, 2).ToArray() : GetAreaAroundPosition(position, 3).ToArray(),
            _ => throw new System.ArgumentException($"Invalid type {MultipleBuildingComponent.Type}")
        };
        RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
    }

    protected override void OnMouseExit(){
        RangeEffectBuildingComponent.HideEffectRange();
    }
}
