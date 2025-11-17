using UnityEngine;
using UnityEngine.UIElements;

public class AudioControll : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource[] audioSource;
    public void ChangeSound()
    {
        foreach (AudioSource source in audioSource)
        {
            source.volume = slider.value;
        }
    }
}
