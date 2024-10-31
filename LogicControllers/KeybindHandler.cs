using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class KeybindHandler : MonoBehaviour {
    public enum Action {
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
        OpenBuildingMenu,
        OpenTotalCost,
        ToggleUI,
        PickBuilding
    }

    public class Keybind {
        public KeyCode keybind;
        public KeyCode optionalSecondButton;

        public Keybind(KeyCode keybind, KeyCode optionalSecondButton = KeyCode.None) {
            this.keybind = keybind;
            this.optionalSecondButton = optionalSecondButton;
        }

        public int ToInt() {
            return ((int)keybind << 16) | (int)optionalSecondButton;
        }

        public override bool Equals(object obj) {
            return obj is Keybind keybind &&
                   this.keybind == keybind.keybind &&
                   optionalSecondButton == keybind.optionalSecondButton;
        }

        public override int GetHashCode() {
            return HashCode.Combine(keybind, optionalSecondButton);
        }

        public override string ToString() {
            string text = "";
            if (optionalSecondButton != KeyCode.None) text = optionalSecondButton.ToString() + " - ";
            text += keybind;
            return text;
        }
    }

    public static KeybindHandler Instance { get; private set; }

    private static Dictionary<Action, Keybind> keybinds = null;
    private static readonly List<KeybindButton> keybindButtons = new();

    public void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        LoadKeybinds();
    }

    public void AddButtonToList(KeybindButton button) {
        keybindButtons.Add(button);
    }

    private static void LoadKeybinds() {
        keybinds = new();
        foreach (Action action in Enum.GetValues(typeof(Action))) {
            int bind = PlayerPrefs.GetInt(action.ToString(), GetDefaultKeybind(action).ToInt());
            Keybind keybind = new((KeyCode)((bind >> 16) & 0xFFFF), (KeyCode)(bind & 0xFFFF));
            keybinds.Add(action, keybind);
        }
    }

    public static Keybind GetKeybind(Action action) {
        if (keybinds == null) LoadKeybinds();
        return keybinds[action] ?? GetDefaultKeybind(action);
    }

    public bool UpdateKeybind(Action action, Keybind bind) {
        foreach (var kvp in keybinds) if (kvp.Value.Equals(bind) && action != kvp.Key) return false;
        keybinds[action] = bind;
        PlayerPrefs.SetInt(action.ToString(), bind.ToInt());
        PlayerPrefs.Save();
        return true;
    }

    public static void ResetKeybinds() {
        keybinds = new();//first clear old ones
        foreach (var button in keybindButtons) button.ResetKeybind();
    }

    public static Keybind GetDefaultKeybind(Action action) {
        return action switch {
            Action.Place => new Keybind(KeyCode.P),
            Action.Edit => new Keybind(KeyCode.E),
            Action.Delete => new Keybind(KeyCode.D),
            Action.DeleteAll => new Keybind(KeyCode.D, KeyCode.LeftControl),
            Action.ToggleUnavailableTiles => new Keybind(KeyCode.I),
            Action.TogglePlantableTiles => new Keybind(KeyCode.I, KeyCode.LeftControl),
            Action.Settings => new Keybind(KeyCode.Escape),
            Action.Quit => new Keybind(KeyCode.Q),
            Action.Save => new Keybind(KeyCode.S),
            Action.Load => new Keybind(KeyCode.L),
            Action.Undo => new Keybind(KeyCode.Z, KeyCode.LeftControl),
            Action.Redo => new Keybind(KeyCode.Y, KeyCode.LeftControl),
            Action.OpenBuildingMenu => new Keybind(KeyCode.B),
            Action.OpenTotalCost => new Keybind(KeyCode.T),
            Action.ToggleUI => new Keybind(KeyCode.U, KeyCode.LeftControl),
            Action.PickBuilding => new Keybind(KeyCode.C),

            _ => throw new Exception("Action not found"),
        };
    }


}
