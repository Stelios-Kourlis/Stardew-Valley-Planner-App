using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class House : Building, ITieredBuilding, IEnterableBuilding {
    public TieredBuildingComponent TieredBuildingComponent { get; private set; }
    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }
    public int Tier => gameObject.GetComponent<TieredBuildingComponent>().Tier;

    public List<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public int MaxTier => gameObject.GetComponent<TieredBuildingComponent>().MaxTier;

    public Vector3Int[] InteriorUnavailableCoordinates { get; private set; }

    public Vector3Int[] InteriorPlantableCoordinates { get; private set; }

    public override void OnAwake() {
        BaseHeight = 6;
        BuildingName = "House";
        base.OnAwake();
        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>();
    }

    public void SetTier(int tier) {
        TieredBuildingComponent.SetTier(tier);
        // if (tier == 4) UpdateTexture(TieredBuildingComponent.Atlas.GetSprite($"House3"));//4 has same sprite as 3
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return Tier switch {
            1 => new List<MaterialInfo> { },
            2 => new List<MaterialInfo>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            },
            3 => new List<MaterialInfo>{
                new(65_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            4 => new List<MaterialInfo>{
                new(160_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public string AddToBuildingData() {
        return $"{Tier}";
    }

    public void LoadExtraBuildingData(string[] data) {
        SetTier(int.Parse(data[0]));
    }

    public void ToggleEditBuildingInterior() {
        throw new System.NotImplementedException();
    }

    public void EditBuildingInterior() {
        throw new System.NotImplementedException();
    }

    public void ExitBuildingInteriorEditing() {
        throw new System.NotImplementedException();
    }

    public void CreateInteriorCoordinates() {
        throw new System.NotImplementedException();
    }

    public void OnMouseRightClick() {
        ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
