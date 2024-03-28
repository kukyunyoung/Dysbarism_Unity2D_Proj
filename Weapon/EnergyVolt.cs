using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyVolt : MonoBehaviour
{
    [SerializeField] GameObject idleStaff;
    [SerializeField] GameObject attStaff;
    [SerializeField] Transform staffFirePos;

    GameObject staff;

    public enum StaffState { idle, att }
    public StaffState staffState { get; private set; }

    float angle;
    bool isAtt = false;
    bool isInit = false;

    private void Start() 
    {
        if (idleStaff == null) { Init(); return; }
    }

    private void OnEnable()
    {
        //if(idleStaff == null) {Init(); return;}
        transform.position = staffFirePos.position;
        GetComponent<BoxCollider2D>().enabled = true;
        staffState = (idleStaff.activeSelf) ? StaffState.idle : StaffState.att;
        isAtt = false;

        StartCoroutine(DestroyTime());
    }

    private void OnDisable() 
    {
        if (idleStaff == null) { Init(); return; }
    }

    public void Init()
    {
        staff = GameObject.FindWithTag("Staff");
        print(staff);
        idleStaff = staff.transform.GetChild(0).gameObject;
        attStaff = staff.transform.GetChild(1).gameObject;
        staffFirePos = attStaff.transform.GetChild(0).gameObject.transform;
        isInit = true;
        gameObject.SetActive(false);
    }

    IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    public void Shot(float angle, float bulletSpeed)
    {
        this.angle = angle;
        switch (staffState)
        {
            case StaffState.idle:
                StartCoroutine(ShotVolt(bulletSpeed));
                break;
            case StaffState.att:
                StartCoroutine(ShotVolt(bulletSpeed));
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("TraceRange")) return;

        if (collision.transform.CompareTag("Enemy"))
        {
            AttProcess<Fish>(collision);
        }

        else if(collision.transform.CompareTag("Crab"))
        {
            if (isAtt) return;
            Crab enemy = collision.gameObject.GetComponent<Crab>();

            switch (transform.gameObject.name)
            {
                case "short":
                    enemy.GetDmg(30, 2f);
                    isAtt = true;
                    gameObject.SetActive(false);
                    break;
                case "middle":
                    enemy.GetDmg(60, 5f);
                    isAtt = true;
                    gameObject.SetActive(false);
                    break;
                case "full":
                    enemy.GetDmg(100, 10f);
                    break;
                default:
                    break;
            }
        }

        else if (collision.transform.CompareTag("Hive"))
        {
            if (isAtt) return;
            Hive enemy = collision.gameObject.GetComponent<Hive>();

            switch (transform.gameObject.name)
            {
                case "short":
                    enemy.GetDmg(30);
                    isAtt = true;
                    gameObject.SetActive(false);
                    break;
                case "middle":
                    enemy.GetDmg(60);
                    isAtt = true;
                    gameObject.SetActive(false);
                    break;
                case "full":
                    enemy.GetDmg(100);
                    break;
                default:
                    break;
            }
        }

        else if (collision.transform.CompareTag("Eye"))
        {
            if (isAtt) return;
            Hive_EyeMob enemy = collision.gameObject.GetComponent<Hive_EyeMob>();

            switch (transform.gameObject.name)
            {
                case "short":
                    enemy.GetDmg(30, 2f);
                    isAtt = true;
                    gameObject.SetActive(false);
                    break;
                case "middle":
                    enemy.GetDmg(60, 5f);
                    isAtt = true;
                    gameObject.SetActive(false);
                    break;
                case "full":
                    enemy.GetDmg(100, 10f);
                    break;
                default:
                    break;
            }
        }
    }

    void AttProcess<T>(Collider2D collision) where T : Fish
    {
        if (isAtt) return;
        T enemy = collision.gameObject.GetComponent<T>();
        enemy.isTrace = true;

        switch (transform.gameObject.name)
        {
            case "short":
                enemy.GetDmg(30, 2f);
                isAtt = true;
                gameObject.SetActive(false);
                break;
            case "middle":
                enemy.GetDmg(60, 5f);
                isAtt = true;
                gameObject.SetActive(false);
                break;
            case "full":
                enemy.GetDmg(100, 10f);
                break;
            default:
                break;
        }
    }

    IEnumerator ShotVolt(float bulletSpeed)
    {
        float speed;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        for (int i = 0; i < 100; i++)
        {
            speed = bulletSpeed - i * 0.2f;
            transform.Translate(Vector2.up.normalized * Time.fixedDeltaTime * speed);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
