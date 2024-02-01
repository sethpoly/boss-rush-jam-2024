using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    public void ScreenShake() 
    {
        var shaker = mainCamera.GetComponent<CameraShake>();
        StartCoroutine(shaker.Shake(duration: .1f, magnitude: .3f));
    }

}
