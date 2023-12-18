using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClipRefsSO audioClipRefsSO;

    private void Start()
    {
        Player.OnAnyPistolFired += Player_OnAnyPistolFired;
        Player.OnAnyRifleFired += Player_OnAnyRifleFired;
        Player.OnAnyShotgunFired += Player_OnAnyShotgunFired;
        Player.OnAnyPistolReload += Player_OnAnyPistolReload;
        Player.OnAnyRifleReload += Player_OnAnyRifleReload;
        Player.OnAnyShotgunReload += Player_OnAnyShotgunReload;

    }



    private void Player_OnAnyShotgunReload(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        player.PlayAudioClip(audioClipRefsSO.shotgunReload);
    }

    private void Player_OnAnyRifleReload(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        player.PlayAudioClip(audioClipRefsSO.rifleReload);
    }

    private void Player_OnAnyPistolReload(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        player.PlayAudioClip(audioClipRefsSO.pistolReload);
    }

    private void Player_OnAnyPistolFired(object sender, System.EventArgs e)
    {
        Player player = sender as Player;

        PlaySound(audioClipRefsSO.rifleFire, player.transform.position);


    }
    private void Player_OnAnyShotgunFired(object sender, System.EventArgs e)
    {
        Player player = sender as Player;

        PlaySound(audioClipRefsSO.shotgunFire, player.transform.position);


    }
    private void Player_OnAnyRifleFired(object sender, System.EventArgs e)
    {
        Player player = sender as Player;

        PlaySound(audioClipRefsSO.rifleFire, player.transform.position);


    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

}
