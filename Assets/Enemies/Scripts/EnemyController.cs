using NaughtyAttributes;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [ReadOnly] public EnemyMotor enemyMotor;
    [ReadOnly] public EnemyAttack enemyAttack;

    public void Init()
    {
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
}
