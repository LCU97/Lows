using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Status
{
    public int hp;
    public int hpMax;
    public float criRate;
    public float criAttack;
    public float attack;
    public float defence;

    public Status(int hp, float criRate, float criAttack, float attack, float defence)
    {
        this.hp = this.hpMax = hp;
        this.criAttack = criAttack;
        this.criRate = criRate;
        this.attack = attack;
        this.defence = defence;
    }
}