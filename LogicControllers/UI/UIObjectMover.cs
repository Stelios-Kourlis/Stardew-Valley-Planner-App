using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIObjectMover {

    public static IEnumerator MoveObjectInConstantTime(Transform obj, Vector3 startPosition, Vector3 endPosition, float totalTime) {
        float time = 0;
        while (time < totalTime) {
            float t = time / totalTime; // Normalize time to range [0, 1]
            obj.GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        obj.GetComponent<RectTransform>().localPosition = endPosition; // Ensure the button reaches the exact end position
    }

    public static IEnumerator MoveObjectWithConstastSpeed(Transform obj, Vector3 startPosition, Vector3 endPosition, float speed) {
        float distance = Vector3.Distance(startPosition, endPosition); //implementation with fixed speed, I like fixed time better
        float remainingDistance = distance;
        while (remainingDistance > 0.01f) {
            obj.GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPosition, endPosition, 1 - (remainingDistance / distance));
            remainingDistance -= speed * Time.deltaTime;
            yield return null;
        }

        obj.GetComponent<RectTransform>().localPosition = endPosition; // Ensure the button reaches the exact end position
    }

    public static IEnumerator ScaleObjectInConstantTime(Transform obj, Vector3 startScale, Vector3 endScale, float totalTime) {
        float time = 0;
        while (time < totalTime) {
            float t = time / totalTime; // Normalize time to range [0, 1]
            obj.GetComponent<RectTransform>().localScale = Vector3.Lerp(startScale, endScale, t);
            time += Time.deltaTime;
            yield return null;
        }
        obj.GetComponent<RectTransform>().localScale = endScale; // Ensure the button reaches the exact end scale
    }
}
