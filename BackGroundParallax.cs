using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackGroundParallax : MonoBehaviour
{
    [SerializeField] FollowCamera cam;
    [SerializeField] float speedX;
    [SerializeField] float speedY;

    private void LateUpdate() 
    {
        transform.position = new Vector3(
            cam.transform.position.x * speedX,
            cam.transform.position.y * speedY,
            2
        );
    }
}
