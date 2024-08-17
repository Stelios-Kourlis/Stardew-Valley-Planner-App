using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoldingMenuItem : MonoBehaviour {

    public bool isAnchorButton = false;

    void Start() {
        transform.parent.GetComponent<FoldingMenuGroup>().AddMenuItem(this);
        // gameObject.AddComponent<Button>();
        // gameObject.AddComponent<RectTransform>();
        // gameObject.AddComponent<Image>();
    }

}
