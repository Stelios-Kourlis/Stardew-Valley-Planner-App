using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using static Utility.TilemapManager;
using System.Runtime.Remoting.Messaging;
using System;

public class Scarecrow : Building/*, IMultipleTypeBuilding, IRangeEffectBuilding, IExtraActionBuilding*/ {

    // public enum Types {
    //     Scarecrow,
    //     Rarecrow1,
    //     Rarecrow2,
    //     Rarecrow3,
    //     Rarecrow4,
    //     Rarecrow5,
    //     Rarecrow6,
    //     Rarecrow7,
    //     Rarecrow8,
    //     DeluxeScarecrow
    // }
    // private bool IsDeluxe { get; set; } = false;

    // public override string TooltipMessage {
    //     get {
    //         if (!IsDeluxe) return "Right Click To Cycle Scarecrow Type";
    //         else return "";
    //     }
    // }

    // public MultipleTypeBuilding MultipleTypeBuildingComponent { get; private set; }

    // public RangeEffectBuilding RangeEffectBuildingComponent { get; private set; }

    // public Enum Type => MultipleTypeBuildingComponent.Type;

    // public override void OnAwake() {
    //     BuildingName = "Scarecrow";
    //     BaseHeight = 1;
    //     base.OnAwake();
    //     MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuilding>();
    //     RangeEffectBuildingComponent = new RangeEffectBuilding(this);
    // }

    // public void PerformExtraActionsOnPlacePreview(Vector3Int position) {
    //     Vector3Int[] coverageArea = MultipleTypeBuildingComponent.Type switch {
    //         Types.DeluxeScarecrow => GetRangeOfDeluxeScarecrow(position).ToArray(),
    //         _ => GetRangeOfScarecrow(position).ToArray()
    //     };
    //     RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
    // }

    // public void PerformExtraActionsOnPlace(Vector3Int position) {
    //     RangeEffectBuildingComponent.HideEffectRange();
    // }

    // public override List<MaterialInfo> GetMaterialsNeeded() {
    //     return MultipleTypeBuildingComponent.Type switch {
    //         Types.DeluxeScarecrow => new List<MaterialInfo>{//Deluxe scarecrow
    //             new(50, Materials.Wood),
    //             new(1, Materials.IridiumOre),
    //             new(40, Materials.Fiber)
    //         },
    //         Types.Scarecrow => new List<MaterialInfo>{//Normal scarecrow
    //             new(50, Materials.Wood),
    //             new(1, Materials.Coal),
    //             new(20, Materials.Fiber)
    //         },
    //         Types.Rarecrow1 => new List<MaterialInfo>{//Rarecrows in order
    //             new("Purchase at the Stardew Valley Fair for 800 Tokens"),
    //         },
    //         Types.Rarecrow2 => new List<MaterialInfo>{
    //             new("	Purchase at the Spirit's Eve festival for 5,000 Coins"),
    //         },
    //         Types.Rarecrow3 => new List<MaterialInfo>{
    //             new("Purchase at the Casino for 10,000 Qi Coins"),
    //         },
    //         Types.Rarecrow4 => new List<MaterialInfo>{
    //             new("Purchase at the Traveling Cart randomly during fall or winter for 4,000 Coins, or purchase at the Festival of Ice for 5,000 Coins"),
    //         },
    //         Types.Rarecrow5 => new List<MaterialInfo>{
    //             new("Purchase at the Flower Dance for 2,500 Coins"),
    //         },
    //         Types.Rarecrow6 => new List<MaterialInfo>{
    //             new("Purchase from the Dwarf for 2,500 Coins"),
    //         },
    //         Types.Rarecrow7 => new List<MaterialInfo>{
    //             new("Donate 20 Artifacts (not counting Minerals) to the Museum. Can be purchased from the Night Market once the first one is earned"),
    //         },
    //         Types.Rarecrow8 => new List<MaterialInfo>{
    //             new("Donate 40 items to the Museum. Can be purchased from the Night Market once the first one is earned"),
    //         },
    //         _ => throw new System.ArgumentException($"Invalid scarecrow type {MultipleTypeBuildingComponent.Type}")
    //     };
    // }

    // public string AddToBuildingData() {
    //     return $"{MultipleTypeBuildingComponent.Type}";
    // }

    // public void LoadExtraBuildingData(string[] data) {
    //     MultipleTypeBuildingComponent.SetType((Types)int.Parse(data[0]));
    // }

    // protected void OnMouseEnter() { //todo Add this
    //     Vector3Int lowerLeftCorner = BaseCoordinates[0];
    //     RangeEffectBuildingComponent.ShowEffectRange(GetAreaAroundPosition(new Vector3Int(lowerLeftCorner.x - 7, lowerLeftCorner.y - 8, 0), 17, 17).ToArray());
    // }

    // protected void OnMouseExit() { //todo Add this
    //     RangeEffectBuildingComponent.HideEffectRange();
    // }

    // public GameObject[] CreateButtonsForAllTypes() {
    //     return MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
    // }

    // public void CycleType() => MultipleTypeBuildingComponent.CycleType();

    // public void SetType(Enum type) => MultipleTypeBuildingComponent.SetType(type);
    // public void ShowEffectRange(Vector3Int[] RangeArea) => RangeEffectBuildingComponent.ShowEffectRange(RangeArea);
    // public void HideEffectRange() => RangeEffectBuildingComponent.HideEffectRange();
    public override List<MaterialInfo> GetMaterialsNeeded() {
        throw new NotImplementedException();
    }
}
