using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HouseExtensionsComponent;

public abstract record UserActionRecord();

public record BuildingPlaceRecord(IEnumerable<BuildingData> BuildingData) : UserActionRecord {
    public BuildingPlaceRecord(BuildingData buildingData)
        : this(new List<BuildingData> { buildingData }) { }
}


public record BuildingDeleteRecord(IEnumerable<BuildingData> BuildingData) : UserActionRecord {
    public BuildingDeleteRecord(BuildingData buildingData)
        : this(new List<BuildingData> { buildingData }) { }
}

public record BuildingPickupRecord(BuildingData BuildingData) : UserActionRecord;

public record WallpaperChangeRecord(WallsComponent WallsComponent, Vector3Int WallPoint, (int OldWallpaper, int NewWallpaper) TextureChange) : UserActionRecord;

public record FlooringChangeRecord(FlooringComponent FlooringComponent, Vector3Int FlooringPoint, (int OldFloorTexture, int NewFloorTexture) TextureChange) : UserActionRecord;

public record HouseRenovationRecord(HouseModificationMenu HouseModificationMenu, HouseModifications ExtensionChanged, bool NewExtensionStatus, IEnumerable<BuildingData> BuildingsDeleted) : UserActionRecord {
    public HouseRenovationRecord(HouseModificationMenu houseModificationMenu, HouseModifications extensionChanged, bool newStatus)
        : this(houseModificationMenu, extensionChanged, newStatus, Enumerable.Empty<BuildingData>()) { }
}

public record SpouseChangeRecord(HouseModificationMenu HouseModificationMenu, (MarriageCandidate OldSpouse, MarriageCandidate NewSpouse) SpouseChange) : UserActionRecord;

public record BuildingTierChangeRecord(BuildingData BuildingData, (int OldTier, int NewTier) TierChange) : UserActionRecord;

public record MushroomCaveMushroomChangeRecord(CaveComponent MushroomCaveComponent) : UserActionRecord;


