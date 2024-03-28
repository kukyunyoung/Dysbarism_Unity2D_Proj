using UnityEngine;
using UnityEngine.UI;

public class WeaponChangeBtn : MonoBehaviour
{
    [SerializeField] Button slot1;
    [SerializeField] Button slot2;
    [SerializeField] GameObject[] weapon;
    [SerializeField] AttBtn attBtn;

    void Start()
    {
        slot1.onClick.AddListener(() => ChangeWeapon(1));
        slot2.onClick.AddListener(() => ChangeWeapon(2));
    }

    // ���Ŀ� �߰��� ���
    // ������ 2���� ���⸸ ������ �����ϱ⶧���� 0���� 1���� �迭�� �����ϴµ�
    // ���߿� ������Ʈ�� �̸��� �迭�� ��ȣ�� ���ؼ� ���ӿ�����Ʈ�� �ٲ��ֵ�����
    void ChangeWeapon(int slotNum)
    {
        if(slotNum == 1)
        {
            weapon[1].gameObject.SetActive(false);
            weapon[2].gameObject.SetActive(true);
            attBtn.SetWeapon(weapon[2]);
        }
        if (slotNum == 2)
        {
            weapon[2].gameObject.SetActive(false);
            weapon[1].gameObject.SetActive(true);
            attBtn.SetWeapon(weapon[1]);
        }
    }
}
