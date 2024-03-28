using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    IEnumerator Move();
    void Trace(); // onTrigger ���
    void Attack(Collision2D collision); // onCollision ���
    void GetDmg(int dmg, float knockBack = 0);
    IEnumerator Die();
}
