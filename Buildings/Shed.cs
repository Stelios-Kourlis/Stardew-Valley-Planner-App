using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using UnityEngine.U2D;

public class Shed : Building, ITieredBuilding, IEnterableBuilding {
    public TieredBuildingComponent TieredBuildingComponent { get; private set; }
    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }
    public int Tier => gameObject.GetComponent<TieredBuildingComponent>().Tier;
    public HashSet<Vector3Int> InteriorUnavailableCoordinates { get; private set; }

    public HashSet<Vector3Int> InteriorPlantableCoordinates { get; private set; }

    public int MaxTier => gameObject.GetComponent<TieredBuildingComponent>().MaxTier;

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Shed";
        BaseHeight = 3;
        base.OnAwake();

        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(2);
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>();

    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return Tier switch {
            1 => new List<MaterialCostEntry>{
                new(15000, Materials.Coins),
                new(300, Materials.Wood),
            },
            2 => new List<MaterialCostEntry>{
                new(35000, Materials.Coins),
                new(850, Materials.Wood),
                new(300, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public string GetExtraData() {
        return $"{Tier}";
    }

    public void LoadExtraBuildingData(string[] data) {
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
    }

    public void SetTier(int tier) {
        TieredBuildingComponent.SetTier(tier);
        EnterableBuildingComponent.interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{BuildingName}{Tier}");
        Debug.Log($"BuildingInsides/{BuildingName}{Tier}");
    }

    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();

    public void OnMouseRightClick() {
        ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
