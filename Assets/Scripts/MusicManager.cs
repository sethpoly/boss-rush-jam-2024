using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource cardSelect;
    [SerializeField] private AudioSource deselectCard;
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource hit;
    [SerializeField] private AudioSource shoot;
    [SerializeField] private AudioSource bossShoot;
    [SerializeField] private AudioSource explosion;
    [SerializeField] private AudioSource battlePhaseEnd;
    [SerializeField] private AudioSource menuTheme;

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

    public void PlayBossShoot()
    {
        bossShoot.Play();
    }

    public void PlayExplosion()
    {
        explosion.Play();
    }

    public void PlayBattlePhaseEnd()
    {
        battlePhaseEnd.Play();
    }

    public void PlayMenuTheme()
    {
        menuTheme.Play();
    }

    public void StopMenuTheme()
    {
        menuTheme.Stop();
    }
}
