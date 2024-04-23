using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuController : MonoBehaviour, IToggleablePanel {
    private Button thisButton;
    private GameObject panel;
    public bool IsMoving {get; private set;}
    public bool IsOpen {get; private set;}
    private readonly Sprite[] arrowButtons = new Sprite[2];
    void Start() {
        panel = gameObject;
        thisButton = panel.transform.Find("ArrowButton").GetComponent<Button>();
        arrowButtons[0] = Sprite.Create(Resources.Load("UI/ExtendBuildingMenu") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        arrowButtons[1] = Sprite.Create(Resources.Load("UI/HideBuildingMenu") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        thisButton.GetComponent<Image>().sprite = arrowButtons[0];
    }

    public void TogglePanel() {
        if (IsMoving) return;
        if (IsOpen) StartCoroutine(ClosePanel());
        else StartCoroutine(OpenPanel());
    }

    public IEnumerator OpenPanel(){
        IsMoving = true;
        thisButton.GetComponent<Image>().sprite = arrowButtons[1];
        while (panel.transform.position.x < 0){
            panel.transform.position += new Vector3(IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0, 0);
            yield return null;
        }
        panel.transform.position = new Vector3(0, panel.transform.position.y, 0);
        IsMoving = false;
        IsOpen = true;
    }

    public IEnumerator ClosePanel(){
        IsMoving = true;
        thisButton.GetComponent<Image>().sprite = arrowButtons[0];
        while (panel.transform.position.x > 0 - panel.GetComponent<RectTransform>().rect.width){
            panel.transform.position -= new Vector3(IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0, 0);
            yield return null;
        }
        panel.transform.position = new Vector3(0 - panel.GetComponent<RectTransform>().rect.width, panel.transform.position.y, 0);
        IsMoving = false;
        IsOpen = false;
    }
}
