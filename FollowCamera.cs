using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowCamera : MonoBehaviour
{
    public RectTransform map;
    Transform target;
    public Vector2 mapSize;

    float smoothSpeed = 3;
    float limitMinX, limitMaxX, limitMinY, limitMaxY;
    float cameraHalfWidth, cameraHalfHeight;
    bool isShake;
    float shakeTime;

    private void Start()
    {
        target = PlayerMove.instance.gameObject.GetComponent<Transform>();
        if(SceneManager.GetActiveScene().name == "VillageScene") map = GameObject.FindGameObjectWithTag("MapSize").GetComponent<RectTransform>();
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHalfHeight = Camera.main.orthographicSize;

        if(map != null) SetViewSize();
    }

    public void SetViewSize()
    {
        transform.position = map.transform.position - new Vector3(0,0,10);

        mapSize.x = map.rect.width;
        mapSize.y = map.rect.height;

        limitMinX = (map.position.x - mapSize.x / 2 );
        limitMaxX = (map.position.x + mapSize.x / 2 );
        limitMinY = (map.position.y - mapSize.y / 2 );
        limitMaxY = (map.position.y + mapSize.y / 2 );
    }

    private void LateUpdate()
    {
        if(isShake) return;

        if(shakeTime > 0)
        {
            Vector3 initialPos = transform.position;
            transform.position = Random.insideUnitSphere * 0.03f + initialPos;
            shakeTime -= Time.deltaTime;
        }

        else
        {
            Vector3 desiredPosition = new Vector3(
                Mathf.Clamp(target.position.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth),   // X
                Mathf.Clamp(target.position.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight), // Y
                -10);                                                                                                  // Z
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        }
    }

    public IEnumerator Shake()
    {
        isShake = true;
        Vector3 nowPos = transform.position;
        for(int i=0; i<10; i++)
        {
            transform.position = Random.insideUnitSphere * 0.1f + nowPos;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
        isShake = false;
    }

    public void Hit()
    {
        shakeTime = 0.2f; 
    }

    public void SetTarget(GameObject go = null)
    {
        if(go == null) target = PlayerMove.instance.gameObject.GetComponent<Transform>();
        else target = go.GetComponent<Transform>();
    }
}
