using NaughtyAttributes;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [ReadOnly] public EnemyMotor enemyMotor;

    public void Init()
    {
        enemyMotor = GetComponent<EnemyMotor>();
        enemyMotor.Init();
    }

    private void FixedUpdate()
    {
        enemyMotor.UpdateMotor();
    }
}
