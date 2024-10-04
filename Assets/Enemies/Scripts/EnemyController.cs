using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, PrefabID
{
    public static List<EnemyController> enemies = new List<EnemyController>();

    [ReadOnly] public EnemyMotor enemyMotor;
    [ReadOnly] public EnemyAttack enemyAttack;

    public static int totalKilled = 0;

    public int id { get; set; }

    public void Init()
    {
        enemies.Add(this);

        enemyMotor = GetComponent<EnemyMotor>();
        enemyAttack = GetComponent<EnemyAttack>();

        enemyMotor.Init();
        enemyAttack.Init();
    }

    private void FixedUpdate()
    {
        enemyMotor.UpdateMotor();
        enemyAttack.UpdateAttack();
    }

    private void OnDestroy()
    {
        enemies.Remove(this);
    }

    public void Die()
    {
        totalKilled++;
        Destroy(gameObject);
    }
}
