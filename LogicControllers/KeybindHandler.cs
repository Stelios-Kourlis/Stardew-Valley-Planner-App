using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class KeybindHandler : MonoBehaviour{
    public enum Action{
        Place,
        Edit,
        Delete,
        DeleteAll,
        ToggleUnavailableTiles,
        TogglePlantableTiles,
        Settings,
        Quit,
        Save,
        Load,
        Undo,
        Redo,
    }

    public class Keybind{
        public KeyCode keybind;
        public KeyCode optionalSecondButton;

        public Keybind(KeyCode keybind, KeyCode optionalSecondButton = KeyCode.None){
            this.keybind = keybind;
            this.optionalSecondButton = optionalSecondButton;
        }

        public int ToInt(){
            return ((int)keybind << 16) | (int)optionalSecondButton;
        }

        public override bool Equals(object obj){
            return obj is Keybind keybind &&
                   this.keybind == keybind.keybind &&
                   optionalSecondButton == keybind.optionalSecondButton;
        }

        public override int GetHashCode(){
            return HashCode.Combine(keybind, optionalSecondButton);
        }

        public override string ToString(){
            string text = "";
            if (optionalSecondButton != KeyCode.None) text = optionalSecondButton.ToString() + " - ";
            text += keybind;
            return text;
        }
    }

    private static readonly Dictionary<Action, Keybind> keybinds = new();
    
    public void Start(){
        LoadKeybinds();
        if (GetComponent<Button>() != null) SetUpButton();
        if (GetComponent<Button>() != null) GetComponent<Button>().onClick.AddListener(SetKeybindFromButton);
    }

    private void LoadKeybinds(){
        keybinds.Clear();
        foreach(Action action in Action.GetValues(typeof(Action))){
            int bind = PlayerPrefs.GetInt(action.ToString(), -1);
            if (bind == -1){
                Debug.Log("Setting up first time keybinds");
                SetUpFirstTimeKeybinds();
                return;
            }
            Keybind keybind = new((KeyCode)((bind >> 16) & 0xFFFF), (KeyCode)(bind & 0xFFFF));
            keybinds.Add(action, keybind);
        }    
    }

    public static Keybind GetKeybind(Action action){
        return keybinds[action];
    }

    public bool UpdateKeybind(Action action, Keybind bind){
        foreach (var keybind in keybinds.Values) if (keybind.Equals(bind)) return false;
        keybinds[action] = bind;
        PlayerPrefs.SetInt(action.ToString(), bind.ToInt());
        PlayerPrefs.Save();
        return true;
    }

    private void SetUpFirstTimeKeybinds(){
        UpdateKeybind(Action.Place, new Keybind(KeyCode.P));
        UpdateKeybind(Action.Edit, new Keybind(KeyCode.E));
        UpdateKeybind(Action.Delete, new Keybind(KeyCode.D));
        UpdateKeybind(Action.DeleteAll, new Keybind(KeyCode.D, KeyCode.LeftControl));
        UpdateKeybind(Action.ToggleUnavailableTiles, new Keybind(KeyCode.I));
        UpdateKeybind(Action.TogglePlantableTiles, new Keybind(KeyCode.I, KeyCode.LeftControl));
        UpdateKeybind(Action.Settings, new Keybind(KeyCode.Escape));
        UpdateKeybind(Action.Quit, new Keybind(KeyCode.Q));
        UpdateKeybind(Action.Save, new Keybind(KeyCode.S));
        UpdateKeybind(Action.Load, new Keybind(KeyCode.L));
        UpdateKeybind(Action.Undo, new Keybind(KeyCode.Z, KeyCode.LeftControl));
        UpdateKeybind(Action.Redo, new Keybind(KeyCode.Y, KeyCode.LeftControl));
    }

    private void SetUpButton(){
        Button button = GetComponent<Button>();
        Text buttonText = button.GetComponentInChildren<Text>();
        Keybind actionKeybind = GetKeybind((Action)Enum.Parse(typeof(Action), gameObject.transform.parent.name));
        string text = "";
                if (actionKeybind.optionalSecondButton != KeyCode.None){
                    text = actionKeybind.optionalSecondButton.ToString() + " - ";
                }
                buttonText.text = text + actionKeybind.keybind.ToString();
    }
    public void SetKeybindFromButton(){
        GetInputHandler().IsSearching = true;
        Button button = GetComponent<Button>();
        Text buttonText = button.GetComponentInChildren<Text>();
        StartCoroutine(GetButtonsCoroutine());
    
    }

    IEnumerator GetButtonsCoroutine(){
        Button button = GetComponent<Button>();
        Text buttonText = button.GetComponentInChildren<Text>();
        string text = buttonText.text;
        buttonText.text = "Press any key to rebind";
        while (true){
            buttonText.text = "Press any key to rebind";
            KeyCode optionalSecondButton = KeyCode.None;

            KeyCode[] modifierButtons = {KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftAlt, KeyCode.RightAlt};
            foreach (KeyCode modifierButton in modifierButtons){
                if (Input.GetKey(modifierButton)){
                    buttonText.text = modifierButton.ToString() + " - ";
                    optionalSecondButton = modifierButton;
                    break;
                }
            }        

            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode))){
                if (modifierButtons.Contains(keyCode) || keyCode == KeyCode.Mouse0) continue;
                if (Input.GetKeyDown(keyCode)){
                    string keyPressed = keyCode.ToString();
                    // Debug.Log(keyPressed);
                    Keybind keybind = new(keyCode, optionalSecondButton);
                    if (!UpdateKeybind((Action)Enum.Parse(typeof(Action), gameObject.transform.parent.name), keybind)){
                        GetNotificationManager().SendNotification("Keybind already in use", NotificationManager.Icons.ErrorIcon);
                        buttonText.text = text;
                        yield break;
                    }
                    text = "";
                    if (keybind.optionalSecondButton != KeyCode.None) text = keybind.optionalSecondButton.ToString() + " - ";
                    buttonText.text = text + keyPressed;
                    while (Input.GetKey(keyCode)) yield return null;
                    yield return null;
                    GetInputHandler().IsSearching = false;
                    yield break;
                }
            }
            yield return null;
        }
       
    }

    public void ToggleKeybindMenu(){
        GameObject keybindMenu = GameObject.FindGameObjectWithTag("KeybindMenu");
        if (keybindMenu.GetComponent<RectTransform>().localPosition.x == 0){
            HideKeybindMenu();
        }else{
            ShowKeybindMenu();
        }
    }
    
    public void ShowKeybindMenu(){
        GameObject keybindMenu = GameObject.FindGameObjectWithTag("KeybindMenu");
        keybindMenu.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }

    public void HideKeybindMenu(){
        GameObject keybindMenu = GameObject.FindGameObjectWithTag("KeybindMenu");
        keybindMenu.GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
    }
}
