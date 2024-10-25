using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HouseExtensionsComponent;
using static WallsComponent;

public class UserAction {
    public readonly Actions action;
    public List<BuildingData> BuildingData { get; private set; }

    public readonly (int, int) textureChange;
    public readonly Vector3Int textureApplyPoint;

    public readonly WallsComponent wallsComponent;
    public readonly FlooringComponent flooringComponent;

    public readonly HouseModificationMenu houseModificationMenu;
    public readonly HouseModifications extensionChanged;
    public readonly bool newExtensionStatus;
    public readonly int oldSpouse;
    public readonly int newSpouse;

    public bool IsMassAction { get; set; }

    public UserAction(Actions action, IEnumerable<BuildingData> data) {
        this.action = action;
        BuildingData = data.ToList();
    }

    public UserAction(Actions action, BuildingData data) {
        this.action = action;
        BuildingData = new List<BuildingData> { data };
    }

    public UserAction(WallsComponent wallsComponent, Vector3Int wallPoint, int oldWallpaper, int newWallpaper) {
        action = Actions.PLACE_WALLPAPER;
        this.wallsComponent = wallsComponent;
        textureApplyPoint = wallPoint;
        textureChange = (oldWallpaper, newWallpaper);
    }

    public UserAction(FlooringComponent flooringComponent, Vector3Int flooringPoint, int oldFloorTexture, int newFloorTexture) {
        action = Actions.PLACE_FLOORING;
        this.flooringComponent = flooringComponent;
        textureApplyPoint = flooringPoint;
        textureChange = (oldFloorTexture, newFloorTexture);
    }

    public UserAction(HouseModificationMenu modificationMenu, HouseModifications extensionChanged, bool newStatus) {
        action = Actions.HOUSE_RENOVATION;
        houseModificationMenu = modificationMenu;
        this.extensionChanged = extensionChanged;
        newExtensionStatus = newStatus;
        BuildingData = new();
    }

    public UserAction(HouseModificationMenu modificationMenu, HouseModifications extensionChanged, bool newStatus, IEnumerable<BuildingData> buildingsDeleted) {
        action = Actions.HOUSE_RENOVATION;
        houseModificationMenu = modificationMenu;
        this.extensionChanged = extensionChanged;
        newExtensionStatus = newStatus;
        BuildingData = buildingsDeleted.ToList();
    }

    public UserAction(HouseModificationMenu modificationMenu, MarriageCandidate oldSpouse, MarriageCandidate newSpouse) {
        action = Actions.CHANGE_SPOUSE;
        houseModificationMenu = modificationMenu;
        this.oldSpouse = (int)oldSpouse;
        this.newSpouse = (int)newSpouse;
    }

    public override string ToString() {
        return action switch {
            Actions.PLACE or Actions.EDIT or Actions.DELETE => $"{action} on {BuildingData.Count} {BuildingData.First().buildingType}",
            Actions.PLACE_WALLPAPER => $"Wallpaper Change from ID = {textureChange.Item1} to ID = {textureChange.Item2}",
            Actions.PLACE_FLOORING => $"Flooring Change from ID = {textureChange.Item1} to ID = {textureChange.Item2}",
            Actions.HOUSE_RENOVATION => $"House Renovation: {extensionChanged} to {newExtensionStatus}",
            Actions.CHANGE_SPOUSE => $"Change Spouse from {(MarriageCandidate)oldSpouse} to {(MarriageCandidate)newSpouse}",
            _ => throw new ArgumentException($"Invalid action {action}")
        };
    }
}