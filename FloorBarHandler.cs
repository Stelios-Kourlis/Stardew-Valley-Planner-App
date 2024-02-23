using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.ClassManager;

public class FloorBarHandler : MonoBehaviour {
    private float moveScale = 5f;
    private bool floorBarIsOpen = false;

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = new Vector3(Screen.width + 300, 5, 0);
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
}
