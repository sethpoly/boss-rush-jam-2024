using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    public void ScreenShake(float duration = .1f, float magnitude = .3f) 
    {
        var shaker = mainCamera.GetComponent<CameraShake>();
        StartCoroutine(shaker.Shake(duration: duration, magnitude: magnitude));
    }

}
