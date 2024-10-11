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
    static GameObject coordLog;
    static bool debugModeinOn;
    void Start() {
        posText = transform.GetChild(0).GetComponent<TMP_Text>();
        typeText = transform.GetChild(1).GetComponent<TMP_Text>();
        coordLog = transform.gameObject;
        debugModeinOn = false;
        gameObject.SetActive(false);
    }

    public static void ToggleDebugMode() {
        coordLog.SetActive(true);
        Debug.Log("Debug!");
        debugModeinOn = !debugModeinOn;
        coordLog.SetActive(debugModeinOn);
    }

    // Update is called once per frame
    void Update() {
        if (!debugModeinOn) return;
        Vector3Int pos = BuildingController.CurrentTilemapTransform.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        posText.text = $"{pos.x},{pos.y}";

        TileType? tileType = InvalidTilesManager.Instance.GetTypeOfTile(pos);
        if (tileType != null) typeText.text = tileType.ToString();
        else typeText.text = "N/A";

    }
}
