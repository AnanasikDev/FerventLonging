using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float maxAttackDistance = 3;
    [SerializeField] private float attackDelay = 1.25f;
    [SerializeField] private float warmthDamage = 5f;

    [SerializeField] private ParticleSystem attackParticles;

    private float lastTime;

    public void Init()
    {
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
        Scripts.Player.playerWarmth.decreaseWarmth(warmthDamage);

        Vector2 direction = Scripts.Player.transform.position - transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        attackParticles.transform.rotation = targetRotation;

        attackParticles.Play();
    }
}
