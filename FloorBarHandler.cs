using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;

public class FloorBarHandler : MonoBehaviour {
    private readonly float moveScale = 5f;
    private bool floorBarIsOpen = false;
    private bool isHidden;
    private Sprite[] arrowButtons = new Sprite[2];

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = new Vector3(Screen.width + 300, 5, 0);
        arrowButtons[0] = Sprite.Create(Resources.Load("UI/FloorBarUnhide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        arrowButtons[1] = Sprite.Create(Resources.Load("UI/FloorBarHide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
    }

    void Update(){
        if (GetBuildingController().GetCurrentBuildingType() == typeof(Floor) && !floorBarIsOpen){
            floorBarIsOpen = true;
            StartCoroutine(OpenBar());
        }
        else if (!(GetBuildingController().GetCurrentBuildingType() == typeof(Floor)) && floorBarIsOpen) {
            floorBarIsOpen = false;
            StartCoroutine(CloseBar());
        }
    }


    /// <summary>
    /// if the bar is visible (floor is selected), toggle it collapsing to the side and back
    /// </summary>
    public void ToggleHideBar(){
        if (!floorBarIsOpen) return;
        if (isHidden) StartCoroutine(OpenBar());
        else StartCoroutine(HideBar());
        isHidden = !isHidden;
    }

    IEnumerator OpenBar() {
        while (gameObject.transform.position.x > Screen.width - 10) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
        transform.GetChild(1).GetComponent<Image>().sprite = arrowButtons[1];
    }

    IEnumerator CloseBar() {
        while (gameObject.transform.position.x < Screen.width + 300) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
    }

    IEnumerator HideBar() {
        while (gameObject.GetComponent<RectTransform>().anchoredPosition.x < Screen.width/2 + GetComponent<RectTransform>().sizeDelta.x) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
        transform.GetChild(1).GetComponent<Image>().sprite = arrowButtons[0];
    }
}
