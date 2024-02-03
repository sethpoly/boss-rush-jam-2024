using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource cardSelect;
    [SerializeField] private AudioSource deselectCard;
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource hit;
    [SerializeField] private AudioSource shoot;


    public void PlayCardSelect()
    {
        cardSelect.Play();
    }

    public void PlayDeselectCard()
    {
        deselectCard.Play();
    }

    public void PlayButtonClick()
    {
        buttonClick.Play();
    }
    public void PlayHit()
    {
        hit.Play();
    }

    public void PlayShoot()
    {
        shoot.Play();
    }
}
