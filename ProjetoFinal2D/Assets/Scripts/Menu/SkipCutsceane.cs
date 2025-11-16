using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class SkipCutsceane : MonoBehaviour
{
    [SerializeField] Animator transition;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Transition("Game");
        }
    }

    public void Transition(string scene)
    {
        StartCoroutine(TransitionCoroutine(scene));
    }
    IEnumerator TransitionCoroutine(string scene)
    {
        transition.SetBool("go", true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
        yield return null;
    }
}
