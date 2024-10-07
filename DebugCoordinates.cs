using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.SpriteManager;

public class DebugCoordinates : MonoBehaviour {
    TMP_Text text;
    static GameObject coordLog;
    static bool debugModeinOn;
    void Start() {
        text = transform.GetChild(0).GetComponent<TMP_Text>();
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
        Vector3Int pos = GameObject.FindWithTag("InvalidTilemap").GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        text.text = $"{pos.x},{pos.y}";
        // GameObject.FindWithTag("InvalidTilemap").GetComponent<Tilemap>().SetTile(pos, LoadTile("RedTile"));
    }
}
