using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyController enemyController;

    [SerializeField] private float maxAttackDistance = 3;
    [SerializeField] private float attackDelay = 1.25f;
    [SerializeField] private float warmthDamage = 5f;

    [SerializeField] private ParticleSystem attackParticles;

    private float lastTime;

    public void Init()
    {
        enemyController = GetComponent<EnemyController>();
        lastTime = Time.time;
    }

    public void UpdateAttack()
    {
        if (isAttackable())
        {
            Attack();
            lastTime = Time.time;
        }
    }

    private bool isAttackable()
    {
        return (transform.position - Scripts.Player.transform.position).magnitude <= maxAttackDistance &&
            Time.time - lastTime > attackDelay;
    }

    public void Attack()
    {
        Vector2 direction = Scripts.Player.transform.position - transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        if (Mathf.Abs(Mathf.Repeat(targetAngle, 360) - enemyController.enemyMotor.agent.transform.eulerAngles.z) < 70 ||
            (transform.position - Scripts.Player.transform.position).magnitude < 0.8f)
        {
            Scripts.Player.playerWarmth.decreaseWarmth(warmthDamage);
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        attackParticles.transform.rotation = targetRotation;

        attackParticles.Play();
    }
}
