using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Slider sl;
    [SerializeField] private Sprite[] iconsSprite;
    [SerializeField] private AudioSource[] audioSource;


    public void ChangeSound()
    {
        foreach (AudioSource source in audioSource)
        {
            source.volume = sl.value;

            if (sl.value == 0) iconImage.sprite = iconsSprite[1];
            else iconImage.sprite = iconsSprite[0];
        }
    }
}
