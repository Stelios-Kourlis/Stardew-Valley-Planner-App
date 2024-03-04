using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.ClassManager;

public class FloorBarHandler : MonoBehaviour {
    private float moveScale = 5f;
    private bool floorBarIsOpen = false;
    private bool isHidden;

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = new Vector3(Screen.width + 300, 5, 0);
    }

    void Update(){
        if (GetBuildingController().GetCurrentBuildingType() == typeof(Floor) && !floorBarIsOpen){
            floorBarIsOpen = true;
            StartCoroutine(OpenBar());
            ///transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (!(GetBuildingController().GetCurrentBuildingType() == typeof(Floor)) && floorBarIsOpen) {
            floorBarIsOpen = false;
            StartCoroutine(CloseBar());
            //transform.GetChild(1).gameObject.SetActive(false);
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
    }

    IEnumerator CloseBar() {
        while (gameObject.transform.position.x < Screen.width + 300) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
    }

    IEnumerator HideBar() {
        Debug.Log($"{gameObject.GetComponent<RectTransform>().anchoredPosition.x}/{Screen.width/2 + GetComponent<RectTransform>().sizeDelta.x}, width is {Screen.width}");
        while (gameObject.GetComponent<RectTransform>().anchoredPosition.x < Screen.width/2 + GetComponent<RectTransform>().sizeDelta.x) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveScale, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return null;
        }
    }
}
