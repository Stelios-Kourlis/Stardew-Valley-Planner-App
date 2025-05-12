using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static HouseExtensionsComponent;

public abstract record UserActionRecord() {
    public abstract GameObject GetEntryInfoAsGameObject();
    public GameObject EntryPrefab => Resources.Load<GameObject>("UI/UserActionEntry");
}

public record BuildingPlaceRecord(IEnumerable<BuildingData> BuildingData) : UserActionRecord {
    public BuildingPlaceRecord(BuildingData buildingData)
        : this(new List<BuildingData> { buildingData }) { }

    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Building Placement";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Placed {BuildingData.Count()}x {BuildingData.First().buildingType}";
        return entry;
    }
}


public record BuildingDeleteRecord(IEnumerable<BuildingData> BuildingData) : UserActionRecord {
    public BuildingDeleteRecord(BuildingData buildingData)
        : this(new List<BuildingData> { buildingData }) { }

    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Building Deletion";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Deleted {BuildingData.Count()}x Buildings";
        return entry;
    }
}

public record BuildingPickupRecord(BuildingData BuildingData) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Building Pickup";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Picked Up {BuildingData.buildingType}";
        return entry;
    }

}

public record WallpaperChangeRecord(WallsComponent WallsComponent, Vector3Int WallPoint, (int OldWallpaper, int NewWallpaper) TextureChange) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        BuildingType buildingType = BuildingSaverLoader.Instance.SaveBuilding(WallsComponent.GetComponent<Building>()).buildingType;
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Wallpaper Change";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Changed Wallpaper from number {TextureChange.OldWallpaper} to number {TextureChange.NewWallpaper} at ({WallPoint.x},{WallPoint.y}) in {buildingType}";
        return entry;
    }
}

public record FlooringChangeRecord(FlooringComponent FlooringComponent, Vector3Int FlooringPoint, (int OldFloorTexture, int NewFloorTexture) TextureChange) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        BuildingType buildingType = BuildingSaverLoader.Instance.SaveBuilding(FlooringComponent.GetComponent<Building>()).buildingType;
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Flooring Change";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Changed Flooring from number {TextureChange.OldFloorTexture} to number {TextureChange.NewFloorTexture} at ({FlooringPoint.x},{FlooringPoint.y}) in {buildingType}";
        return entry;
    }
}

public record HouseRenovationRecord(HouseModificationMenu HouseModificationMenu, HouseModifications ExtensionChanged, bool NewExtensionStatus, IEnumerable<BuildingData> BuildingsDeleted) : UserActionRecord {
    public HouseRenovationRecord(HouseModificationMenu houseModificationMenu, HouseModifications extensionChanged, bool newStatus)
        : this(houseModificationMenu, extensionChanged, newStatus, Enumerable.Empty<BuildingData>()) { }

    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        // BuildingType buildingType = BuildingSaverLoader.Instance.SaveBuilding(FlooringComponent.GetComponent<Building>()).buildingType;
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "House Renovation Change";
        // entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Changed Flooring from number {TextureChange.OldFloorTexture} to number {TextureChange.NewFloorTexture} at ({FlooringPoint.x},{FlooringPoint.y}) in {buildingType}";
        return entry;
    }
}

//TODO: spouse change might delete buildings, add it
public record SpouseChangeRecord(HouseModificationMenu HouseModificationMenu, (MarriageCandidate OldSpouse, MarriageCandidate NewSpouse) SpouseChange) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        // BuildingType buildingType = BuildingSaverLoader.Instance.SaveBuilding(FlooringComponent.GetComponent<Building>()).buildingType;
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Spouse Change";
        // entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Changed Flooring from number {TextureChange.OldFloorTexture} to number {TextureChange.NewFloorTexture} at ({FlooringPoint.x},{FlooringPoint.y}) in {buildingType}";
        return entry;
    }
}

public record BuildingTierChangeRecord(TieredBuildingComponent TieredBuildingComponent, (int OldTier, int NewTier) TierChange, IEnumerable<BuildingData> InteriorBuildingsDeleted, IEnumerable<Animals> AnimalsRemoved) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        BuildingType buildingType = BuildingSaverLoader.Instance.SaveBuilding(TieredBuildingComponent.GetComponent<Building>()).buildingType;
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Tier Change";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Changed {buildingType} Tier from {TierChange.OldTier} to {TierChange.NewTier}";
        return entry;
    }
}

public record MushroomCaveMushroomChangeRecord(CaveComponent MushroomCaveComponent, bool HasMushrooms) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Farm Cave Change";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Changed Cave to {(HasMushrooms ? "have" : "not have")} have mushrooms";
        return entry;
    }
}

public record AnimalChangeRecord(AnimalHouseComponent AnimalHouseComponent, Animals AnimalType, bool IsAddition) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        BuildingType buildingType = BuildingSaverLoader.Instance.SaveBuilding(AnimalHouseComponent.GetComponent<Building>()).buildingType;
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Animal Change";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"{(IsAddition ? "Added" : "Removed")} {AnimalType} from {buildingType}";
        return entry;
    }
}

public record FishChangeRecord(FishPondComponent FishPondComponent, (Fish PreviousFish, Fish NewFish) FishType) : UserActionRecord {
    public override GameObject GetEntryInfoAsGameObject() {
        GameObject entry = Object.Instantiate(EntryPrefab);
        entry.transform.Find("Text").Find("Type").GetComponent<TMP_Text>().text = "Fish Change";
        entry.transform.Find("Text").Find("Details").GetComponent<TMP_Text>().text = $"Changed Fish from {FishType.PreviousFish} to {FishType.NewFish}";
        return entry;
    }
}


