using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private new GameObject light;
    [SerializeField] private new Camera camera;

    [SerializeField] private Animator animator;

    private void Update()
    {
        light.transform.position = Vector3.Lerp(light.transform.position, camera.ScreenToWorldPoint(Input.mousePosition), Time.deltaTime);
        light.transform.position = light.transform.position.WithZ(9);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        IEnumerator proceed()
        {
            animator.SetTrigger("fadeAway");
            yield return new WaitForSecondsRealtime(2f);
            SceneManager.LoadScene("Main");
        }

        StartCoroutine(proceed());
    }
}
