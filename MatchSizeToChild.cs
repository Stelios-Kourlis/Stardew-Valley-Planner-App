using UnityEngine;

public class MatchSizeToChild : MonoBehaviour{
    public int paddinX = 30;
    public int paddinY = 10;
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform childRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

        rectTransform.sizeDelta = childRectTransform.sizeDelta + new Vector2(paddinX, paddinY);
    }
}