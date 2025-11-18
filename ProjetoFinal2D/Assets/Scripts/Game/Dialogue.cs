using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] float writeSpeed;
    private bool onCoroutine;
    [HideInInspector] public bool onDialogue;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] AudioSource fala;
    [SerializeField] Image personagem;

    Coroutine writingCoroutine;
    bool isTyping;

    private void Start()
    {
        dialogueText.text = null;
    }
    public void StartDialogue(string[] dialogue, AudioClip[] falas, string name, bool mudo)
    {
        personagem.gameObject.SetActive(!mudo);
        StartCoroutine(Dialogue_(dialogue, falas, name, mudo));
        dialogueBox.SetActive(true);
    }

    IEnumerator Dialogue_(string[] dialogue, AudioClip[]falas, string name, bool mudo) 
    {
        nameText.text = name;
        foreach (string text in dialogue)
        {
            if (!mudo)
            {
                fala.clip = falas[Random.Range(0, falas.Length)];
                fala.Play();
            }
            yield return null;

            dialogueText.text = null;
            isTyping = true;

            writingCoroutine = StartCoroutine(WriteText(text));

            yield return new WaitUntil(() => !isTyping || Input.GetKeyDown(KeyCode.Space));

            if (isTyping)
            {
                StopCoroutine(writingCoroutine);

                dialogueText.text = text;
            }

            isTyping = false;
            yield return null;

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        }

        dialogueText.text = null;
        GameManager.instance.onDialogue = false;
        GameManager.instance.player.ResetParrot();

        dialogueBox.SetActive(false);
    }

    IEnumerator WriteText(string text)
    {
        foreach (char letter in text) 
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(writeSpeed);
        }
        isTyping = false;
        yield return null;
    }
}
