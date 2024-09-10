using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static KeybindHandler;
// using static Utility.ClassManager;
using Action = KeybindHandler.Action;

public class KeybindButton : MonoBehaviour {

    GameObject CancelRebindGameObject => transform.GetChild(0).GetChild(0).gameObject;
    GameObject KeybindButtonGameObject => transform.GetChild(0).GetChild(1).gameObject;
    GameObject ResetKeybindButton => transform.GetChild(0).GetChild(2).gameObject;

    private static (bool, KeybindButton) isButtonBeingRebound = (false, null);

    KeyValuePair<Action, Keybind> actionKeybindPair;
    private bool cancelRebind = false;

    public void Start() {
        SetUpButton();
        KeybindHandler.Instance.AddButtonToList(this);
    }

    private void SetUpButton() {
        // Debug.Log("Setting up button: " + gameObject.name);

        Action action = (Action)Enum.Parse(typeof(Action), KeybindButtonGameObject.transform.parent.parent.name);
        Keybind actionKeybind = GetKeybind(action);
        actionKeybindPair = new(action, actionKeybind);

        //Keybind Text
        TMP_Text buttonText = KeybindButtonGameObject.GetComponentInChildren<TMP_Text>();
        string text = actionKeybind.optionalSecondButton != KeyCode.None ? actionKeybind.optionalSecondButton.ToString() + " - " : "";
        buttonText.text = text + actionKeybind.keybind.ToString();
        KeybindButtonGameObject.GetComponent<Button>().onClick.AddListener(SetKeybindFromButton);

        //Reset Button
        ResetKeybindButton.GetComponent<Button>().onClick.AddListener(ResetKeybind);

        //Clear Rebind Button
        CancelRebindGameObject.GetComponent<Button>().onClick.AddListener(CancelRebind);
        CancelRebindGameObject.SetActive(false);
    }

    public void SetKeybindFromButton() {
        if (isButtonBeingRebound.Item1) isButtonBeingRebound.Item2.CancelRebind();

        InputHandler.Instance.IsSearching = true;
        isButtonBeingRebound = (true, this);
        StartCoroutine(GetButtonsCoroutine());
    }

    public void CancelRebind() {
        cancelRebind = true;
        isButtonBeingRebound = (false, null);
    }

    public void ResetKeybind() {
        TMP_Text buttonText = KeybindButtonGameObject.GetComponentInChildren<TMP_Text>();
        Keybind defaultKeybind = GetDefaultKeybind(actionKeybindPair.Key);
        if (!KeybindHandler.Instance.UpdateKeybind(actionKeybindPair.Key, defaultKeybind)) {
            NotificationManager.Instance.SendNotification($"Default Keybind ({defaultKeybind}) already in use", NotificationManager.Icons.ErrorIcon);
            return;
        }
        string text = defaultKeybind.optionalSecondButton != KeyCode.None ? defaultKeybind.optionalSecondButton.ToString() + " - " : "";
        buttonText.text = text + defaultKeybind.keybind.ToString();
    }

    IEnumerator GetButtonsCoroutine() {
        Button button = KeybindButtonGameObject.GetComponent<Button>();
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        string text = buttonText.text;
        buttonText.text = "Press any key to rebind";
        CancelRebindGameObject.SetActive(true);
        while (true) {
            buttonText.text = "Press any key to rebind";
            KeyCode optionalSecondButton = KeyCode.None;

            if (cancelRebind) {
                buttonText.text = text;
                cancelRebind = false;
                InputHandler.Instance.IsSearching = false;
                CancelRebindGameObject.SetActive(false);
                yield break;
            }

            //Get modifier button
            KeyCode[] modifierButtons = { KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftAlt, KeyCode.RightAlt };
            foreach (KeyCode modifierButton in modifierButtons) {
                if (Input.GetKey(modifierButton)) {
                    buttonText.text = modifierButton.ToString() + " - ";
                    optionalSecondButton = modifierButton;
                    break;
                }
            }

            //Get key
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) {
                if (modifierButtons.Contains(keyCode) || keyCode == KeyCode.Mouse0) continue;
                if (Input.GetKeyDown(keyCode)) {
                    string keyPressed = keyCode.ToString();
                    Keybind keybind = new(keyCode, optionalSecondButton);
                    if (!KeybindHandler.Instance.UpdateKeybind(actionKeybindPair.Key, keybind)) {
                        NotificationManager.Instance.SendNotification("Keybind already in use", NotificationManager.Icons.ErrorIcon);
                        buttonText.text = text;
                        isButtonBeingRebound = (false, null);
                        yield break;
                    }
                    text = "";
                    if (keybind.optionalSecondButton != KeyCode.None) text = keybind.optionalSecondButton.ToString() + " - ";
                    buttonText.text = text + keyPressed;
                    while (Input.GetKey(keyCode)) yield return null;
                    yield return null;
                    CancelRebindGameObject.SetActive(false);
                    InputHandler.Instance.IsSearching = false;
                    isButtonBeingRebound = (false, null);
                    yield break;
                }
            }
            yield return null;
        }
    }
}
