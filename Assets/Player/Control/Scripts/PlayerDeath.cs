using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Animator fadeAnimator;

    [SerializeField] private TextMeshProUGUI totals;
    private string totalsFormat = "Fuel collected: {0}\nEnemies killed: {1}\nRooms explored: {2}";

    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource source;

    private bool isDead = false;

    public void Init()
    {
        Scripts.Player.playerWarmth.onPlayerDiedEvent += Die;
    }

    private void Die()
    {
        if (isDead) return;

        fadeAnimator.SetTrigger("fadeIn");

        totals.text = string.Format(totalsFormat, Fuel.totalFuelCollected, EnemyController.totalKilled, Room._id);
        isDead = true;

        foreach (var clip in clips)
            source.PlayOneShot(clip);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void PlayAgain()
    {
        SceneManager.LoadScene("Main");
    }
}