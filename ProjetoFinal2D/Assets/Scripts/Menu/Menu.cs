using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] Animator transition_;
    public void NextSceane(string scene) 
    {
        StartCoroutine(Game(transition_, scene));
    }
    IEnumerator Game(Animator transition, string scene)
    {
        transition.SetBool("go", true);
        yield return new WaitForSeconds(1);
        transition_.SetBool("go", false);
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
