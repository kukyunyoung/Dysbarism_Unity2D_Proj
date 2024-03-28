using UnityEngine;

public class RewardBox : MonoBehaviour
{
    [SerializeField] GameObject popUp;
    [SerializeField] GameObject[] fieldItem;
    [SerializeField] GameObject[] portionItem;
    [SerializeField] GameObject[] healItem;
    
    Animator anim;
    bool isOpen;

    void Start()
    {
        isOpen = false;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) popUp.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player") && Input.GetKey(KeyCode.E)) 
        {
            OpenBox();
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) popUp.SetActive(false);
    }

    void OpenBox()
    {
        if(isOpen) return;
        isOpen = true;
        anim.Play("BoxOpen");

        int random = Random.Range(0, fieldItem.Length);
        Instantiate(fieldItem[random], gameObject.transform);
        random = Random.Range(0, portionItem.Length);
        Instantiate(portionItem[random], gameObject.transform);
        for (int i=0; i<healItem.Length; i++) Instantiate(healItem[i], gameObject.transform);

        Destroy(this, 3);
    }
}
