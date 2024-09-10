using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class MoveablePanel : MonoBehaviour {

    public enum PanelState {
        Open,
        Closed,
        Moving,
        Hidden
    }

    private enum MoveType {
        ConstantTime,
        ConstastSpeed
    }

    [SerializeField] private PanelState panelState;
    [SerializeField] private MoveType moveType;
    [SerializeField] private float moveSpeed, moveTime;
    public Action panelOpened, panelClosed;
    [SerializeField] Vector3 closedPosition, openPosition, hiddenPosition;
    [SerializeField] private Button[] toggleButtons, openButtons, closeButtons;
    [SerializeField] private readonly Sprite[] toggleButtonSprites = new Sprite[2];
    [SerializeField] private bool SetActionToNothingOnOpen = true;
    [SerializeField] private bool IsFullFocusPanel;
    private Actions actionBeforeOpeningPanel;
    private Vector3 CurrentPosition => gameObject.GetComponent<RectTransform>().localPosition;
    public static bool panelWithNoActionRequirementIsOpen;
    public static (bool, MoveablePanel) FullFocusPanelIsOpen;

    public void Start() {
        if (toggleButtons != null) foreach (Button button in toggleButtons) button.onClick.AddListener(() => TogglePanel(button.gameObject));
        if (openButtons != null) foreach (Button button in openButtons) button.onClick.AddListener(() => SetPanelToOpenPosition(button.gameObject));
        if (closeButtons != null) foreach (Button button in closeButtons) button.onClick.AddListener(() => SetPanelToClosedPosition(button.gameObject));

        panelState = PanelState.Closed;
        gameObject.GetComponent<RectTransform>().localPosition = hiddenPosition == Vector3.zero ? closedPosition : hiddenPosition;
        panelState = hiddenPosition == Vector3.zero ? PanelState.Closed : PanelState.Hidden;
    }

    public void TogglePanel(GameObject invoker = null) {
        switch (panelState) {
            case PanelState.Open:
                SetPanelToClosedPosition(invoker);
                break;
            case PanelState.Closed:
                SetPanelToOpenPosition(invoker);
                break;
            case PanelState.Moving:
                break;
        }
    }

    public void SetPanelToOpenPosition(GameObject invoker = null) {
        if (panelState == PanelState.Open || panelState == PanelState.Moving) return;
        panelState = PanelState.Moving;
        if (moveType == MoveType.ConstantTime) StartCoroutine(UIObjectMover.MoveObjectInConstantTime(transform, CurrentPosition, openPosition, moveTime));
        else StartCoroutine(UIObjectMover.MoveObjectWithConstastSpeed(transform, CurrentPosition, openPosition, moveSpeed));
        panelState = PanelState.Open;
        // Debug.Log(toggleButtonSprites[0]);
        if (invoker != null && toggleButtonSprites[0] != null) invoker.GetComponent<Image>().sprite = toggleButtonSprites[0];

        if (SetActionToNothingOnOpen) {
            actionBeforeOpeningPanel = BuildingController.CurrentAction;
            BuildingController.SetCurrentAction(Actions.DO_NOTHING);
            panelWithNoActionRequirementIsOpen = true;
        }

        if (IsFullFocusPanel) {
            // if (FullFocusPanelIsOpen.Item1) FullFocusPanelIsOpen.Item2.SetPanelToClosedPosition();
            FullFocusPanelIsOpen = (true, this);
        }

        panelOpened?.Invoke();
    }

    public void SetPanelToClosedPosition(GameObject invoker = null) {
        if (panelState == PanelState.Closed || panelState == PanelState.Moving) return;
        // Debug.Log($"Closing {gameObject.name}");
        panelState = PanelState.Moving;
        if (moveType == MoveType.ConstantTime) StartCoroutine(UIObjectMover.MoveObjectInConstantTime(transform, CurrentPosition, closedPosition, moveTime));
        else StartCoroutine(UIObjectMover.MoveObjectWithConstastSpeed(transform, CurrentPosition, closedPosition, moveSpeed));
        panelState = PanelState.Closed;
        if (invoker != null && toggleButtonSprites[1] != null) invoker.GetComponent<Image>().sprite = toggleButtonSprites[1];

        if (SetActionToNothingOnOpen && BuildingController.CurrentAction == Actions.DO_NOTHING) {
            BuildingController.SetCurrentAction(actionBeforeOpeningPanel);
            panelWithNoActionRequirementIsOpen = false;
        }

        if (IsFullFocusPanel) {
            FullFocusPanelIsOpen = (false, null);
        }

        panelClosed?.Invoke();
    }

    public void SetPanelToHiddenPosition() {
        if (panelState == PanelState.Hidden) return;
        StopAllCoroutines();
        StartCoroutine(UIObjectMover.MoveObjectWithConstastSpeed(transform, CurrentPosition, hiddenPosition, 500f));
        panelState = PanelState.Hidden;
    }

    public PanelState GetPanelState() {
        return panelState;
    }

    public bool IsPanelOpen() {
        return panelState == PanelState.Open;
    }

    // public void OnPointerEnter(PointerEventData eventData) {
    //     if (BuildingController.CurrentAction == Actions.DO_NOTHING) return;
    //     ActionBeforeEnteringSettings = BuildingController.CurrentAction;
    //     BuildingController.SetCurrentAction(Actions.DO_NOTHING);
    // }

    // public void OnPointerExit(PointerEventData eventData) {
    //     if (!(BuildingController.CurrentAction == Actions.DO_NOTHING)) return;
    //     BuildingController.SetCurrentAction(ActionBeforeEnteringSettings);
    // }
}
