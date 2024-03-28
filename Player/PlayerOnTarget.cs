using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnTarget : MonoBehaviour
{
    [SerializeField] GameObject pointer;

    public float GetRotation(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Vector3 attDir = collision.transform.position - gameObject.GetComponentInParent<Transform>().position;
            float angle = Mathf.Atan2(collision.transform.position.y - transform.position.y, collision.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            pointer.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            return angle;
        }
        return 0;
    }

    public float GetRotation(Vector3 target)
    {
            Vector3 attDir = target - gameObject.GetComponentInParent<Transform>().position;
            float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
            pointer.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            return angle;
    }
}
