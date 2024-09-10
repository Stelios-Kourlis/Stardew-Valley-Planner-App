using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using System.Collections.ObjectModel;
using System.Linq;

public class Greenhouse : Building, IExtraActionBuilding {

    private GameObject porchTilemapObject;
    private Sprite porchSprite;
    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Greenhouse";
        BaseHeight = 6;
        base.OnAwake();
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>().AddInteriorInteractions(new HashSet<ButtonTypes>()); //no interior interactions
        porchSprite = Resources.Load<Sprite>("Buildings/GreenhousePorch");
        porchTilemapObject = CreateTilemapObject(transform, 0, "Porch");
        HidBuildingPreview += NoPreviewPortch;
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        Vector3Int porchBottomRight = position + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
        Vector3Int[] porchCoordinates = GetRectAreaFromPoint(porchBottomRight, 2, 3).ToArray();
        HashSet<Vector3Int> unavailableCoordinates = BuildingController.GetUnavailableCoordinates();
        if (unavailableCoordinates.Intersect(porchCoordinates).Count() > 0) return;
        porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        porchTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
    }

    public void PerformExtraActionsOnPickup() {
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    public void NoPreviewPortch() {
        // Debug.Log($"NoPreview on {BuildingName} (State = {CurrentBuildingState})");
        if (CurrentBuildingState == BuildingState.PLACED) porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        else porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    public void PerformExtraActionsOnPlacePreview(Vector3Int position) {
        Vector3Int porchBottomRight = position + new Vector3Int(2, -2, 0);
        Vector3Int[] porchCoordinates = GetRectAreaFromPoint(porchBottomRight, 2, 3).ToArray();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        porchTilemapObject.GetComponent<Tilemap>().color = Tilemap.color;
    }

    public void PerformExtraActionsOnDeletePreview() {
        porchTilemapObject.GetComponent<Tilemap>().color = Tilemap.color;
    }

    public void PerformExtraActionsOnPickupPreview() {
        porchTilemapObject.GetComponent<Tilemap>().color = Tilemap.color;
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry>{
            new("Complete the community center's pantry room.")
        };
    }

    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();
    public void OnMouseRightClick() {
        ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeInHierarchy);
    }
}
