using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroControl : MonoBehaviour
{
    [SerializeField] VideoPlayer video;
    [SerializeField] Animator transition;

    private void Start()
    {
        StartCoroutine(start_());
    }
    IEnumerator start_()
    {
        yield return new WaitForSeconds(1);
        video.Play();
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() =>  !video.isPlaying);

        Transition("Menu");
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
