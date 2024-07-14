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
        Debug.Log($"Adding action {action.action} to log");
        actionLog.Push(action);
        undoLog.Clear();
    }
    public static void UndoLastAction() {
        Debug.Log("Current Action Log:");
        foreach (UserAction action_ in actionLog) Debug.Log(action_.action + " " + action_.BuildingData.type);
        Debug.Log("Undoing last action");
        if (actionLog.Count == 0) return;
        ignoreAction = true;
        UserAction lastAction = actionLog.Pop();
        undoLog.Push(lastAction);
        Actions action = lastAction.action;
        switch (action) {
            case Actions.PLACE:
                Debug.Log($"Deleting {lastAction.BuildingData.type}");
                Vector3Int lowerLeftCorner = lastAction.GetLowerLeftCorner();
                BuildingController.FindAndDeleteBuilding(lowerLeftCorner);
                break;
            case Actions.DELETE:
            case Actions.EDIT:
                Debug.Log($"Placing {lastAction.BuildingData.type} at {lastAction.GetLowerLeftCorner()}");
                BuildingController.PlaceSavedBuilding(lastAction.BuildingData);
                break;
            default:
                throw new ArgumentException($"Invalid action {action}");
        }
        ignoreAction = false;
        Debug.Log($"Last action was {action + " " + lastAction.BuildingData.type}");
        Debug.Log($"New last action is {actionLog.First().action + " " + actionLog.First().BuildingData.type}");
        if (actionLog.First().action == Actions.EDIT) {
            Debug.Log($"Last action was an edit, doing the other phase of the edit");
            UndoLastAction();//edit is a 2 phase action, undo both phases
        }

    }
    // P
    // P E
    public static void RedoLastUndo() {
        if (undoLog.Count == 0) return;
        UserAction lastAction = undoLog.Pop();
        actionLog.Push(lastAction);
        Actions action = lastAction.action;
        switch (action) {
            case Actions.PLACE:
                BuildingController.PlaceSavedBuilding(lastAction.BuildingData);
                break;
            case Actions.DELETE:
            case Actions.EDIT:
                Vector3Int lowerLeftCorner = lastAction.GetLowerLeftCorner();
                BuildingController.FindAndDeleteBuilding(lowerLeftCorner);
                break;
            default:
                throw new ArgumentException($"Invalid action {action}");
        }
        if (lastAction.action == Actions.EDIT) RedoLastUndo();//edit is a 2 phase action, redo both phases
    }
}
