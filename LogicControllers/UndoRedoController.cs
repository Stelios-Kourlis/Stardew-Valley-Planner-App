using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UndoRedoController {
    private static readonly Stack<UserActionRecord> actionLog = new();
    private static readonly Stack<UserActionRecord> undoLog = new();
    public static bool ignoreAction = false;

    public static void AddActionToLog(UserActionRecord action) {
        if (ignoreAction) return;
        actionLog.Push(action);
        undoLog.Clear();
    }
    public static void UndoLastAction() {
        if (actionLog.Count == 0) { NotificationManager.Instance.SendNotification("Nothing to undo", NotificationManager.Icons.InfoIcon); return; }

        UserActionRecord lastAction = actionLog.Pop();
        Debug.Log($"Undoing action {lastAction}");
        undoLog.Push(lastAction);
        // Actions action = lastAction.Action;
        ignoreAction = true;
        BuildingController.IsLoadingSave = true;
        switch (lastAction) {
            case BuildingPlaceRecord buildingPlaceRecord:
                foreach (BuildingData buildingData in buildingPlaceRecord.BuildingData) BuildingController.FindAndDeleteBuilding(buildingData.lowerLeftCorner);
                break;
            case BuildingPickupRecord buildingPickupRecord:
                BuildingSaverLoader.Instance.LoadBuilding(buildingPickupRecord.BuildingData);
                break;
            case BuildingDeleteRecord buildingDeleteRecord:
                foreach (BuildingData buildingData in buildingDeleteRecord.BuildingData) BuildingSaverLoader.Instance.LoadBuilding(buildingData);
                break;
            case WallpaperChangeRecord wallpaperChangeRecord:
                wallpaperChangeRecord.WallsComponent.ApplyWallpaper(wallpaperChangeRecord.WallPoint, wallpaperChangeRecord.TextureChange.OldWallpaper);
                break;
            case FlooringChangeRecord flooringChangeRecord:
                flooringChangeRecord.FlooringComponent.ApplyFloorTexture(flooringChangeRecord.FlooringPoint, flooringChangeRecord.TextureChange.OldFloorTexture);
                break;
            case HouseRenovationRecord houseRenovationRecord:
                houseRenovationRecord.HouseModificationMenu.GetModificationToggle(houseRenovationRecord.ExtensionChanged).isOn = !houseRenovationRecord.NewExtensionStatus;
                foreach (BuildingData buildingData in houseRenovationRecord.BuildingsDeleted)
                    BuildingSaverLoader.Instance.LoadBuilding(buildingData);
                break;
            case SpouseChangeRecord spouseChangeRecord:
                spouseChangeRecord.HouseModificationMenu.SetSpouseDropdownValue((int)spouseChangeRecord.SpouseChange.OldSpouse);
                break;
            // case BuildingTierChangeRecord buildingTierChangeRecord:
            // buildingTierChangeRecord
            case MushroomCaveMushroomChangeRecord mushroomCaveMushroomChangeRecord:
                mushroomCaveMushroomChangeRecord.MushroomCaveComponent.ToggleMushroomBoxes();
                break;
            default:
                throw new ArgumentException($"Invalid Type {lastAction}");
        }

        ignoreAction = false;
        BuildingController.IsLoadingSave = false;
        // Debug.Log($"New last action is {actionLog.First().action + " " + actionLog.First().BuildingData.buildingType}");
        if (actionLog.Any()) if (actionLog.First() is BuildingPickupRecord) UndoLastAction();//edit is a 2 phase action, undo both phases

    }

    public static void RedoLastUndo() {
        if (undoLog.Count == 0) return;
        UserActionRecord lastAction = undoLog.Pop();
        actionLog.Push(lastAction);
        Debug.Log($"Redoing action {lastAction}");
        ignoreAction = true;
        BuildingController.IsLoadingSave = true;
        switch (lastAction) {
            case BuildingPlaceRecord buildingPlaceRecord:
                foreach (BuildingData buildingData in buildingPlaceRecord.BuildingData) BuildingSaverLoader.Instance.LoadBuilding(buildingData);
                break;
            case BuildingPickupRecord buildingPickupRecord:
                BuildingController.FindAndDeleteBuilding(buildingPickupRecord.BuildingData.lowerLeftCorner);
                break;
            case BuildingDeleteRecord buildingDeleteRecord:
                foreach (BuildingData buildingData in buildingDeleteRecord.BuildingData) BuildingController.FindAndDeleteBuilding(buildingData.lowerLeftCorner);
                break;
            case WallpaperChangeRecord wallpaperChangeRecord:
                wallpaperChangeRecord.WallsComponent.ApplyWallpaper(wallpaperChangeRecord.WallPoint, wallpaperChangeRecord.TextureChange.NewWallpaper);
                break;
            case FlooringChangeRecord flooringChangeRecord:
                flooringChangeRecord.FlooringComponent.ApplyFloorTexture(flooringChangeRecord.FlooringPoint, flooringChangeRecord.TextureChange.NewFloorTexture);
                break;
            case HouseRenovationRecord houseRenovationRecord:
                foreach (BuildingData buildingData in houseRenovationRecord.BuildingsDeleted)
                    BuildingController.FindAndDeleteBuilding(buildingData.lowerLeftCorner);
                houseRenovationRecord.HouseModificationMenu.GetModificationToggle(houseRenovationRecord.ExtensionChanged).isOn = houseRenovationRecord.NewExtensionStatus;
                break;
            case SpouseChangeRecord spouseChangeRecord:
                spouseChangeRecord.HouseModificationMenu.SetSpouseDropdownValue((int)spouseChangeRecord.SpouseChange.NewSpouse);
                break;
            default:
                throw new ArgumentException($"Invalid action {lastAction}");
        }
        ignoreAction = false;
        BuildingController.IsLoadingSave = false;
        if (undoLog.Any()) if (lastAction is BuildingPickupRecord) RedoLastUndo();//edit is a 2 phase action, redo both phases
    }

    public static void ClearLogs() {
        actionLog.Clear();
        undoLog.Clear();
    }
}
