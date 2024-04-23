using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationWidow : MonoBehaviour, IToggleablePanel {
    private Button button;
    private GameObject confirmWindow;
    public bool IsOpen {get; private set;}
    public bool IsMoving {get; private set;}
    private readonly float sizeScale = 0.01f;
    // Start is called before the first frame update
    void Start() {
        button = gameObject.GetComponent<Button>();
        confirmWindow = GameObject.FindGameObjectWithTag("ConfirmDeleteAll");
        confirmWindow.transform.position = new Vector3(-Screen.width / 2, -Screen.height / 2, 0);
        confirmWindow.transform.localScale = new Vector3(0, 0, 0);
    }

    public void OpenConfirmDialog() {
        if (!IsOpen) StartCoroutine(OpenPanel());
    }

    public void CloseConfirmDialog() {
        if (IsOpen) StartCoroutine(ClosePanel());
    }

    public void TogglePanel(){
        if (IsMoving) return;
        if (IsOpen) StartCoroutine(ClosePanel());
        else StartCoroutine(OpenPanel());
    }

    public void Accepted() {
        CloseConfirmDialog();
    }

    public IEnumerator OpenPanel() {
        confirmWindow.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        IsMoving = true;
        for (int i = 0; i < 100; i++) {
            confirmWindow.transform.localScale = new Vector3(confirmWindow.transform.localScale.x + sizeScale, confirmWindow.transform.localScale.y + sizeScale, 0);
            yield return null;
        }
        IsMoving = false;
        IsOpen = true;
    }

    public IEnumerator ClosePanel() {
        confirmWindow.transform.position = new Vector3(-Screen.width / 2, -Screen.height / 2, 0);
        IsMoving = true;
        for (int i = 0; i < 100; i++) {
            confirmWindow.transform.localScale = new Vector3(confirmWindow.transform.localScale.x - sizeScale, confirmWindow.transform.localScale.y - sizeScale, 0);
            yield return new WaitForSecondsRealtime(1f * Time.deltaTime);
        }
        IsMoving = false;
        IsOpen = false;
    }
}
