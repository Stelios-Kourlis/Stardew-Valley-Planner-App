using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UndoRedoController {
    private static readonly Stack<UserAction> actionLog = new();
    private static readonly Stack<UserAction> undoLog = new();
    public static bool ignoreAction = false;

    public static void AddActionToLog(UserAction action) {
        if (ignoreAction) return;
        actionLog.Push(action);
        undoLog.Clear();
    }
    public static void UndoLastAction() {
        if (actionLog.Count == 0) { NotificationManager.Instance.SendNotification("Nothing to undo", NotificationManager.Icons.InfoIcon); return; }

        UserAction lastAction = actionLog.Pop();
        Debug.Log($"Undoing action {lastAction}");
        undoLog.Push(lastAction);
        Actions action = lastAction.action;
        ignoreAction = true;
        BuildingController.IsLoadingSave = true;
        switch (action) {
            case Actions.PLACE:
                foreach (BuildingData buildingData in lastAction.BuildingData) BuildingController.FindAndDeleteBuilding(buildingData.lowerLeftCorner);
                break;
            case Actions.DELETE:
            case Actions.EDIT:
                foreach (BuildingData buildingData in lastAction.BuildingData) BuildingController.PlaceSavedBuilding(buildingData);
                // BuildingController.CreateNewBuilding();
                break;
            case Actions.PLACE_WALLPAPER:
                Debug.Log(lastAction.wallsComponent);
                lastAction.wallsComponent.ApplyWallpaper(lastAction.textureApplyPoint, lastAction.textureChange.Item1);
                break;
            case Actions.PLACE_FLOORING:
                lastAction.flooringComponent.ApplyFloorTexture(lastAction.textureApplyPoint, lastAction.textureChange.Item1);
                break;
            case Actions.HOUSE_RENOVATION:
                lastAction.houseModificationMenu.GetModificationToggle(lastAction.extensionChanged).isOn = !lastAction.newExtensionStatus;
                foreach (BuildingData buildingData in lastAction.BuildingData)
                    BuildingController.PlaceSavedBuilding(buildingData);
                break;
            case Actions.CHANGE_SPOUSE:
                lastAction.houseModificationMenu.SetSpouseDropdownValue(lastAction.oldSpouse);
                break;
            default:
                throw new ArgumentException($"Invalid action {action}");
        }

        ignoreAction = false;
        BuildingController.IsLoadingSave = false;
        // Debug.Log($"New last action is {actionLog.First().action + " " + actionLog.First().BuildingData.buildingType}");
        if (actionLog.Any()) if (actionLog.First().action == Actions.EDIT) UndoLastAction();//edit is a 2 phase action, undo both phases

    }

    public static void RedoLastUndo() {
        if (undoLog.Count == 0) return;
        UserAction lastAction = undoLog.Pop();
        actionLog.Push(lastAction);
        Debug.Log($"Redoing action {lastAction}");
        Actions action = lastAction.action;
        ignoreAction = true;
        BuildingController.IsLoadingSave = true;
        switch (action) {
            case Actions.PLACE:
                foreach (BuildingData buildingData in lastAction.BuildingData) BuildingController.PlaceSavedBuilding(buildingData);
                break;
            case Actions.DELETE:
            case Actions.EDIT:
                foreach (BuildingData buildingData in lastAction.BuildingData) BuildingController.FindAndDeleteBuilding(buildingData.lowerLeftCorner);
                break;
            case Actions.PLACE_WALLPAPER:
                lastAction.wallsComponent.ApplyWallpaper(lastAction.textureApplyPoint, lastAction.textureChange.Item2);
                break;
            case Actions.PLACE_FLOORING:
                lastAction.flooringComponent.ApplyFloorTexture(lastAction.textureApplyPoint, lastAction.textureChange.Item2);
                break;
            case Actions.HOUSE_RENOVATION:
                foreach (BuildingData buildingData in lastAction.BuildingData)
                    BuildingController.FindAndDeleteBuilding(buildingData.lowerLeftCorner);
                lastAction.houseModificationMenu.GetModificationToggle(lastAction.extensionChanged).isOn = lastAction.newExtensionStatus;
                break;
            case Actions.CHANGE_SPOUSE:
                lastAction.houseModificationMenu.SetSpouseDropdownValue(lastAction.newSpouse);
                break;
            default:
                throw new ArgumentException($"Invalid action {action}");
        }
        ignoreAction = false;
        BuildingController.IsLoadingSave = false;
        if (undoLog.Any()) if (lastAction.action == Actions.EDIT) RedoLastUndo();//edit is a 2 phase action, redo both phases
    }

    public static void ClearLogs() {
        actionLog.Clear();
        undoLog.Clear();
    }
}
