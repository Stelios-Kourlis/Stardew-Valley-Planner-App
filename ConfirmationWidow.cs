using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationWidow : MonoBehaviour {
    private Button button;
    private GameObject confirmWindow;
    private float sizeScale = 0.01f;
    bool inConfrimDialog = false;
    // Start is called before the first frame update
    void Start() {
        button = gameObject.GetComponent<Button>();
        confirmWindow = GameObject.FindGameObjectWithTag("ConfirmDeleteAll");
        confirmWindow.transform.position = new Vector3(-Screen.width / 2, -Screen.height / 2, 0);
        confirmWindow.transform.localScale = new Vector3(0, 0, 0);
    }

    public void OpenConfirmDialog() {
        if (!inConfrimDialog) { inConfrimDialog = true; StartCoroutine(Open()); }
    }

    public void CloseConfirmDialog() {
        if (inConfrimDialog) { inConfrimDialog = false; StartCoroutine(Close()); }
    }

    public void Accepted() {
        //GameObject.FindGameObjectWithTag("LogicComponent").GetComponent<PlaceBuilding>().deleteAll();
        CloseConfirmDialog();
    }

    IEnumerator Open() {
        confirmWindow.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        for (int i = 0; i < 100; i++) {
            confirmWindow.transform.localScale = new Vector3(confirmWindow.transform.localScale.x + sizeScale, confirmWindow.transform.localScale.y + sizeScale, 0);
            yield return null;
        }
    }

    IEnumerator Close() {
        confirmWindow.transform.position = new Vector3(-Screen.width / 2, -Screen.height / 2, 0);
        for (int i = 0; i < 100; i++) {
            confirmWindow.transform.localScale = new Vector3(confirmWindow.transform.localScale.x - sizeScale, confirmWindow.transform.localScale.y - sizeScale, 0);
            yield return new WaitForSecondsRealtime(1f * Time.deltaTime);
        }
    }
}
