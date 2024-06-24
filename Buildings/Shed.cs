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
    public Vector3Int[] InteriorUnavailableCoordinates { get; private set; }

    public Vector3Int[] InteriorPlantableCoordinates { get; private set; }

    public int MaxTier => gameObject.GetComponent<TieredBuildingComponent>().MaxTier;

    public List<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Shed";
        BaseHeight = 3;

        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(2);
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>();
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return Tier switch {
            1 => new List<MaterialInfo>{
                new(15000, Materials.Coins),
                new(300, Materials.Wood),
            },
            2 => new List<MaterialInfo>{
                new(35000, Materials.Coins),
                new(850, Materials.Wood),
                new(300, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public string AddToBuildingData() {
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

    public void CreateInteriorCoordinates() {
        Vector3Int interiorLowerLeftCorner = EnterableBuildingComponent.InteriorAreaCoordinates[0];
        if (Tier == 1) {
            List<Vector3Int> interiorUnavailableCoordinates = new();
            for (int i = 0; i < 13; i++) {
                if (i != 6) interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 0, 0));
                interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 10, 0));
                interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 11, 0));
                interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 12, 0));
                interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 13, 0));
            }
            for (int i = 0; i < 13; i++) {
                interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(0, i, 0));
                interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(12, i, 0));
            }
            InteriorUnavailableCoordinates = GetAllInteriorUnavailableCoordinates(interiorUnavailableCoordinates.ToArray()).ToArray();
        }
        InteriorPlantableCoordinates = new Vector3Int[] { };
    }

    public void OnMouseRightClick() {
        ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
