using System.Collections;
using System.Collections.Generic;
using PlusMusic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource cardSelect;

    public void PlayCardSelect()
    {
        cardSelect.Play();
    }

}
