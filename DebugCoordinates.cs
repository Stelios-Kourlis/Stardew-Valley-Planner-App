using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.SpriteManager;

public class DebugCoordinates : MonoBehaviour {
    TMP_Text posText;
    TMP_Text typeText;
    private static GameObject coordLog;
    public static bool DebugModeinOn { get; private set; }
    void Start() {
        posText = transform.GetChild(0).GetComponent<TMP_Text>();
        typeText = transform.GetChild(1).GetComponent<TMP_Text>();
        coordLog = transform.gameObject;
        DebugModeinOn = false;
        gameObject.SetActive(false);
    }

    public static void ToggleDebugMode() {
        coordLog.SetActive(true);
        Debug.Log("Debug!");
        DebugModeinOn = !DebugModeinOn;
        coordLog.SetActive(DebugModeinOn);
    }

    // Update is called once per frame
    void Update() {
        if (!DebugModeinOn) return;
        Vector3Int pos = BuildingController.CurrentTilemapTransform.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        posText.text = $"{pos.x},{pos.y}";

        TileType? tileType = InvalidTilesManager.Instance.GetTypeOfTile(pos);
        if (tileType != null) typeText.text = tileType.ToString();
        else typeText.text = "N/A";

    }
}
