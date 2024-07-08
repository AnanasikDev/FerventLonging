using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float maxAttackDistance;
    [SerializeField] private int hitWarmthLoss;

    public void Init() { }

    public void UpdateAttack()
    {
        if (!Scripts.Player.playerInput.isCommitWarmthPressed) return;

        EnemyController enemy = EnemyController.enemies.Count == 0 ? null : EnemyController.enemies.Where(e => e && (e.transform.position - transform.position).magnitude < maxAttackDistance).FirstOrDefault();

        if (enemy == null) return;

        enemy.Die();
        Scripts.Player.playerWarmth.decreaseWarmth(hitWarmthLoss);
    }
}